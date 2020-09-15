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
using Games;
using Steamworks;

namespace REMOEngine
{
    public static class REMO_Steamworks
    {
        private static uint appID;
        private static SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
        private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
        {
            TxtEditor.AppendLinesToTop("Debug", "SteamLog", new string[] { pchDebugText.ToString() });
        }
        public static void EnableSteamFeatures(uint AppID)
        {
            appID = AppID;
            InternalInit();
        }
        public static void InternalInit()
        {
            TxtEditor.MakeTextFile("steam_appid");
            TxtEditor.WriteAllLines("steam_appid", new string[] { appID.ToString() });
            TxtEditor.MakeTextFile("Logs", "SteamLog");

            if (!Packsize.Test())
                throw new Exception("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            if (!DllCheck.Test())               
                throw new Exception("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            
            
            try
            {
                if (SteamAPI.RestartAppIfNecessary((AppId_t)appID))
                {
                    Game1.GameExit = true;
                }
            }
            catch(System.DllNotFoundException e)
            {
                Game1.GameExit = true;
                throw new Exception("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n"+e);
            }
            SteamAPI.Init();
        }

        public static void InternalUpdate()
        {
            if (m_SteamAPIWarningMessageHook == null)
            {
                m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
                SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
            }
            SteamAPI.RunCallbacks();
        }
    }
}
