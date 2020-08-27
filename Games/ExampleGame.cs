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

namespace Games
{
    public static class EatAppleGame
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 200, 50, 50));
        public static Gfx2D apple = new Gfx2D(new Rectangle(0, 0, 20, 20));
        public static float speed = 5f;
        public static int Score = 0;
        public static void EatApple()
        {
            Score += 10;
            speed += 0.1f;
            apple.Pos = new REMOPoint(StandAlone.Random(0, StandAlone.FullScreen.Width), StandAlone.Random(0, StandAlone.FullScreen.Height));
            //The apple teleports to the random point.
        }

        public static Scene scn = new Scene(
            () =>
            {
                scn.InitOnce(() =>
                {
                    apple.Pos = new REMOPoint(StandAlone.Random(0, StandAlone.FullScreen.Width), StandAlone.Random(0, StandAlone.FullScreen.Height));
                    apple.RegisterDrawAct(() =>
                    {
                        apple.Draw(Color.Red);
                        StandAlone.DrawString("I'm Apple!", apple.Pos + new REMOPoint(0, -30), Color.White * Fader.Flicker(100), Color.Black);
                    }
                    );
                });
            },
            () =>
            {
                User.ArrowKeyPAct((p) => { sqr.Center += p * speed; }); //The User-square moves when the user is pressing the arrow keys.
                if ((sqr.Center - apple.Center).Abs < 30)
                    EatApple();
            },
            () =>
            {
                sqr.Draw(Color.White, Color.Purple * 0.5f * Fader.Flicker(100));
                apple.Draw();
                Cursor.Draw(Color.White);
                StandAlone.DrawString("Press arrow keys to move square. and eat Apples.", new REMOPoint(344, 296), Color.White);
                StandAlone.DrawString("Score : " + Score, new REMOPoint(300, 45), Color.White);
            }
            );
    }

    public static class TestConsole
    {
        public static Scene scn = new Scene(() =>
        {

        }, () =>
        {

        }, () =>
        {
            StandAlone.DrawString(TLReader.FormatTag("test test test"), new REMOPoint(300, 300), Color.White);
        });
    }


   
}


        
