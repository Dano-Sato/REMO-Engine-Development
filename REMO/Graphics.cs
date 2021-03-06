﻿using System;
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
    public abstract class Gfx : IMovable, IDrawable, IBoundable// 기본적인 Bound 연산 및 무브먼트 함수를 지원 + 드로우 함수 지정
    {
        protected Rectangle bound;
        public virtual Rectangle Bound
        {
            get { return bound; }
            set { bound = value; }
        }
        public REMOPoint Pos //왼쪽 위를 포지션으로 지정한다.
        {
            get { return Bound.Location; }
            set { bound.Location = value; }
        }

        public REMOPoint Center
        {
            get { return bound.Center; }
            set { bound.Location = new REMOPoint(value.X - bound.Width / 2, value.Y - bound.Height / 2); }
        }

        public int X
        {
            get { return bound.X; }
            set { bound.X = value; }
        }

        public int Y
        {
            get { return bound.Y; }
            set { bound.Y = value; }
        }
        public int Width
        {
            get { return bound.Width; }
            set { bound.Width = value; }
        }
        public int Height
        {
            get { return bound.Height; }
            set { bound.Height = value; }
        }

        public void MoveTo(REMOPoint p) => Pos = p;
        public void MoveByVector(REMOPoint v, double speed) => Pos += v * ((float)speed / v.Abs);
        public void MoveTo(int x, int y, double speed) => MoveTo(new REMOPoint(x, y), speed);
         public void MoveTo(REMOPoint Destination, double speed)
        {
            float distance = (Destination - Pos).Abs;
            if (distance < speed)
                Pos = Destination;
            else
                MoveByVector(Destination - Pos, speed);
        }


        protected Action DrawAction;
        public void Draw()
        {
            if (DrawAction == null)
                Draw(Color.White);
            else
                DrawAction();
        }

        public void RegisterDrawAct(Action a) //Gfx에 커스터마이즈된 드로우함수를 달아줄 수 있습니다. 이를 통해 쉽게 커스텀함수를 불러올 수 있습니다. 
        {
            DrawAction = a;
        }

        public abstract void Draw(params Color[] colors);

        public void DrawAddon(string AddonName, params Color[] cs) //Graphic의 바운드에 새 애드온을 그립니다.
        {
            Game1.Painter.Draw(new Gfx2D(AddonName, Bound), cs);
        }

        public void DrawAddon(string AddonName, REMOPoint Vector, REMOPoint newSize, params Color[] cs)//Graphic의 position을 기준으로 하는 새 사이즈를 가진 애드온을 그립니다.
        {
            Game1.Painter.Draw(new Gfx2D(AddonName, new Rectangle(Pos + Vector, newSize)), cs);
        }
        public void DrawAddon(string AddonName, REMOPoint newSize, params Color[] cs)//Graphic의 position에 새 사이즈를 가진 애드온을 그립니다.
        {
            Game1.Painter.Draw(new Gfx2D(AddonName, new Rectangle(Pos, newSize)), cs);
        }

        public bool Contains(REMOPoint p)
        {
            return Bound.Contains(p.ToPoint());
        }
        public bool ContainsCursor()
        {
            return Contains(Cursor.Pos);
        }


        //Animation 처리.

        private class ALReader : Scripter // ALReader는 커스텀 룰이 적용된 TLReader입니다. Animation Language Reader입니다. Animation에 필요한 Tag Language를 분석합니다.
        {
            public bool isLooped = false;
            public List<string> IndicationList = new List<string>();
            public ALReader()
            {
                /* TL을 읽는 두가지 룰을 만듭니다. Tag를 통해 Loop Setting을 구분하고, 지시문 리스트를 만듭니다.
                 * incompleted - <FR>5<Loop>S1-S2-S3-S4 (in this manner)
                 * */
                AddRule("Loop", (s) =>
                {
                    isLooped = true;
                    IndicationList = Scripter.Parse(s, "->").ToList();
                });
                AddRule("ReadOnce", (s) =>
                {
                    isLooped = false;
                    IndicationList = Scripter.Parse(s, "->").ToList();
                });
            }
        }
        private ALReader aLReader = new ALReader();
        private REMOPoint Slicer = new REMOPoint(1, 1);
        private string CurrentStatement;
        private int AnimationTimer = 0;

        public void Animate(string statement, int FrameRate) // FrameRate만큼의 Frame이 지나면 스프라이트를 교체하는 형식의 애니메이트를 합니다. Update 함수에서 호출되어야 정상적으로 작동합니다.
        {
            if (CurrentStatement != statement) //Statement가 변경되면
            {
                aLReader.ReadLine(statement);//새로운 지시문 리스트를 만듭니다.
                AnimationTimer = 0;
            }
            else//변경되지 않았을 경우 리스트 내부를 처리합니다.
            {
                if (AnimationTimer == 0)
                {
                    //스프라이트 교체 작업을 실시합니다.(아직 구현 안됨)

                    AnimationTimer = FrameRate;
                }
                else
                {
                    AnimationTimer--;
                }
            }
        }


    }


    public class GfxStr : Gfx
    {
        public override Rectangle Bound
        {
            set { bound = new Rectangle(value.Location, new REMOPoint(value.Size)*( FontSize / StandAlone.SpriteFontSize)); }
            get { return bound; }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                Bound = new Rectangle(Pos, Texture.MeasureString(Text).ToPoint());
            }
        }
        public SpriteFont Texture;
        public int FontSize = StandAlone.DefaultFontSize;

        public GfxStr(GfxStr s)
        {
            FontSize = s.FontSize;
            Texture = s.Texture;
            Text = s.Text;
            Bound = new Rectangle(s.Pos, Texture.MeasureString(Text).ToPoint());
        }

        public GfxStr(string _text) : this(_text, REMOPoint.Zero) { }
        public GfxStr(string text, REMOPoint pos)
        {
            Texture = Game1.content.Load<SpriteFont>("DefaultFont");
            Text = text;
            Bound = new Rectangle(pos, Texture.MeasureString(Text).ToPoint());
        }
        public GfxStr(int fontSize, string text) : this(fontSize, text, Point.Zero)
        {
        }

        public GfxStr(int fontSize, string text, REMOPoint pos)
        {
            FontSize = fontSize;
            Texture = Game1.content.Load<SpriteFont>("DefaultFont");
            Text = text;
            Bound = new Rectangle(pos, Texture.MeasureString(Text).ToPoint());
        }


        public GfxStr(string font, string text) : this(font, text, REMOPoint.Zero) { }
        public GfxStr(string font, string text, REMOPoint pos)
        {
            Texture = Game1.content.Load<SpriteFont>(font);
            Text = text;
            Bound = new Rectangle(pos, Texture.MeasureString(Text).ToPoint());
        }

        public GfxStr(int fontSize, string font, string text) : this(fontSize, font, text, REMOPoint.Zero) { }
        public GfxStr(int fontSize, string font, string text, REMOPoint pos)
        {
            FontSize = fontSize;
            Texture = Game1.content.Load<SpriteFont>(font);
            Text = text;
            Bound = new Rectangle(pos, Texture.MeasureString(Text).ToPoint());
        }
        public override void Draw(params Color[] colors) => Game1.Painter.Draw(this, colors);
    }

    public class Gfx2D : Gfx
    {
        public Texture2D Texture;
        public string Sprite
        {
            get { return Texture.ToString().Split('.')[1]; }
            set
            {
                Texture = Game1.content.Load<Texture2D>(value);
            }
        }



        public REMOPoint ROrigin; // 회전 중심점. 재할당을 하지 않을 경우 텍스처의 중심이 회전중심이 됩니다. 재할당을 할 때는 물체의 왼쪽 위 지점을 (0,0)이라고 생각해주십시오. 중요한 점은, 실제 텍스처 파일의 가로 세로값을 참조한다는 것입니다.
        public float Rotate;  // 회전각. radian을 따릅니다.



        /// <summary>
        /// 메소드를 사용할 수 없는 Null 객체를 생성합니다.
        /// </summary>
        public Gfx2D() 
        {

        }
        public Gfx2D(Rectangle boundRect) : this("REMO.WhiteSpace", boundRect) { }

        public Gfx2D(string SpriteName, Rectangle boundRect)
        {
            Texture = Game1.content.Load<Texture2D>(SpriteName);
            Bound = boundRect;
            ROrigin = new REMOPoint(Texture.Width / 2, Texture.Height / 2);
        }

        public Gfx2D(string SpriteName, REMOPoint p, double r)
        {
            Texture = Game1.content.Load<Texture2D>(SpriteName);
            Bound = new Rectangle(p, new REMOPoint((int)(Texture.Width * r), (int)(Texture.Height * r)));
            ROrigin = new REMOPoint(Texture.Width / 2, Texture.Height / 2);
        }


        public override void Draw(params Color[] colors) => Game1.Painter.Draw(this, colors);


        /// <summary>
        /// 만약 Gfx의 rotate값이 0이 아닐 경우, RContains 함수가 정확합니다. 다만, 보시다시피, 비싼 계산을 요구합니다.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool RContains(REMOPoint p)
        {
            Vector2 v = p;
            REMOPoint RO = new REMOPoint(Pos.X + (ROrigin.X * Bound.Width) / Texture.Width, Pos.Y + (ROrigin.Y * Bound.Height) / Texture.Height); //실제 회전중심의 위치를 잡습니다.
            v = Vector2.Transform(v, Matrix2D.Rotate(RO, -Rotate));
            return Bound.Contains(v);
        }
        public bool RContainsCursor()
        {
            return RContains(Cursor.Pos);
        }
        /// <summary>
        /// The method has some error while casting Vector2 to Point. Not Precise.
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="r"></param>
        public void Zoom(REMOPoint Origin, float r)
        {
            REMOPoint v = Pos;
            REMOPoint v2 = Pos + Bound.Size;// Get Right lower point of the Bound.
            v = Vector2.Transform(v, Matrix2D.Zoom(Origin, r));
            v2 = Vector2.Transform(v2, Matrix2D.Zoom(Origin, r));
            Bound = new Rectangle(v, v2 - v);
        }

    }

    public class Camera2D
    {
        protected float zoom=1.0f; // Camera Zoom
        protected float rotation=0.0f; // Camera Rotation
        public Matrix Transform //카메라에 의한 변환행렬을 불러옵니다.
        {
            get
            {
                return Matrix2D.Translate(REMOPoint.Zero - Origin) * Matrix2D.Mat2D(TransformOrigin, rotation, Zoom);
            }
        } // Matrix Transform
        public REMOPoint Origin=REMOPoint.Zero; // Camera Position
        public REMOPoint TransformOrigin= REMOPoint.Zero;//회전, 줌 변환의 중심입니다.

        public Camera2D() { }
        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
            } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }



    }
    public static class Projectors
    {
        public static projector Projector = new projector();
        public static projector SubProjector = new projector();
        public static projector SubProjector2 = new projector();

        public static projector[] InvocationList = new projector[] { Projector, SubProjector, SubProjector2 };

        public static void Update()
        {
            for (int i = 0; i < InvocationList.Length; i++)
                InvocationList[i].Update();
        }

        public static void Draw()
        {
            for (int i = 0; i < InvocationList.Length; i++)
                InvocationList[i].Draw();
        }
    }
    public class projector
    {
        private event Action UpdateSceneEvent = () => { };
        private event Action DrawSceneEvent = () => { };
        private event Action PausedSceneEvent = () => { };

        public Camera2D MainCamera;

        public void Update()
        {
            UpdateSceneEvent?.Invoke();

        }

        public void Draw()
        {
            DrawSceneEvent?.Invoke();

        }

        public void Clear()
        {
            UpdateSceneEvent = () => { };
            DrawSceneEvent = () => { };
            PausedSceneEvent = () => { };
            //MusicBox.StopSong(); //Debug
        }

        public bool Loaded(Scene s) // 특정 씬이 로드됐는지를 확인합니다. 정확히는 특정 씬이 드로우되고 있으면 로드되어있다고 판단합니다. 업데이트는 Pause에 의해 멈출 수 있지만 드로우는 로드되어있으면 멈추지 않기 떄문이죠.
        {
            return DrawSceneEvent.GetInvocationList().Contains(s.drawAction);
        }

        public void Load(params Scene[] scs) //특정 씬을 로드합니다. 같은 씬을 중복해서 로드하더라도 단 한번만 등록됩니다.
        {
            foreach (Scene s in scs)
            {
                if (!Loaded(s))
                {
                    UpdateSceneEvent += s.updateAction;
                    DrawSceneEvent += s.drawAction;
                    s.InitAction?.Invoke();
                    if (s.bgm != "")
                    {
                        MusicBox.PlaySong(s.bgm);
                    }
                }
            }
        }

        public void Unload(params Scene[] scs) //특정 씬을 제거합니다.
        {
            foreach (Scene s in scs)
            {
                UpdateSceneEvent -= s.updateAction;
                DrawSceneEvent -= s.drawAction;
                PausedSceneEvent -= s.updateAction;
                //MusicBox.StopSong(); //Debug
            }
        }

        public void Pause(params Scene[] scs)
        {
            foreach (Scene s in scs)
            {
                UpdateSceneEvent -= s.updateAction;
                PausedSceneEvent += s.updateAction;
            }
        }
        public void Resume(params Scene[] scs)
        {
            foreach (Scene s in scs)
            {
                PausedSceneEvent -= s.updateAction;
                UpdateSceneEvent += s.updateAction;
            }
        }

        public void PauseAll()
        {
            PausedSceneEvent = UpdateSceneEvent;
            UpdateSceneEvent = null;
        }

        public void ResumeAll()
        {
            UpdateSceneEvent += PausedSceneEvent;
            PausedSceneEvent = null;
        }


        public void SwapTo(params Scene[] scs)
        {
            Clear();
            Load(scs);
        }

      


    }

    public class Scene : HInitOnce
    {
        public Camera2D Camera = new Camera2D();
        public string bgm = "";
        public Action updateAction=()=> { };
        public Action drawAction = () => { };
        public Action InitAction = () => { };

        public Scene() { }

        public Scene(Action UEvent, Action DEvent) => SetEvent(UEvent, DEvent);
        public Scene(Action IEv, Action UEvent, Action DEvent)
        {
            SetEvent(UEvent, DEvent);
            InitAction = IEv;
        }


        public Scene(Camera2D SceneCam, Action UEvent, Action DEvent)
        {
            Camera = SceneCam;
            SetEvent(UEvent, DEvent);
        }
        public Scene(Camera2D SceneCam, Action IEv, Action UEvent, Action DEvent)
        {
            Camera = SceneCam;
            InitAction = IEv;
            SetEvent(UEvent, DEvent);
        }



        public void SetEvent(Action UEvent, Action DEvent)
        {
            ClearEvent();
            updateAction = UEvent;
            drawAction = () =>
            {
                Game1.Painter.OpenCanvas(Camera,
                    () =>
                    {
                        DEvent();
                    });
            };
        }

        public void ClearEvent()
        {
            updateAction = null;
            drawAction = null;
        }



    }


    public static class Matrix2D
    {

        public static Matrix Translate(REMOPoint p)
        {
            return Matrix.CreateTranslation(p.X, p.Y, 0);
        }
        public static Matrix Zoom(REMOPoint Origin, float Zoom)//Origin을 중심으로 줌을 한다. 기본값은 1f
        {
            Matrix translateToOrigin = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0);
            Matrix zoomMatrix = Matrix.CreateScale(Zoom);
            Matrix translateBackToPosition = Matrix.CreateTranslation(Origin.X, Origin.Y, 0);

            Matrix compositeMatrix = translateToOrigin * zoomMatrix * translateBackToPosition;

            return compositeMatrix;
        }

        public static Matrix Rotate(REMOPoint Origin, float rotate)//Origin을 중심으로 회전한다. 기본값은 0f. 단위는 radian.
        {
            Matrix translateToOrigin = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0);
            Matrix rotationMatrix = Matrix.CreateRotationZ(rotate);
            Matrix translateBackToPosition = Matrix.CreateTranslation(Origin.X, Origin.Y, 0);

            Matrix compositeMatrix = translateToOrigin * rotationMatrix * translateBackToPosition;

            return compositeMatrix;
        }

        public static Matrix Mat2D(REMOPoint Origin, float rotate, float Zoom)//Origin을 중심으로 회전한 후 Zoom합니다.
        {
            Matrix translateToOrigin = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0);
            Matrix zoomMatrix = Matrix.CreateScale(Zoom);
            Matrix rotationMatrix = Matrix.CreateRotationZ(rotate);
            Matrix translateBackToPosition = Matrix.CreateTranslation(Origin.X, Origin.Y, 0);

            Matrix compositeMatrix = translateToOrigin * rotationMatrix * zoomMatrix * translateBackToPosition;

            return compositeMatrix;
        }
    }


    public static class Fader // Fade 관련 애니메이션 처리
    {
        //Flicker : 깜빡깜빡거리는 액션 처리할때 쓰는 인자(Coefficient)
        public static float Flicker(int Timer, int FlickTime)
        {
            return (Math.Max(Timer % FlickTime, FlickTime - Timer % FlickTime) - FlickTime / 2) / (float)(FlickTime / 2);
        }
        public static float Flicker(int FlickTime)
        {
            return Flicker(StandAlone.FrameTimer, FlickTime);
        }

        public static Dictionary<Color, Dictionary<Gfx, int>> FadeAnimations = new Dictionary<Color, Dictionary<Gfx, int>>();
        public static Dictionary<Color, int> FadeAnimationTimers = new Dictionary<Color, int>();
        public static List<Color> ExcludedColorSets = new List<Color>();



        public static void Add(Gfx g, int Timer, Color c)
        {
            if (FadeAnimations.ContainsKey(c))
            {
                FadeAnimations[c].Add(g, Timer);
            }
            else
            {
                FadeAnimations.Add(c, new Dictionary<Gfx, int>());
                FadeAnimations[c].Add(g, Timer);
                FadeAnimationTimers.Add(c, Timer);
            }
        }

        public static void Exclude(Color c)
        {
            if (!ExcludedColorSets.Contains(c))
                ExcludedColorSets.Add(c);
        }

        public static void Clear(Color c)
        {
            if (FadeAnimations.ContainsKey(c))
            {
                FadeAnimations[c].Clear();
            }
        }

        public static void Update()
        {
            foreach (Color c in FadeAnimations.Keys.ToList())
            {
                foreach (Gfx g in FadeAnimations[c].Keys.ToList())
                {
                    if (FadeAnimations[c][g] == 0)
                    {
                        FadeAnimations[c].Remove(g);
                    }
                    else
                        FadeAnimations[c][g]--;
                }
            }
        }

        public static void Draw(Color c)
        {
            if (FadeAnimations.ContainsKey(c))
            {
                foreach (Gfx g in FadeAnimations[c].Keys)
                {
                    g.Draw(c * ((float)FadeAnimations[c][g] / FadeAnimationTimers[c]));
                }
            }
        }

        public static void DrawAll()
        {
            foreach (Color c in FadeAnimations.Keys)
            {
                if (!ExcludedColorSets.Contains(c))
                    Draw(c);
            }
        }

    }

    public static class Filter
    {
        private static Gfx2D Lighter;
        public static void Absolute(Rectangle Bound, Color c)
        {
            Lighter = new Gfx2D("WhiteSpace", Bound);
            Lighter.Draw(c);
        }
        public static void Absolute(IBoundable g, Color c) => Absolute(g.Bound, c);
        public static void Vignette(Rectangle Bound, float opacity)
        {
            Lighter = new Gfx2D("Light", Bound);
            Lighter.Draw(Color.White * opacity);
        }
        public static void Vignette(IBoundable g, float opacity) => Vignette(g.Bound, opacity);
    }

    /// <summary>
    /// The Extended Point class that is recommended to use. It's fully compatible with existing classes(Point,Vector2)
    /// </summary>
    public class REMOPoint
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }
        public REMOPoint(float x, float y)
        {
            X = (int)x;
            Y = (int)y;
        }
        public REMOPoint(REMOPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        public static REMOPoint Zero
        {
            get { return new REMOPoint(0, 0); }        
        }

        /// <summary>
        /// Returns Absolute value.
        /// </summary>
        public float Abs
        {
            get 
            {
                Vector2 v = this;
                return v.Length();
            }
        }


        //Binding to Point
        public static implicit operator Point(REMOPoint rmp)
        {
            return new Point(rmp.X,rmp.Y);
        }
        public static implicit operator REMOPoint(Point p)
        {
            return new REMOPoint(p.X, p.Y);
        }
        public Point ToPoint()
        {
            return this;
        }

        //Binding to Vector2

        public static implicit operator Vector2(REMOPoint rmp)
        {
            return new Vector2(rmp.X,rmp.Y);
        }
        public static implicit operator REMOPoint(Vector2 v)
        {
            return new REMOPoint(v.X, v.Y);
        }
        public Vector2 ToVector2()
        {
            return this;
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

        public static REMOPoint operator +(REMOPoint p1, Point p2)
        {
            REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
            return ret;
        }
        public static REMOPoint operator +(Point p1, REMOPoint p2)
        {
            REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
            return ret;
        }

        public static REMOPoint operator +(REMOPoint p1, Vector2 p2)
        {
            REMOPoint ret = new REMOPoint(p1.X + p2.X, p1.Y + p2.Y);
            return ret;
        }
        public static REMOPoint operator +(Vector2 p1, REMOPoint p2)
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
            REMOPoint ret = new REMOPoint(p1.X - p2.X, p1.Y - p2.Y);
            return ret;
        }
        public static REMOPoint operator -(Point p1, REMOPoint p2)
        {
            REMOPoint ret = new REMOPoint(p1.X - p2.X, p1.Y - p2.Y);
            return ret;
        }

        public static REMOPoint operator -(Vector2 p1, REMOPoint p2)
        {
            REMOPoint ret = new REMOPoint(p1.X - p2.X, p1.Y - p2.Y);
            return ret;
        }

        public static REMOPoint operator -(REMOPoint p1, REMOPoint p2)
        {
            REMOPoint ret = new REMOPoint(p1.X - p2.X, p1.Y - p2.Y);
            return ret;
        }


    }




}
