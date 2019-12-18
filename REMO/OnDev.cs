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
        public static Scene Debugger = new Scene(() => { }, () => { }, () =>
        {
        });
    }

    public static class REMODebugger // 레모 내장 디버거입니다.
    {
        public static string CurrentLog="";
        public static int SavedTimer = 0;
        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() => {

                TxtEditor.MakeTextFile("Logs", "DebugLog");
            });

        }, () => 
        {
            if (SavedTimer > 0)
                SavedTimer--;
            if (User.JustRightClicked()) //우클릭 : 지금 관심있는 디버그 로그를 로깅합니다.
            {
                TxtEditor.AppendLines("Logs", "DebugLog", new string[] { CurrentLog + "/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") });
                SavedTimer = 30;
            }
            if (User.JustPressed(Keys.LeftControl, Keys.D)) // Ctrl+D : 디버그 로그를 불러옵니다.
                System.Diagnostics.Process.Start(TxtEditor.MakePath("Logs","DebugLog"));
        }, () => 
        {
            if (User.Pressing(Keys.LeftControl, Keys.Q)) // Ctrl+Q : 현재 커서위치를 표시합니다.
            {
                CurrentLog = Cursor.Pos.X + "," + Cursor.Pos.Y;
                StandAlone.DrawString(CurrentLog, Cursor.Pos + new Point(30, 0), Color.White, Color.Black);
            }
            if (SavedTimer > 0)
                StandAlone.DrawString("Saved", StandAlone.FullScreen.Center-new Point(30,0), Color.White, Color.Black);
        });
    }
}
