﻿


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


namespace REMOEngine
{
    public static class Repository
    {
        //다른 클래스로 편입되기 전의 임시 그래픽스 혹은 각종 인수를 선언합니다.




        
    }

    public class Game1 : Game
    {
        #region Modified_Game1_SourceCode
        public static GraphicsDeviceManager graphics;
        public static LocalizedContentManager content;
        public static class Painter
        {
            private static Matrix CurrentMatrix;
            public static SpriteBatch spriteBatch;
            //private static bool isBegined = false;
            public static void Init() => spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            private static void BeginCanvas()
            {
                CurrentMatrix=Matrix.Identity;
                spriteBatch.Begin(SpriteSortMode.Immediate,
  BlendState.AlphaBlend,
  null,
  null,
  null,
  null,
  null);
            }
            private static void BeginCanvas(Camera2D CanvasCam) => BeginCanvas(CanvasCam.Transform);
            private static void BeginCanvas(Matrix m)
            {
                CurrentMatrix = m;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
    null,
    null,
    null,
    null,
    m);
            }
            private static void CloseCanvas()
            {
                CurrentMatrix = Matrix.Identity;
                spriteBatch.End();
            }

            public static void Draw(Gfx2D gfx) => Draw(gfx, Color.White);
            public static void Draw(Gfx2D gfx, Color c) => spriteBatch.Draw(gfx.Texture, new Rectangle(new REMOPoint(gfx.Pos.X + (gfx.ROrigin.X * gfx.Bound.Width) / gfx.Texture.Width, gfx.Pos.Y + (gfx.ROrigin.Y * gfx.Bound.Height) / gfx.Texture.Height), gfx.Bound.Size), null, c, gfx.Rotate, gfx.ROrigin, SpriteEffects.None, 0);
            public static void Draw(Gfx2D gfx, params Color[] cs)
            {
                for (int i = 0; i < cs.Length; i++)
                    Draw(gfx, cs[i]);
            }
            public static void Draw(GfxStr gfx) => Draw(gfx, Color.Black); 
            public static void Draw(GfxStr gfx, Color c)
            {
                Matrix m = CurrentMatrix;
                CloseCanvas();
                Vector2 pos = Vector2.Transform(gfx.Pos, m);
                OpenCanvas(Matrix2D.Zoom(pos,StandAlone.FontZoom*gfx.FontSize/StandAlone.SpriteFontSize),
                    () =>
                    {
                        spriteBatch.DrawString(gfx.Texture, gfx.Text, pos, c);
                    });
                BeginCanvas(m);
            }

            public static void Draw(GfxStr gfx, params Color[] cs)
            {
                for (int i = 0; i < cs.Length; i++)
                    Draw(gfx, cs[i]);
            }
            public static void ClearBackground(Color c) => graphics.GraphicsDevice.Clear(c);

            public static void OpenCanvas(Action a) // 캔버스 열고 닫기를 신경쓰지 않도록 만든 액션 실행 함수입니다.
            {
                BeginCanvas();
                a();
                CloseCanvas();
            }

            public static void OpenCanvas(Camera2D c, Action a)
            {
                BeginCanvas(c);
                a();
                CloseCanvas();
            }

            public static void OpenCanvas(Matrix m, Action a)
            {   
                BeginCanvas(m);
                a();
                CloseCanvas();
            }
        }

        public class LocalizedContentManager : ContentManager
        {
            public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory, CultureInfo currentCulture, string languageCodeOverride) : base(serviceProvider, rootDirectory)
            {
                this.CurrentCulture = currentCulture;
                this.LanguageCodeOverride = languageCodeOverride;
            }

