using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using REMO_Engine_Developer;


namespace REMO_Engine_Developer
{
    public class Aligned<T> : IMovable, IDrawable, IBoundable  where T : IMovable, IDrawable, IBoundable  // 일렬로 정렬된 객체들을 포함하는 콜렉션입니다.
    {
        public List<T> Components = new List<T>();
        public Point Pos { get; set; }//집합을 정렬할 기준점입니다. 
        public Point Interval;//각 컴포넌트간 간격을 설정하는 벡터입니다.

        public Rectangle Bound { get {
                Rectangle temp = Rectangle.Empty;
                for (int i = 0; i < Count; i++)
                    Rectangle.Union(temp, this[i].Bound);
                return temp;
            } } // 컬렉션의 영역은 각 컴포넌트를 모두 포함하는 최소의 사각형입니다. O(n)스케일.


        public void MoveTo(Point p)
        {
            Pos = p;
            Align();
        }

        public void MoveTo(Point p, double speed)//기준점 p에 맞춰 컴포넌트들이 줄을 섭니다.
        {
            Pos = p;
            LazyAlign(speed);
        }

        public void MoveByVector(Point p, double speed)
        {
            double N = Method2D.Distance(new Point(0, 0), p);
            int Dis_X = (int)(p.X * speed / N);
            int Dis_Y = (int)(p.Y * speed / N);
            Pos = new Point(Pos.X + Dis_X, Pos.Y + Dis_Y);

            Align();
        }


        public T this[int i]
        {
            get => Components[i];
            set => value = Components[i];
        }

        public int Count
        {
            get => Components.Count;
        }

        public void RemoveAt(int i) => Components.RemoveAt(i);
        public int IndexOf(T t) => Components.IndexOf(t);
        public void Clear() => Components.Clear();

        public bool MouseIsOnComponents //마우스가 컴포넌트들 위에 있는지를 체크합니다.
        {
            get => OneContainsCursor() != null;
        }
        public int ClickedIndex//방금 클릭된 컴포넌트의 인덱스를 반환합니다. 컴포넌트가 클릭되지 않았을 경우 -1, 혹은 컴포넌트의 인덱스(0부터 시작)를 반환합니다.
        {
            get => IndexOf(OneJustClicked());
        }



        public Aligned(Point pos, Point interval)//상대벡터와 간격벡터를 설정합니다. 이후 Origin을 조정하여 특정 컴포넌트에 매달 수 있습니다.
        {
            Pos = pos;
            Interval = interval;
        }

        public Aligned(Point pos, Point interval, List<T> TList)
        {
            Pos = pos;
            Interval = interval;
            Components = TList;
        }

        public void Add(T t)
        {
            Components.Add(t);
        }


        public void Align() //컴포넌트들이 즉시 정렬됩니다.
        {
            Point temp = Pos;
            for (int i = 0; i < Count; i++)
            {
                this[i].MoveTo(temp);
                temp += Interval;
            }
        }

        public void LazyAlign(double AlignSpeed) // 컴포넌트들이 정해진 스피드에 맞춰 느긋하게 정렬됩니다.
        {
            Point temp = Pos;
            for (int i = 0; i < Count; i++)
            {
                this[i].MoveTo(temp, AlignSpeed);
                temp += Interval;
            }

        }




        public T Pick(Func<T, bool> FilterCondition) // 특정 Condition을 만족하는 객체를 뽑아냅니다. O(n)이므로 유의하여 사용하는 것이 좋습니다.
        {
            for (int i = 0; i < Count; i++)
            {
                if (FilterCondition(this[i]))
                    return this[i];
            }
            return default(T);
        }

        public T OneContains(Point p)
        {
            return Pick((t) =>
            {
                return t.Bound.Contains(p);
            });
        }

        public T OneContainsCursor() => OneContains(Cursor.Pos); // 커서가 놓여있는 객체를 뽑아냅니다. O(n)이므로 유의하여 사용하는 것이 좋습니다. 반환값이 null일 수 있으므로 . 대신 ?. operator를 씁시다.
        public T OneJustClicked() // 막 클릭된 객체를 뽑아냅니다. O(n)이므로 유의하여 사용하는 것이 좋습니다. 반환값이 null일 수 있으므로 . 대신 ?. operator를 씁시다.
        {
            return Pick((t) =>
            {
                return t.Bound.Contains(Cursor.Pos) && User.JustLeftClicked();
            });
        }



        private Action DrawAction;
        public void RegisterDrawAct(Action a)
        {
            DrawAction = a;
        }
        public void Draw()
        {
            if (DrawAction == null)
            {
                for (int i = 0; i < Count; i++)
                    this[i].Draw();
            }
            else
                DrawAction();
        }

    }

}

