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

namespace REMOEngine
{
    //On Developing. 아직 개발중인 컴포넌트들은 이 클래스 내부에 포함됩니다.
    public static class OnDev
    {
       
    }
    public static class TestScene2_Disposed
    {

        public class CustomTLReader : TLReader
        {
            public CustomTLReader() : base()
            {
                this.Embracer = "[]";
                this.AddRule("Script", (s) => { StandAlone.DrawString(s, new Point(100, 200), Color.White); });
            }
        }

         

        public static CustomTLReader t = new CustomTLReader();


        public static Scene scn = new Scene(() =>
        {

        }, () =>
        {

        }, () =>
        {
            t.ReadLine("[Script] My life is so great");
            //Strings.Draw();
            Cursor.Draw(Color.White);
        });

    }


    public static class TestScene4_Disposed
    {
        public static Scripter s = new Scripter(new Point(100, 100), 10, 0, 20);

        public static Scene scn = new Scene(() => {
            s.BuildScript("Yesterday, love is such an easy game to play, now I need a place to hide away. ");


        }, () => {



        }, () => {
            s.Script.Draw();
            Cursor.Draw(Color.White);
        });




    }

    public static class TestBedForREMOPoint
    {

        /// <summary>
        /// The Point class that is recommended to use. It's compatible with existing classes(Point,Vector2)
        /// </summary>
        public class REMOPoint
        {
            private Vector2 p;

            public float X
            {
                get { return p.X; }
                set { p.X = value; }
            }

            public float Y
            {
                get { return p.Y; }
                set { p.Y = value; }
            }

            public REMOPoint(float x, float y)
            {
                p = new Vector2(x, y);
            }

            //Binding to Point

            public static implicit operator Point(REMOPoint rmp)
            {
                return rmp.p.ToPoint();
            }
            public static implicit operator REMOPoint(Point p)
            {
                return new REMOPoint(p.X, p.Y);
            }

            //Binding to Vector2

            public static implicit operator Vector2(REMOPoint rmp)
            {
                return rmp.p;
            }
            public static implicit operator REMOPoint(Vector2 v)
            {
                return new REMOPoint(v.X, v.Y);
            }


            //overloading arithmetic operator. 

            public static REMOPoint operator *(REMOPoint p1, float c)
            {
                REMOPoint ret = new REMOPoint(c * p1.X, c * p1.Y); 
                return ret;
            }
            public static REMOPoint operator *(float c, REMOPoint p1)
            {
                REMOPoint ret = new REMOPoint(c * p1.X, c * p1.Y);
                return ret;
            }

            public static REMOPoint operator+(REMOPoint p1, Point p2)
            {
                REMOPoint ret = new REMOPoint(p1.X+p2.X,p1.Y+p2.Y);
                return ret;
            }
            public static REMOPoint operator +(REMOPoint p1, Vector2 p2)
            {
                REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
                return ret;
            }

            public static REMOPoint operator +(REMOPoint p1, REMOPoint p2)
            {
                REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
                return ret;
            }
            public static REMOPoint operator -(REMOPoint p1, Point p2)
            {
                REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
                return ret;
            }
            public static REMOPoint operator -(REMOPoint p1, Vector2 p2)
            {
                REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
                return ret;
            }

            public static REMOPoint operator -(REMOPoint p1, REMOPoint p2)
            {
                REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
                return ret;
            }



        }


        public static Action ExampleAct = () =>
          {
              REMOPoint p = new Point(5, 5);
              p += new Point(5, 5);
              p -= new Point(5, 5);
              p += new Vector2(5, 5);
              p -= new Vector2(5, 5);
              p = new REMOPoint(5, 5) - new Point(3, 3);
              p *= 5;
              p = 5.2f * p;
              p += new REMOPoint(5, 5);
              p -= new REMOPoint(5, 5);

              //var g = new Gfx2D("WhiteSpace", new REMOPoint(5, 5), 1.0f);
          };
    }


    public static class NewTest
    {
        public static GfxStr str = new GfxStr("1. Change the Font size(Press Q,W)", new REMOPoint(100, 100));
        public static GfxStr str2 = new GfxStr("KoreanFont", "자아. 도련님, 내가 따르는 술을 받아줘~", new REMOPoint(400, 400));

        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 200, 50, 50));
        public static Scene scn = new Scene(() => {
            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
        
        },
            () => {
                //Pressing Q,W changes size of the font.
                if(User.Pressing(Keys.Q))
                {
                    str2.FontSize++;
                    str.Text = "1. Change the Font size=" + str.FontSize+ "(Press Q,W)";
                }
                if (User.Pressing(Keys.W))
                {
                    str2.FontSize--;
                    str.Text = "1. Change the Font size=" + str.FontSize+ "(Press Q,W)";
                }
                //Pressing E rotates the square.
                if(User.Pressing(Keys.E))
                {
                    sqr.Rotate += 0.02f;
                }
                if(User.Pressing(Keys.A))
                {
                    sqr.Zoom(sqr.Center, 1.1f);
                }
                if (User.Pressing(Keys.S))
                {
                    sqr.Zoom(sqr.Center, 0.9f);
                }



            },
            () => {
                Filter.Absolute(StandAlone.FullScreen, Color.White);

                str.Draw(Color.Black);
                if (str.ContainsCursor())
                    str.Draw(Color.Red);


                sqr.Draw(Color.Black);
                if (sqr.RContains(Cursor.Pos))
                    sqr.Draw(Color.Red);

                str2.Draw(Color.Black);
                StandAlone.DrawString("2. Rotate the Square(Press E)", new REMOPoint(100, 300), Color.Black);



                Cursor.Draw(Color.Black);
            });
    
    }


 
}
