using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using REMOEngine;


namespace Yokai
{
    public static class ScripterScene
    {
        public static Scripter ScriptReader = new Scripter();
        public static int line = 0;
        public static string[] Scripts = TxtEditor.ReadAllLines("Scripts", "Test");
        public static string currentString="";
        public static Action AfterAction=()=> { };
        public static Gfx2D testCG = new Gfx2D("YOKAI.TestCG2", new REMOPoint(800, 0), 0.6f);

        public static void EnterScript(string ScriptName, Action afterAction)
        {
            Scripts = TxtEditor.ReadAllLines("Scripts", ScriptName);
            Projectors.Projector.Load(scn);
            AfterAction = afterAction;
 
        }
        
        public static Scene scn = new Scene(() =>
        {

            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);


            //Rule for Name
            ScriptReader.AddRule("n", (s) => { StandAlone.DrawString(20, "KoreanFont", s, new REMOPoint(300, 300), Color.Black); });
            //Rule for Script
            ScriptReader.AddRule("s", (s) => {

                s = s.Replace("@", "\n");   
                if (currentString.Length < s.Length&&StandAlone.FrameTimer%4==0)
                {
                    currentString = s.Replace("%%", "\n").Substring(0, currentString.Length + 1);
                }
                StandAlone.DrawString(20, "KoreanFont", currentString, new REMOPoint(300, 500), Color.Black); });

        }, () =>
        {

            if (User.JustPressed(Keys.Z)||User.JustPressed(Keys.Space)||User.JustLeftClicked())
            {
                string currentScript = ScriptReader.ParseLine("s", Scripts[line]);
                currentScript = currentScript.Replace("@", "\n");
                if (currentString.Length == currentScript.Length)
                {
                    line++; //다음 라인으로 넘어갑니다.
                    currentString = "";
                    if(line==Scripts.Length)//스크립트를 다 읽었을 경우, 지정해둔 액션으로 넘어갑니다.
                    {
                        Projectors.Projector.Unload(scn);
                        AfterAction();
                    }
                }
                else
                {
                    currentString = currentScript;
                }
            }
               

        }, () =>
        {
            Filter.Absolute(StandAlone.FullScreen, Color.White);
            testCG.Draw();
            ScriptReader.ReadLine(Scripts[line]);
            Cursor.Draw(Color.Black);

        });
    }

    public static class TownScene
    {
        public static GfxStr Ryokan = new GfxStr(20, "KoreanFont", "요괴 여관", new REMOPoint(200, 300));
        public static GfxStr Shrine = new GfxStr(20, "KoreanFont", "아마노카가미 신사", new REMOPoint(1000, 200));
        public static GfxStr Shoten = new GfxStr(20, "KoreanFont", "네코마타 상점가", new REMOPoint(300, 700));
        public static GfxStr Nanashiyama = new GfxStr(20, "KoreanFont", "이름없는 산", new REMOPoint(1250, 400));
        public static GfxStr Tanukimori = new GfxStr(20, "KoreanFont", "너구리 숲", new REMOPoint(1200, 550));
        public static GfxStr Ningennomura = new GfxStr(20, "KoreanFont", "인간 마을", new REMOPoint(1000, 700));
        public static List<GfxStr> Towns = new List<GfxStr>();



        public static Scene scn = new Scene(() =>
        {
            Towns.Add(Ryokan);
            Towns.Add(Shrine);
            Towns.Add(Shoten);
            Towns.Add(Nanashiyama);
            Towns.Add(Tanukimori);
            Towns.Add(Ningennomura);

        }, () =>
        {

        }, () =>
        {
            foreach (GfxStr s in Towns)
                s.Draw(Color.White);

            Cursor.Draw(Color.White);

        });
    }

    public static class Commands
    {

    }


    public class Ingredient
    {
        public int Code { get; set;}        
        public string Name { get; set; }

        public int Freshness { get; set; }
        public List<string> Buffs;


        public Ingredient(int code, params string[] buffs)
        {
            Code = code;
            Name = "Default";
            Freshness = 100;
            Buffs = buffs.ToList();
        }

        public Ingredient(Ingredient another)
        {
            Code = another.Code;
            Name = another.Name;
            Freshness = another.Freshness;
            Buffs = another.Buffs;
        }

        public string ToScript()
        {
            Dictionary<string, string> Builder = new Dictionary<string, string>();
            Builder.Add("Code", Code.ToString());
            Builder.Add("Name", Name.ToString());
            Builder.Add("Freshness", Freshness.ToString());
            StringBuilder buffstring = new StringBuilder();
            for(int i=0;i<Buffs.Count;i++)
            {
                buffstring.Append(Buffs[i]);
                if (i != Buffs.Count - 1)
                    buffstring.Append("%%");
            }
            Builder.Add("Buff", buffstring.ToString());
            Builder.Add("End", String.Empty);
         
            return Scripter.BuildLine(Builder);
        }       
    }

    public static class Test_Scripter
    {
        public static Ingredient testIngredient = new Ingredient(1, "SClass","Fermented");
        public static Scene scn = new Scene(() =>
        {
            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
        }, () =>
        {            

        }, () =>
        {
            StandAlone.DrawFullScreen("RyokanBG");
            StandAlone.DrawString(testIngredient.ToScript(), new REMOPoint(200, 200), Color.White);
            StandAlone.DrawString(50, "KoreanFont", "요괴 여관",new REMOPoint(50,50), Color.Black);

        });

    }
}
