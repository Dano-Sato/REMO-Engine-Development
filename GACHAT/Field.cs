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

namespace REMO_Engine_Developer.GACHAT
{
    //Template for new Scene. 나중에 커스텀 항목 템플릿을 통해 Scene을 만들 수 있게 계획하겠음.
    public static class FieldScene
    {
        /*Space for Scene properties*/

        //Write your own Update&Draw Action in here        
        public static Scene scn = new Scene(
            () =>
            {
                scn.InitOnce(() => {
                    StandAlone.FullScreen = new Rectangle(0, 0, 1920, 1080);
                    Cursor.Size = new Point(30, 30);
                    scn.Camera.TransformOrigin = StandAlone.FullScreen.Center;
                
                
                });

            },
            () =>
            {
                if (User.Pressing(Keys.Z))
                    scn.Camera.Zoom -= 0.01f;
                if (User.Pressing(Keys.X))
                    scn.Camera.Zoom += 0.01f;


            },
            () =>
            {
                StandAlone.DrawFullScreen("Field");
                Cursor.Draw(Color.White);
            }
            );

    }
}
