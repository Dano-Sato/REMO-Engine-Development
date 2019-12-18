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
    //On Developing. 아직 개발중인 컴포넌트들은 이 클래스 내부에 포함됩니다.
    public static class OnDev
    {
       
    }

    public static class REMODebugger // 레모 내장 디버거입니다.
    {
        private static string CurrentLog="";
        private static int SavedTimer = 0;
        private static Rectangle Bound = Rectangle.Empty;
        enum DebuggerMode
        {
            NULL,
            ShowCursorPos,
            ShowRectangle
        }
        private static DebuggerMode DebugMode = DebuggerMode.NULL;

        private static void DebugKeyAct(Keys k, Action a)
        {
            if (User.JustPressed(Keys.LeftControl, k))
                a();
        }

        private static void DebugModeAct(Keys k, DebuggerMode m)
        {
            DebugKeyAct(k, () => {
                if (DebugMode == m)
                    DebugMode = DebuggerMode.NULL;
                else
                    DebugMode = m;
            });// 특정 키에 대해 디버그모드 m을 할당합니다.

        }

        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() => {

                TxtEditor.MakeTextFile("Logs", "DebugLog"); // 디버깅 기록을 로그할 텍스트파일을 만듭니다.
            });

        }, () =>
        {
            if (SavedTimer > 0)
                SavedTimer--;
            switch (DebugMode)
            {
                case DebuggerMode.NULL:
                    CurrentLog = "";
                    break;
                case DebuggerMode.ShowCursorPos:
                    CurrentLog = Cursor.Pos.ToString();
                    break;
                case DebuggerMode.ShowRectangle:
                    if (User.JustLeftClicked())
                        Bound = new Rectangle(Cursor.Pos, Point.Zero);
                    if(User.Pressing(MouseButtons.LeftMouseButton))
                    {
                        Bound = new Rectangle(Bound.Location, Cursor.Pos - Bound.Location);
                    }
                    CurrentLog = Bound.ToString();

                    break;
            }
            if (User.JustRightClicked()) //우클릭 : 지금 관심있는 디버그 로그를 로깅합니다.
            {
                TxtEditor.AppendLines("Logs", "DebugLog", new string[] { CurrentLog + "/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") });
                Bound = Rectangle.Empty;
                SavedTimer = 30;
            }
            DebugKeyAct(Keys.D, () =>
            {
                System.Diagnostics.Process.Start(TxtEditor.MakePath("Logs", "DebugLog"));
            });// Ctrl+D : 디버그 로그를 불러옵니다.
            DebugModeAct(Keys.Q,DebuggerMode.ShowCursorPos);// Ctrl+Q : 커서 위치를 보여주는 모드를 불러옵니다.
            DebugModeAct(Keys.W, DebuggerMode.ShowRectangle);// Ctrl+W : 사각형을 보여주는 모드를 불러옵니다.

            DebugKeyAct(Keys.C, () =>
            {
                TxtEditor.Clear("Logs", "DebugLog");
            });// Ctrl+C : 로그 기록을 전부 지웁니다.
        }, () => 
        {
            if (DebugMode == DebuggerMode.ShowRectangle) 
            {
                Filter.Absolute(Bound, Color.Red * 0.5f);//사각형 영역을 표시합니다.
            }

            if (DebugMode!=DebuggerMode.NULL) //NULL 모드가 아닐시 현재 로그를 불러옵니다.
            {
                StandAlone.DrawString(CurrentLog, Cursor.Pos + new Point(30, 0), Color.White, Color.Black);
            }
            if (SavedTimer > 0)
                StandAlone.DrawString("Saved", StandAlone.FullScreen.Center-new Point(30,0), Color.White, Color.Black);
        });
    }
}
