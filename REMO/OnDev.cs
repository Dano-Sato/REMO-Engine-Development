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

        private static void SaveLog() //로그를 저장합니다.
        {
           if(DebugMode!=DebuggerMode.NULL)
                TxtEditor.AppendLinesToTop("Logs", "DebugLog", new string[] { LogNameWriter.TypeLine + "=" + CurrentLog });
           else
                TxtEditor.AppendLinesToTop("Logs", "DebugLog", new string[] { LogNameWriter.TypeLine });

            SavedTimer = 30;
        }

        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() => {
                TxtEditor.MakeTextFile("Logs", "DebugLog"); // 디버깅 기록을 로그할 텍스트파일을 만듭니다.
                LogNameWriter.AddCustomType(Keys.Space, " ");
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
                    CurrentLog = "new Point("+Cursor.Pos.X+","+Cursor.Pos.Y+")";
                    break;
                case DebuggerMode.ShowRectangle:
                    if (User.JustLeftClicked())
                        Bound = new Rectangle(Cursor.Pos, Point.Zero);
                    if(User.Pressing(MouseButtons.LeftMouseButton))
                    {
                        Bound = new Rectangle(Bound.Location, Cursor.Pos - Bound.Location);
                    }
                    CurrentLog = "new Rectangle(" + Bound.X + "," + Bound.Y + "," + Bound.Width + "," + Bound.Height + ")";

                    break;
            }
            DebugKeyAct(Keys.S, () =>
            {
                SaveLog();
                System.Diagnostics.Process.Start(TxtEditor.MakePath("Logs", "DebugLog"));
            });// Ctrl+S : 현재 로그를 저장하고 디버그 로그를 불러옵니다.
            DebugModeAct(Keys.Q,DebuggerMode.ShowCursorPos);// Ctrl+Q : 커서 위치를 보여주는 모드를 불러옵니다.
            DebugModeAct(Keys.W, DebuggerMode.ShowRectangle);// Ctrl+W : 사각형을 보여주는 모드를 불러옵니다.


            //로그에 주석을 달아주는 라이터 항목에 관한 업데이트입니다.
            if(WriterIsOn&&!User.Pressing(Keys.LeftControl))
                LogNameWriter.Update();
            if(User.JustPressed(Keys.Enter))
            {
                if (WriterIsOn == true)
                    LogNameWriter.Empty();
               WriterIsOn = !WriterIsOn;

            }

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
            
            if(WriterIsOn)
            {
                StandAlone.DrawString("> " + LogNameWriter.TypeLine, Cursor.Pos + new Point(30, 20), Color.SkyBlue, Color.Black);
            }

            StandAlone.DrawString("DEBUG", new Point(0, 0), Color.White, Color.Black);
        });
        private static bool WriterIsOn = false;
        private static TypeWriter LogNameWriter=new TypeWriter(); //로깅할 때 로그에 주석을 달 수 있습니다.

        public static void Enable() //이녀석을 업데이트 함수에 넣을 경우, Ctrl+Alt+Q를 통해 디버거를 로드할 수 있는 상태가 됩니다.
        {
            if (User.JustPressed(Keys.LeftControl, Keys.LeftAlt, Keys.Q))
            {
                if (!Projector.Loaded(REMODebugger.scn))
                {
                    Projector.PauseAll();
                    Projector.Load(REMODebugger.scn);
                }
                else
                {
                    Projector.ResumeAll();
                    Projector.Unload(REMODebugger.scn);
                }
            }
        }
    }
}
