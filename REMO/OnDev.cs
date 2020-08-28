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
        public static GfxStr str2 = new GfxStr(20, "KoreanFont", "3. 한글폰트 지원완료", new REMOPoint(100, 350));

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


    public static class ScripterTest
    {
        public static TLReader ScriptPainter = new TLReader();
        public static int line = 0;
        public static string TestScript = "<n>결계 무녀<s>...일월은 나의 형, 하늘은 나의 벗...%" +
            "<n>결계 무녀<s>...욕사지귀는 당아하고 불욕사지귀는 피아하라.%" +
            "<s>무녀의 말이 끝나자마자 요괴들은 검은 글자의 주박에 묶여 쓰러졌다.%" +
            "<n>결계 무녀<s>이런 녀석들한테 쓰러질 줄은 몰랐는데 말이야. 식신을 붙여두길 잘했네.%" +
            "<n>결계 무녀<s>동생아. 처리하렴.%" +
            "<n>검 무녀<s>응.%" +
            "<s>시리도록 하얀 검 끝이 달빛으로 빛나며, 요괴들이 차례차례 잘려나갔다.%" +
            "<s>무녀가 이쪽으로 걸어왔다.%" +
            "<n>결계 무녀<s>어때 너희들. 이번에는 신세를 졌지? 일단 요괴 퇴치는 내가 시킨 일이니까, 책임은 지도록 할게...%" +
            "<n>결계 무녀<s>요괴가 돼서 인간의 신세를 지고 싶지 않으면, 더 강해지도록 해.%" +
            "<s>당신은 눈 앞이 점점 흐려지는 것을 느낀다.%" +
            "<n>결계 무녀<s>일어났어? 몸은 좀 괜찮아?%" +
            "<s>다행히 다친 부분은 없는 듯 하다.%" +
            "<n>결계 무녀<s>네 부하들은 먼저 여관으로 돌아갔어. 주인이 돼서 회복이 늦구나, 너.%" +
            "<n>결계 무녀<s>부끄러우면 네 동료들과 함께 더 강해지도록 해. 자, 이건 선물이야.%" +
            "<s>폭발의 부적*3을 받았다.%" +
            "<n>결계 무녀<s>위급할 땐 이걸 요괴에게 던지고 주문을 외우면 돼. 적당한 위력은 나올 거야.%" +
            "<n>결계 무녀<s>그럼 요괴 퇴치, 힘내.%";

        public static string[] Scripts;

        public static Scene scn = new Scene(() =>
        {

            StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);


            //Rule for Name
            ScriptPainter.AddRule("n", (s)=> { StandAlone.DrawString(20, "KoreanFont", s, new REMOPoint(300, 300), Color.White); });
            //Rule for Script
            ScriptPainter.AddRule("s", (s) => { StandAlone.DrawString(20, "KoreanFont", s, new REMOPoint(300, 500), Color.White); });

            Scripts = TestScript.Split('%');
        }, () =>
        {
            if (User.JustPressed(Keys.Z))
                line++;
            
            
        }, () =>
        {
            Filter.Absolute(StandAlone.FullScreen, Color.Black);
            ScriptPainter.ReadLine(Scripts[line]);
        });

    }


}
