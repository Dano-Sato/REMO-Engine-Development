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

namespace REMO_Engine_Developer
{   //각종 StandAlone계열 클래스들을 담당. 게임 씬과 무관한 기본 기능들을 수행하는 컴포넌트들을 처리합니다.
    public static class StandAlone
    {


        /// <summary>
        /// 게임이 시작하고부터 지나간 프레임수를 측정합니다. 0으로 초기화할 수 있습니다.
        /// </summary>
        public static int FrameTimer = 0;
        /// <summary>
        /// 한 프레임에서 다음 프레임으로 넘어가는 밀리초를 측정합니다.
        /// </summary>
        public static int ElapsedMillisec = 0; 

        /// <summary>
        /// 이것은 함부로 불러서는 안됩니다.
        /// </summary>
        public static void InternalUpdate() 
        {
            FrameTimer++;
            Fader.Update();
            MusicBox.Update();
        }
        /// <summary>
        /// 이것은 함부로 불러서는 안됩니다. 
        /// </summary>
        public static void InternalDraw() 
        {
            /*
            Game1.Painter.OpenCanvas(() =>
            {
                Fader.DrawAll();
            });*/ //페이더를 그리는 함수가 기본 내장되어있는 것은 좋지 않다고 여겨 수정중입니다. 드로우 순서는 본인이 알고 배치하는 것이 좋습니다.
        }


        public static int DefaultFontSize = 30;
        /// <summary>
        /// 화면에 문자열을 그립니다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="c"></param>
        public static void DrawString(string s, Point p, Color c)
        {
            GfxStr t = new GfxStr(s, p);
            t.Draw(c);
        }
        /// <summary>
        /// 화면에 문자열을 그립니다. 백그라운드 색을 설정합니다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <param name="BackGroundColor"></param>
        public static void DrawString(string s, Point p, Color c, Color BackGroundColor)
        {
            GfxStr t = new GfxStr(s, p, 3);
            Filter.Absolute(t, BackGroundColor);
            t.Draw(c);
        }
        /// <summary>
        /// 고정된 사이즈의 게임스크린을 설정하고, 이후 이 스크린을 카메라를 통해 매핑하는 형식으로 해상도를 조정하는 것이 좋습니다. 게임스크린 사이즈가 지원하는 해상도보다 커야 합니다.
        /// </summary>
        public static readonly Rectangle GameScreen = new Rectangle(0,0,800,480);   


        /// <summary>
        /// 현재 게임의 풀스크린을 다룹니다.
        /// </summary>
        public static Rectangle FullScreen 
        {
            set
            {
                Game1.graphics.PreferredBackBufferWidth = value.Width;
                Game1.graphics.PreferredBackBufferHeight = value.Height;
                Game1.graphics.ApplyChanges();
            }
            get { return new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height); }
        }

        /// <summary>
        /// 윈도우 상단바가 없는 풀스크린 모드로 전환합니다.
        /// </summary>
        public static void ToggleFullScreen() 
        {
            Game1.graphics.ToggleFullScreen();
        }

        public static void DrawFullScreen(string SpriteName)
        {
            Gfx2D g = new Gfx2D(SpriteName, FullScreen);
            g.Draw();
        }


        public static void DrawFullScreen(string SpriteName, Color c)
        {
            Gfx2D g = new Gfx2D(SpriteName, FullScreen);
            g.Draw(c);
        }

     


        private static Random random = new Random();

        /// <summary>
        /// X,Y 사이의 랜덤한 값을 반환하는 함수입니다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Random(int x, int y)
        {
            return random.Next(Math.Min(x, y), Math.Max(x, y));
        }

        public static double Random()
        {
            return random.NextDouble();
        }
        /// <summary>
        /// 리스트 아이템 중 한개를 랜덤하게 픽합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Ts"></param>
        /// <returns></returns>
        public static T RandomPick<T>(List<T> Ts) 
        {
            double r = StandAlone.Random();
            double m = 1.0 / Ts.Count;
            for (int i = 0; i < Ts.Count; i++)
            {
                if (r >= m * i && r < m * (i + 1))
                {
                    return Ts[i];
                }
            }
            return Ts[0];
        }

    }
}
