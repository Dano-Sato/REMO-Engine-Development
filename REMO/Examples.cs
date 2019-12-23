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
{
    public static class ExampleScene1 // 사각형이 튕기는 씬입니다.
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 200, 50, 50));
        public static Gfx2D sqr2 = new Gfx2D(new Rectangle(200, 400, 300, 20));

        public static Vector2 v = new Vector2(0, 0);
        public static Vector2 g = new Vector2(0, 0.2f);
        public static double speed = 0;
        public static void MoveSquare() => sqr.Pos += v.ToPoint();


        public static Scene scn = new Scene(
            () =>
            {

            },
            () =>
            {
                v += g;//이 물체는 중력을 받는다.

                MoveSquare();//속도벡터에 의해 물체를 움직인다.
                if (Rectangle.Intersect(sqr.Bound, sqr2.Bound) != Rectangle.Empty)//충돌 판정
                {
                    v = -v;//바에 충돌할 경우, 속도벡터가 바뀐다.
                    MoveSquare();
                }
            },
            () =>
            {
                sqr.Draw(Color.White);
                sqr2.Draw(Color.Red);

            }
            );

    }
}
