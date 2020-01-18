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

    public class Stats
    {
        public double Attack = 0;//공격력
        public double Defense = 0;//방어력
        public double HP = 0;//체력
          
        public Stats() : this(0, 0, 0) { }

        public Stats(double a, double d, double h)
        {
            Attack = a;
            Defense = d;
            HP = h;
        }
    }

    public enum UnitClass { Merchant, Hunter, Blacksmith}
    public class UnitBase
    {
        public Stats BaseStats = new Stats();
        public Stats BonusStats = new Stats();
        public List<Buff> Buffs = new List<Buff>();
        public UnitClass Class;
        public int Number;
    }

    public class VanillaUnitBase : UnitBase //가령 Normal Hunter, Jack 등 튜닝을 거치기 전 순정 상태의 카드 스테이터스를 담습니다. 
    {
        public VanillaUnitBase(Stats Base) //보너스 없이 기본 스탯만 있는 유닛을 생성합니다. 노멀 유닛들이 될 것입니다.
        {
            BaseStats = Base;
        }
        public VanillaUnitBase(Stats Base, Stats Bonus) //보너스 스탯이 추가된 유닛을 생성합니다. 레어 유닛들이 될 것입니다.
        {
            BaseStats = Base;
            BonusStats = Bonus;
        }

    }

    public static class VanillaUnitTable//순정 유닛 카드들을 담고 있는 테이블입니다.
    {
        public static VanillaUnitBase NormalUnit
        {
            get { return new VanillaUnitBase(new Stats(1, 0, 1)); }
        }
        public static VanillaUnitBase NormalHeadUnit
        {
            get { return new VanillaUnitBase(new Stats(1.2,0, 1)); }
        }

    }

    public class Tuner
    {

    }

    public class TunedUnitBase : UnitBase//VanillaUnit이 튜너를 거쳐 튜닝이 된 상태입니다. 실제 유닛 생성의 베이스가 됩니다.
    {
        public TunedUnitBase(VanillaUnitBase v, Tuner t)//바닐라 베이스에 튜너를 결합하여 튜닝된 베이스를 만듭니다. 패산을 형성할때, 패산은 튜닝된 베이스의 산이 됩니다.
        {

        }
    }


    public enum BuffState {Stat, Attack, Speed, Range };


    public class Buff
    { 
        //Buff Tag : X(Stat Buff), A(Attack Buff), S(Speed Buff), R(Range Buff)
        //각종 액션의 순간에 유닛의 버프리스트를 읽어들여 버프의 기능들을 수행합니다.    
    }


    public static class BuffTable
    {
        public static Func<Stats,Stats> Multiply(double r) //유닛 스탯에 r배 계수 뻥튀기를 먹이는 함수를 만든다.
        {
            return (s) => { return new Stats(s.Attack * r, s.Defense * r, s.HP * r); };
        }


    }

    //카드 계열. 카드 인터페이스를 계승받을 예정입니다.

    public class Unit //필드 위에 존재하는 유닛 카드입니다.
    {
        public UnitBase Base; //본인의 넘버링과 베이스를 간직하고 있습니다.
        public Stats CurrentStat; //베이스와 별개로 현재 스탯이 있습니다.
        public void ProcessStatBuff()
        {

        }
    
    
    }

    public class Weapon//필드 위에 존재하는 무기 카드입니다.
    {
        public Stats BonusStats;//이 무기를 장착하는 유닛에게 줄 보너스 수치값입니다.
        public List<Buff> Buffs;//이 무기를 장착하는 유닛에게 줄 버프들입니다.

    }


    public interface ICard
    {

    }
    //Template for new Scene. 나중에 커스텀 항목 템플릿을 통해 Scene을 만들 수 있게 계획하겠음.
    public static class YourScene
    {
        /*Space for Scene properties*/



        //Write your own Update&Draw Action in here        
        public static Scene scn = new Scene(
            () =>
            {

            },
            () =>
            {

            }
            );

    }
}
