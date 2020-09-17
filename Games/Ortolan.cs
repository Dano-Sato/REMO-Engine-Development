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

namespace Ortolan
{
    public static class Testboard
    {

        public static Gfx2D slime = new Gfx2D("ORTO.Slime1", new REMOPoint(400, 400), 0.5f);
        public static Centipede testCent = new Centipede(new Gfx2D("ORTO.Centipede",new Rectangle(0, 0, 50, 50)));
        public static Gfx2D TestOrtolan = new Gfx2D(new Rectangle(900, 500, 50, 50));
        public static Scene scn = new Scene(() =>
        {
            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
            for(int i=0;i<5;i++)
            {
                testCent.Bodies.Add(new Gfx2D("ORTO.Centipede", new Rectangle(0, 40+i*50, 50, 50)));
            }
        }, () =>
        {
            ButterFly.Update();
            if (StandAlone.FrameTimer % 60 == 30)
                slime.Sprite = "Slime1";
            else if (StandAlone.FrameTimer % 60 == 0)
                slime.Sprite = "Slime2";
            testCent.Update(TestOrtolan.Center,5.0);
            User.ArrowKeyPAct((p) => { TestOrtolan.MoveByVector(p, 4.0); });
         

        }, () =>
        {
            StandAlone.DrawFullScreen("Title");
            slime.Draw();
            testCent.Head.Draw(Color.White);
            foreach(Gfx2D g in testCent.Bodies)
            {
                g.Draw(Color.White);
            }
            TestOrtolan.Draw();
            Fader.DrawAll();
        });
    }

    public class Centipede
    {
        public Gfx2D Head;
        public List<Gfx2D> Bodies=new List<Gfx2D>();

        public Centipede(Gfx2D head)
        {
            Head = head;
        }
        public void Update(REMOPoint destination, double speed)
        {
            int size = Head.Width;
            Vector2 v = Vector2.Normalize((destination - Head.Center).ToVector2());
            Vector2 n = new Vector2(v.Y, -v.X);

            Head.MoveByVector((v-((float)speed/2)*n*(float)Math.Sin(0.1f*StandAlone.FrameTimer))*10.0f, speed);
            for(int i=0;i<Bodies.Count;i++)
            {
                
            {
                    float interval = 0.3f;
                    float f= (Bodies[i].Center - Head.Center).ToVector2().Length()-interval*size;
                if (i == 0&&f > 0)
                    Bodies[i].MoveTo(Head.Center, f*interval/2);
                if(i!=0)
                    f = (Bodies[i].Center - Bodies[i - 1].Center).ToVector2().Length() - interval*size;
                if (i != 0 && f>0)
                    Bodies[i].MoveTo(Bodies[i - 1].Center, f*interval/2);
            }
        }
        }

    }
    
    public static class ButterFly
    {
        public static int KillCool = 30;
        public static void Update()
        {
            if (StandAlone.FrameTimer % 30 < 15)
            {
                Gfx2D g = new Gfx2D("CURSOR1", Cursor.Pos, 1.0);
                g.Center = Cursor.Pos;
                Fader.Add(g, 30, Color.White * 0.2f);
            }
            if (StandAlone.FrameTimer % 30 >= 15)
            {
                Gfx2D g = new Gfx2D("CURSOR2", Cursor.Pos, 1.0);
                g.Center = Cursor.Pos;
                Fader.Add(g, 30, Color.White * 0.2f);
            }
        }
    }
}
