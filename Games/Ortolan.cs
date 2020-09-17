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
        public static Scene scn = new Scene(() =>
        {
            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
        }, () =>
        {
            if (StandAlone.FrameTimer % 30 <15 )
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

        }, () =>
        {
            Fader.DrawAll();

        });
    }
}
