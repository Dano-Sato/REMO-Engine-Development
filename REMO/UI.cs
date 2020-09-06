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
using REMOEngine;
using System.Security.Cryptography.X509Certificates;

namespace REMOEngine
{


    public enum MouseButtons { LeftMouseButton, RightMouseButton, MouseScrollButton } //마우스가 가진 버튼들에 대한 묶음입니다.


    /// <summary>
    ///  유저 인터랙션을 담당하는 클래스입니다.
    /// </summary>
    public static class User
    {
        private static KeyboardState OldKeyboardState = Keyboard.GetState();
        private static MouseState OldMouseState = Mouse.GetState();


        /// <summary>
        /// 유저가 키 k를 누르고 있는지를 판별하는 함수입니다. 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool Pressing(Keys k)
        {
            return Keyboard.GetState().IsKeyDown(k);
        }

        public static bool Pressing(params Keys[] ks)
        {
            bool res = true;
            foreach (Keys k in ks)
            {
                res &= Pressing(k);
            }
            return res;
        }

        public static bool Pressing(MouseButtons b)
        {
            switch (b)
            {
                case MouseButtons.LeftMouseButton:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
                case MouseButtons.MouseScrollButton:
                    return Mouse.GetState().MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightMouseButton:
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
            }
            return false;

        }

        public static bool JustPressed(Keys k)
        {
            return Pressing(k) && OldKeyboardState.IsKeyUp(k);
        }
        public static bool JustPressed(params Keys[] keys)
        {
            bool result = true;
            for (int i = 0; i < keys.Length - 1; i++)
            {
                result &= Pressing(keys[i]);
            }
            result &= JustPressed(keys[keys.Length - 1]);

            return result;

        }
        public static bool JustPressed(MouseButtons b)
        {
            switch (b)
            {
                case MouseButtons.LeftMouseButton:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed && OldMouseState.LeftButton != ButtonState.Pressed;
                case MouseButtons.MouseScrollButton:
                    return Mouse.GetState().MiddleButton == ButtonState.Pressed && OldMouseState.MiddleButton != ButtonState.Pressed;
                case MouseButtons.RightMouseButton:
                    return Mouse.GetState().RightButton == ButtonState.Pressed && OldMouseState.RightButton != ButtonState.Pressed;
            }
            return false;
        }
        public static bool JustLeftClicked() => JustPressed(MouseButtons.LeftMouseButton);
        public static bool JustLeftClicked(IBoundable g)
        {
            return JustLeftClicked() && g.Bound.Contains(Cursor.Pos.ToPoint());
        }
        public static bool JustLeftClicked(Rectangle g)
        {
            return JustLeftClicked() && g.Contains(Cursor.Pos.ToPoint());
        }


        public static bool JustRightClicked() => JustPressed(MouseButtons.RightMouseButton);

        public static void KeyJAct(Keys k, Action a) //본 함수를 Update()에 넣을 경우, User가 key를 막 눌렀을때 action을 일으킵니다. J는 JustPressed의 약자입니다.
        {
            if (JustPressed(k))
                a();
        }
        public static void KeyPAct(Keys k, Action a) //본 함수를 Update()에 넣을 경우, User가 key를 누르고 있을때 action을 일으킵니다. P는 Pressing의 약자입니다.
        {
            if (Pressing(k))
                a();
        }




        public static Keys[] ArrowKeys = new Keys[] { Keys.Up, Keys.Left, Keys.Down, Keys.Right, }; //방향키를 할당하는 배열입니다. 방향키 세팅을 바꾸고 싶으면 이 배열을 바꾸면 됩니다.

        public static void ArrowKeyPAct(Action<REMOPoint> VectorAction)//본 함수를 Update()에 넣을 경우, User가 Arrow Key를 누르는 경우에 대해 해당되는 방향벡터 액션을 일으킵니다.
        {
            KeyPAct(ArrowKeys[0], () => VectorAction(new REMOPoint(0, -1)));
            KeyPAct(ArrowKeys[1], () => VectorAction(new REMOPoint(-1, 0)));
            KeyPAct(ArrowKeys[2], () => VectorAction(new REMOPoint(0, 1)));
            KeyPAct(ArrowKeys[3], () => VectorAction(new REMOPoint(1, 0)));
        }
        public static void ArrowKeyJAct(Action<REMOPoint> VectorAction)//본 함수를 Update()에 넣을 경우, User가 Arrow Key를 누르는 경우에 대해 해당되는 방향벡터 액션을 일으킵니다.
        {
            KeyJAct(ArrowKeys[0], () => VectorAction(new REMOPoint(0, -1)));
            KeyJAct(ArrowKeys[1], () => VectorAction(new REMOPoint(-1, 0)));
            KeyJAct(ArrowKeys[2], () => VectorAction(new REMOPoint(0, 1)));
            KeyJAct(ArrowKeys[3], () => VectorAction(new REMOPoint(1, 0)));
        }




        public static void Update()
        {
            Cursor.Update();
            OldKeyboardState = Keyboard.GetState();
            OldMouseState = Mouse.GetState();
        }



    }
    public static class Cursor
    {
        public static Gfx2D Graphic = new Gfx2D("Cursor", new Rectangle(0, 0, 20, 20));
        public static Gfx DraggedTarget; // 현재 드래깅되고 있는 객체를 추적하는 Gfx 포인터입니다.

        public static REMOPoint Size
        {
            get { return Graphic.Bound.Size; }
            set { Graphic.Bound = new Rectangle(Pos, value); }

        }
        public static REMOPoint Pos
        {
            get
            {
                return Graphic.Pos;
            }
        }
        public static void Update()
        {
            Graphic.Pos = Mouse.GetState().Position;
        }

