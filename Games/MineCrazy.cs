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
using System.Security.Cryptography;
using REMOEngine;
using System.Runtime.ExceptionServices;
using Steamworks;
using System.Security.Policy;

namespace MineCrazy
{


    public static class Pickaxe
    {
        public static int Power=10;
        private static int level=1;
        public static int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                Power = level * 9+(int)Math.Pow(1.5,level);
                TuningScene.Reinforcefee = (int)(50*(Math.Pow(1.5, level-1)));
                TuningScene.ReinforcePossibility = (float)(1.0 * Math.Pow(0.98, level - 1));
                TuningScene.ReinforceButton.ButtonGraphic = new GfxStr("Reinforce(R) : " + TuningScene.Reinforcefee + "G", new REMOPoint(400, 200));
            }
        }
        public static List<string> Enchants=new List<string>();
    }

    public static class UserInterface
    {
        public static int Gold = 0;
        public static int Trash = 0;
        public static Button TuningSceneButton = new Button(new GfxStr("(→)Go To Tuning Scene", new REMOPoint(800, 100)), () => {
            Projectors.Projector.SwapTo(TuningScene.scn); });
        public static Button MiningSceneButton = new Button(new GfxStr("(←)Go To Mining Scene", new REMOPoint(800, 100)), () => {
            Projectors.Projector.SwapTo(MiningScene.scn); });
        public static Aligned<Button> CurrentButtons=new Aligned<Button>(new REMOPoint(700,100),new REMOPoint(0,50)); 


        public static void SetButtons(params Button[] buttons)
        {
            CurrentButtons.Clear();
            foreach(Button b in buttons)
            {
                CurrentButtons.Add(b);
            }
        }
        public static void Update()
        {
            CurrentButtons.Align();
            foreach(Button b in CurrentButtons.Components)
            {
                b.Enable();
                if (User.JustLeftClicked(b))
                    break;
            }
        }
        public static void Draw()
        {
            StandAlone.DrawString(20, "Gold : " + Gold+"G", new REMOPoint(400, 50), Color.Gold);
            if(Trash>0)
            {
                StandAlone.DrawString(20, "Trash : " + Trash, new REMOPoint(400, 100), Color.White);
                StandAlone.DrawString(20, "Trash : " + Trash, new REMOPoint(400, 100), Color.Gray * 0.6f);
            }

            foreach (Button b in CurrentButtons.Components)
            {
                b.DrawWithAccent(Color.White, Color.Red);
            }
            StandAlone.DrawString("", new REMOPoint(800, 50), Color.White);

        }
    }
    public static class MiningScene
    {
        public static class Rock
        {
            public static Gfx2D Graphic=new Gfx2D(new Rectangle(100,100,200,200));
            private static int level=1;
            public static int Level
            {
                get { return level; }
                set 
                {
                    level = value;
                    MaxHP = (int)(30 * Math.Pow(1.5, level - 1));
                    CurrentHP = MaxHP;
                    MaxDEF = (int)(Math.Pow(1.5, level - 1));
                    CurrentDEF = MaxDEF;
                
                
                }
            }
            public static int MaxHP=30;
            public static int MaxDEF=1;
            public static int CurrentHP=30;
            public static int CurrentDEF=1;

            public static SimpleGauge HPGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 370, 1000, 15)));
            public static SimpleGauge DEFGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 420, 1000, 15)));


            public static void Update()
            {
                HPGauge.Update(CurrentHP, MaxHP);
                if(User.JustLeftClicked(Rock.Graphic)||User.JustPressed(Keys.Z))
                {
                    Rock.CurrentHP -= Math.Max(0, Pickaxe.Power - Rock.CurrentDEF);
                }
                if(Rock.CurrentHP<=0)
                {
                    Rock.CurrentHP = Rock.MaxHP;
                    GetReward();
                }
                if (User.JustPressed(Keys.S))
                    Rock.Level++;
                if (User.JustPressed(Keys.A))
                    Rock.Level=Math.Max(1,Rock.level-1);

            }
            public static void GetReward()
            {
                UserInterface.Gold += 10*(int)Math.Pow(2,Level-1);
            }

            public static void Draw()
            {
                Rock.Graphic.Draw(Color.White);
                if (Rock.Graphic.ContainsCursor())
                    Rock.Graphic.Draw(Color.Red);
                StandAlone.DrawString("level "+level+" Rock", Rock.Graphic.Center - new REMOPoint(50, 20), Color.Black);
                HPGauge.Draw(Color.Red);
                StandAlone.DrawString("HP : "+Rock.CurrentHP, HPGauge.Graphic.Pos-new REMOPoint(0,30), Color.Red);
                DEFGauge.Draw(Color.Green);
                StandAlone.DrawString("DEF :"+Rock.CurrentDEF, DEFGauge.Graphic.Pos - new REMOPoint(0, 30), Color.Green);

            }
        }


        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons(UserInterface.TuningSceneButton);

            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Right))
                Projectors.Projector.SwapTo(TuningScene.scn);
            Rock.Update();
        }, () =>
        {
            Rock.Draw();
            StandAlone.DrawString("Press Z to Mine!", new REMOPoint(400,200), Color.White);
            UserInterface.Draw();
            Cursor.Draw(Color.White);
        });
    }

    public static class TuningScene
    {
        public static int Reinforcefee = 50;
        public static float ReinforcePossibility = 1.0f;
        public static Button ReinforceButton = new Button(new GfxStr("Reinforce(R) : " + Reinforcefee+"G", new REMOPoint(400, 200)), () =>
               {
                   if(UserInterface.Gold>=Reinforcefee)
                   {
                       UserInterface.Gold -= Reinforcefee;
                       if(StandAlone.Random()<ReinforcePossibility)
                           Pickaxe.Level += 1;
                       else
                       {
                           UserInterface.Trash += Pickaxe.Level;
                           Pickaxe.Level = 1;
                       }

                   }
               });
        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons(UserInterface.MiningSceneButton);

        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Left))
                Projectors.Projector.SwapTo(MiningScene.scn);

            ReinforceButton.Enable();
            if(User.JustPressed(Keys.R))
            {
                ReinforceButton.ButtonClickAction();      
            }

        }, () =>
        {
            StandAlone.DrawString("level " + Pickaxe.Level + " Pickaxe", new REMOPoint(50, 100), Color.White);
            StandAlone.DrawString("Power : "+Pickaxe.Power, new REMOPoint(50, 150), Color.White);
            StandAlone.DrawString("Possibility : " + (int)(ReinforcePossibility*100)+"%", ReinforceButton.Pos + new REMOPoint(0, 50), Color.White);
            UserInterface.Draw();
            ReinforceButton.DrawWithAccent(Color.Gold, Color.Red);
            Cursor.Draw(Color.White);
        });

    }

    public static class LandFillScene
    {

    }


    public static class ScriptScene
    {
        public static Scripter DialogReader = new Scripter();
        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() =>
            {
                DialogReader.AddRule("n", (s) => { });
            });

        }, () =>
        {

        }, () =>
        {

        });

    }

}
