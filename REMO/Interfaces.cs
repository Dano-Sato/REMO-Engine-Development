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

namespace REMOEngine
{
    public interface IMovable
    {
        REMOPoint Pos { get; set; }
        void MoveTo(REMOPoint p);//(x,y)로 옮긴다.
        void MoveTo(REMOPoint p, double speed); // (x,y)를 향해 등속운동.
        void MoveByVector(REMOPoint v, double speed); // 벡터 v의 방향으로 speed의 속도로 등속운동한다.
    }


    public abstract class Movable //물체의 위치와 위치를 이동시키는 함수를 포함하는 추상클래스입니다.
    {
        public abstract REMOPoint Pos { get; set; }
        public void MoveTo(int x, int y, float speed)// (x,y)를 향해 등속운동.
        {
            double N = (new REMOPoint(x, y)-Pos).Abs;//Distance between to point.
            if (N < speed)//거리가 스피드보다 가까우면 도착.
            {
                Pos = new REMOPoint(x, y);
                return;
            }

            Vector2 v = new REMOPoint(x,y)-Pos;
            MoveByVector(v, speed);
        }
        public void MoveByVector(REMOPoint v, float speed)// 벡터 v의 방향으로 speed의 속도로 등속운동한다.
        {            
            Pos += v * (speed / v.Abs);
        }
    }

    public interface IDrawable
    {
        void Draw();
        void RegisterDrawAct(Action a);
    }

    public interface IBoundable
    {
        Rectangle Bound { get; }
    }

    public interface IInitiable//이 클래스를 상속받은 객체는 단 한번 InitOnce 함수를 통해 액션을 호출할 기회를 얻습니다. 그 이후 InitOnce는 작동하지 않습니다. 주로 커스터마이즈된 이니셜라이즈 함수를 구현할 때 쓰게 됩니다.실제 구현은 REMOC을 참조하십시오.
    {
        void InitOnce(Action a);
    }

    //주로 씬의 Init Block 안에서 이것을 활용합니다. 기본적으로 씬의 init block은 씬을 load할 때마다 발생하는데, 그중 단 한번만 init하고 싶은 경우가 있습니다.
    public abstract class HInitOnce : IInitiable //IInitiable을 구현하는 추상 클래스입니다. 씬에서 이것을 유용하게 활용할 수 있습니다.
    {
        //InitOnce 구현.
        private bool isInited = false;
        public void InitOnce(Action a)
        {
            if (!isInited)
            {
                a();
                isInited = true;
            }

        }
    }

    public interface IClickable : IBoundable
    {
        void ClickAct();
        void RegisterClickAct(Action a);
        bool ContainsCursor();
        bool CursorClickedThis();
        bool Contains(REMOPoint p);
    }


  
}