        public static void Draw(Color c)
        {
            Graphic.Draw(c);
        }

        public static bool JustLeftClicked(Gfx g)
        {
            return g.ContainsCursor() && User.JustLeftClicked();
        }
        public static bool IsDragging(Gfx g)
        {
            if (JustLeftClicked(g))
            {
                DraggedTarget = g;
            }

            if (DraggedTarget == g && !User.Pressing(MouseButtons.LeftMouseButton))
            {
                DraggedTarget = new Gfx2D();

            }
            if (DraggedTarget == g)
                return true;
            else
                return false;
        }

    }


    

    public class VolumeBar
    {

        public Gfx2D Line;
        public Gfx2D Wheel;
        public int Interval;
        private float coefficient = 0f;
        public float Coefficient
        {
            get { return coefficient; }
            set
            {
                if (value != coefficient)
                    ScrollAction(value);
                coefficient = value;
            }
        }

        public Action<float> ScrollAction;
        public VolumeBar(Gfx2D line, string WheelSpriteName, int BarSize, Action<float> scrollAction)
        {
            Line = line;
            Interval = Line.Bound.Width - BarSize;
            ScrollAction = scrollAction;
            Wheel = new Gfx2D(WheelSpriteName, new Rectangle(new REMOPoint(Line.Pos.X + Interval, Line.Pos.Y), new REMOPoint(BarSize, Line.Bound.Height)));
        }
        public void Enable()
        {
            if (Cursor.IsDragging(Wheel) || (User.JustLeftClicked() && Line.ContainsCursor()))
            {
                Wheel.Center = new REMOPoint(Cursor.Pos.X, Wheel.Pos.Y);
            }

            //바가 범위를 벗어나지 않도록 조정.

            if (Wheel.Pos.X < Line.Pos.X)
                Wheel.Pos = Line.Pos;
            if (Wheel.Pos.X > Line.Pos.X + Interval)
                Wheel.Pos = new REMOPoint(Line.Pos.X + Interval, Wheel.Pos.Y);
            Wheel.Pos = new REMOPoint(Wheel.Pos.X, Line.Pos.Y);


            //계수조정.
            Coefficient = (Wheel.Pos.X - Line.Pos.X) / (float)Interval;
        }

        public void Draw()
        {
            Line.Draw();
            Wheel.Draw();
        }

        public void Draw(Color c1, Color c2)
        {
            Line.Draw(c1);
            Wheel.Draw(c2);
        }


        public float MapCoefficient(float From, float To)
        {
            return Coefficient * (To - From) + From;
        }
        public void Initialize(float initCoefficient)
        {
            Coefficient = initCoefficient;
            Wheel.Pos = new REMOPoint((int)((1.0f - Coefficient) * Interval + Line.Pos.X), 0);
        }
    }


    public class Button : IMovable, IDrawable, IBoundable // 그래픽스에 클릭액션이 달린 구조. Clickable 인터페이스를 만들고 폐기할 예정입니다.
    {
        public Gfx ButtonGraphic;
        public Action ButtonClickAction;

        public REMOPoint Pos { get { return ButtonGraphic.Pos; }
            set { ButtonGraphic.Pos = value; } }

        public Rectangle Bound
        {
            get { return ButtonGraphic.Bound; }
            set { ButtonGraphic.Bound = value; }
        }

        public Button(Gfx g, Action a)
        {
            ButtonGraphic = g;
            ButtonClickAction = a;
        }

        public void Enable()
        {
            if (User.JustLeftClicked(ButtonGraphic))
            {
                ButtonClickAction();
            }
        }

        public void Draw() => ButtonGraphic.Draw();
        public void Draw(Color c) => ButtonGraphic.Draw(c);

        public void RegisterDrawAct(Action a) => ButtonGraphic.RegisterDrawAct(a);

        public void MoveTo(REMOPoint p) => ButtonGraphic.MoveTo(p);
        public void MoveTo(REMOPoint p, double speed) => ButtonGraphic.MoveTo(p, speed);
        public void MoveByVector(REMOPoint p, double v) => ButtonGraphic.MoveByVector(p, v);
    }

    public class SimpleMenu
    {
        public Aligned<Button> Menus;
        public Color AccentColor = Color.Red;
        public SimpleMenu(int fontsize, REMOPoint pos, REMOPoint interval, string[] MenuNames, params Action[] MenuActions)
        {
            Menus = new Aligned<Button>(pos, interval);
            for(int i=0;i<MenuNames.Length; i++)
            {
                Button menu = new Button(new GfxStr(fontsize, MenuNames[i]), MenuActions[i]);
                Menus.Add(menu);
            }
            Menus.Align();
        }
        
        public void Update()
        {
            Menus.Align();
            foreach(Button menu in Menus.Components)
            {
                menu.Enable();
            }
        }
        public void Draw(params Color[] cs)
        {
            foreach(Button menu in Menus.Components)
            {
                if(!menu.Bound.Contains(Cursor.Pos.ToPoint()))
                {
                    foreach (Color c in cs)
                    {
                        menu.Draw(c);
                    }
                }
                else
                {
                    menu.Draw(AccentColor);
                }
            }
        }
    }

    public class SimpleGauge 
    {
        public Gfx2D Graphic;
        public Rectangle MaxBound;

        public SimpleGauge(Gfx2D gaugeGraphic)
        {
            Graphic = gaugeGraphic;
            MaxBound = Graphic.Bound;

        }
        public void Update(float coefficient, float coeefficient_Max)
        {
            Graphic.Bound = new Rectangle(Graphic.Pos, new REMOPoint((coefficient * MaxBound.Width) / coeefficient_Max, MaxBound.Height));
        }

        public void Draw(Color c)
        {
            Graphic.Draw(c);
        }

    }

}