            public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory) : this(serviceProvider, rootDirectory, Thread.CurrentThread.CurrentCulture, null)
            {
            }

            public static string ProcessContentString(string s)
            {
                if (s.Contains("."))
                    return s;
                else
                {
                    for (int i = 0; i < GAMEOPTION.NameSpaces.Length; i++)
                    {
                        if (Game1.content.assetExists(GAMEOPTION.NameSpaces[i] + "." + s))
                        {
                            return GAMEOPTION.NameSpaces[i] + "." + s;
                        }
                    }
                    return s;
                }
            }

            public override T Load<T>(string assetName)
            {
                assetName = ProcessContentString(assetName);
                string localizedAssetName = assetName + "." + this.languageCode();
                if (this.assetExists(localizedAssetName))
                {
                    return base.Load<T>(localizedAssetName);
                }
                return base.Load<T>(assetName);

            }

            private string languageCode()
            {
                if (this.LanguageCodeOverride != null)
                {
                    return this.LanguageCodeOverride;
                }
                return this.CurrentCulture.TwoLetterISOLanguageName;
            }

            public bool assetExists(string assetName)
            {
                return File.Exists(Path.Combine(base.RootDirectory, assetName + ".xnb"));
            }

            public string LoadString(string path, params object[] substitutions)
            {
                string assetName;
                string key;
                this.parseStringPath(path, out assetName, out key);
                Dictionary<string, string> strings = this.Load<Dictionary<string, string>>(assetName);
                if (!strings.ContainsKey(key))
                {
                    strings = base.Load<Dictionary<string, string>>(assetName);
                }
                return string.Format(strings[key], substitutions);
            }

            private void parseStringPath(string path, out string assetName, out string key)
            {
                int i = path.IndexOf(':');
                if (i == -1)
                {
                    throw new ContentLoadException("Unable to parse string path: " + path);
                }
                assetName = path.Substring(0, i);
                key = path.Substring(i + 1, path.Length - i - 1);
            }

            public LocalizedContentManager CreateTemporary()
            {
                return new LocalizedContentManager(base.ServiceProvider, base.RootDirectory, this.CurrentCulture, this.LanguageCodeOverride);
            }

            public CultureInfo CurrentCulture;
            public string LanguageCodeOverride;
        }

        public static bool GameExit = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Painter.Init();
            Game1.content = new LocalizedContentManager(base.Content.ServiceProvider, base.Content.RootDirectory);
            StandAlone.FullScreen = StandAlone.GameScreen;
            CustomInit();
        }

        protected override void UnloadContent()
        {

        }

        #endregion
        #region Engine Update&Draw
        protected override void Update(GameTime gameTime)
        {
            if (GameExit)
                Exit();

            Projectors.Update();
            StandAlone.InternalUpdate();
            CustomUpdate();
            User.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            StandAlone.ElapsedMillisec = gameTime.ElapsedGameTime.Milliseconds;
            Game1.Painter.ClearBackground(Color.Black);
            Projectors.Draw();
            StandAlone.InternalDraw();
            CustomDraw();
            base.Draw(gameTime);
        }

        #endregion


        protected void CustomInit()
        {
            GAMEOPTION.Build("MINE",MineCrazy.MiningScene.scn );


        }

        protected void CustomUpdate()
        {
        }
        protected void CustomDraw()
        {
        }
    }
  
    #region GAMEOPTION CLASS
    public static class GAMEOPTION // 게임의 빌드 옵션을 지정하는 클래스입니다.
    {
        // 게임의 옵션을 지정합니다. 

        public static string[] NameSpaces = { "REMO" };

        public static void Build(params Scene[] sceneInvocationList) // 외부 콘텐츠 없이 레모엔진 컴포넌트만으로 빌드합니다.
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("Content");
            foreach (System.IO.FileInfo File in di.GetFiles()) //특정 네임스페이스를 제외한 콘텐츠들을 제거합니다.
            {
                if (!File.Name.Contains("REMO"))
                {
                    File.Delete();
                }
            }
            GAMEOPTION.NameSpaces = new string[] { "REMO" };

            if (sceneInvocationList.Length > 0)
                Projectors.Projector.Load(sceneInvocationList[0]);//메인 씬을 불러옵니다.
            if (sceneInvocationList.Length > 1)
                Projectors.SubProjector.Load(sceneInvocationList[1]);//서브 씬을 불러옵니다.
            if (sceneInvocationList.Length > 2)
                Projectors.SubProjector2.Load(sceneInvocationList[2]);//서브2 씬을 불러옵니다.
        }


        public static void Build(string NameSpace, params Scene[] sceneInvocationList) // 특정 네임스페이스에 대해서 빌드합니다.
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("Content");
            foreach (System.IO.FileInfo File in di.GetFiles()) //특정 네임스페이스를 제외한 콘텐츠들을 제거합니다.
            {
                if (!File.Name.Contains(NameSpace) && !File.Name.Contains("REMO"))
                {
                    File.Delete();
                }
            }
            GAMEOPTION.NameSpaces = new string[] { "REMO", NameSpace };

            if (sceneInvocationList.Length > 0)
                Projectors.Projector.Load(sceneInvocationList[0]);//메인 씬을 불러옵니다.
            if (sceneInvocationList.Length > 1)
                Projectors.SubProjector.Load(sceneInvocationList[1]);//서브 씬을 불러옵니다.
            if (sceneInvocationList.Length > 2)
                Projectors.SubProjector2.Load(sceneInvocationList[2]);//서브2 씬을 불러옵니다.
        }



    }
    #endregion

}




