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
        public static Scene scn = new Scene(() =>
        {
            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
        }, () =>
        {
            ButterFly.Update();
            if (StandAlone.FrameTimer % 60 == 30)
                slime.Sprite = "Slime1";
            else if (StandAlone.FrameTimer % 60 == 0)
                slime.Sprite = "Slime2";

         

        }, () =>
        {
            StandAlone.DrawFullScreen("Title");
            slime.Draw();
            Fader.DrawAll();

        });
    }

    public class Centipede
    {
        
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
