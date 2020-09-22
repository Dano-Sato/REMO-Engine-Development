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
using System.Security.Cryptography;
using REMOEngine;
using System.Runtime.ExceptionServices;
using Steamworks;
using System.Security.Policy;
using System.Diagnostics.Tracing;
using Yokai;

namespace MineCrazy
{


    public static class Pet
    {
        public static Gfx2D PetGraphic=new Gfx2D(new Rectangle(500,150,100,100));
        public static Tuple<string, double> OptionSlot = new Tuple<string, double>("", 0);

        public static string[] PetCodes = new string[] { "BS", "YS", "RS", "GS", "US" };
        public static Dictionary<string, string> PetNames = new Dictionary<string, string>();

        private static string petCode="";
        public static string PetCode
        {
            get { return petCode; }
            set 
            {
                petCode = value;
                if (PetCode == "US")
                    PetGraphic.Sprite = "Snail";
                else
                    PetGraphic.Sprite = "PetSlime";
             
                switch (PetCode)
                {
                    case "US":
                        OptionSlot = new Tuple<string, double>("", 0);
                        break;
                    case "GS":
                        OptionSlot = new Tuple<string, double>("UB", 0.5);
                        break;
                    case "YS":
                        OptionSlot = new Tuple<string, double>("CC", 0.5);
                        break;
                    case "BS":
                        OptionSlot = new Tuple<string, double>("CD", 3.0);
                        break;
                    case "RS":
                        OptionSlot = new Tuple<string, double>("A", 1.7);
                        break;
                }

                if(PetCode=="")
                {
                    PetGraphic.Sprite = "WhiteSpace";
                    OptionSlot = new Tuple<string, double>("", 0);
                }
            }
        }
        public static void GetPet()
        {
            //get random pet
            double r = StandAlone.Random();
            if (r < 0.4)
            {
                PetCode = "US";
            }
            else if (r < 0.55)
                PetCode = "BS";
            else if(r<0.7)
                PetCode = "YS";
            else if (r < 0.85)
                PetCode = "RS";
            else
                PetCode = "GS";


        


        }

        public static void Init()
        {
            PetNames.Add("BS", "Blue Slime");
            PetNames.Add("YS", "Yellow Slime");
            PetNames.Add("RS", "Red Slime");
            PetNames.Add("GS", "Green Slime");
            PetNames.Add("US", "Useless Snail");
            PetGraphic.Sprite = "PetSlime";

        }

        public static void Update()
        {
            if(PetCode!="" && User.JustLeftClicked(PetGraphic))
            {
                Projectors.Projector.PauseAll();
                Projectors.Projector.Load(scn);
            }
            if(LandFillScene.EggTimer>0&&User.JustLeftClicked(PetGraphic))
            {
                MusicBox.PlaySE("Mining",0.15f);
                LandFillScene.EggTimer = Math.Max(10, LandFillScene.EggTimer - 100);
            }
        }

        public static void Draw()
        {
            switch(PetCode)
            {
                case "BS":
                    PetGraphic.Draw(Color.LightBlue,Color.Blue*0.4f);
                    break;
                case "YS":
                    PetGraphic.Draw(Color.Yellow);
                    break;
                case "RS":
                    PetGraphic.Draw(Color.IndianRed);
                    break;
                case "GS":
                    PetGraphic.Draw(Color.LightSeaGreen);
                    break;
                case "US":
                    PetGraphic.Draw();
                    break;
            }
            if(LandFillScene.EggTimer>0)
            {
                PetGraphic.Draw();
            }
        }

        public static Gfx2D board = new Gfx2D("MINE.Board2", new Rectangle(650, 150, 300, 250));
        public static Button Discard = new Button(new GfxStr("Discard", new REMOPoint(760, 340)), () => {
            Pet.PetCode = "";        
        
        });

        public static Scene scn = new Scene(() =>
        {

        }, () =>
        {
            if(User.JustLeftClicked())
            {
                Projectors.Projector.Unload(scn);
                Projectors.Projector.ResumeAll();
            }
            Discard.Enable();
        }, () =>
        {
            board.Draw();
            switch(PetCode)
            {
                case "US":
                    StandAlone.DrawString("Useless Snail", new REMOPoint(750, 200), Color.Black);
                    StandAlone.DrawString("Useless", new REMOPoint(770, 250), Color.Black);
                    break;
                case "RS":
                    StandAlone.DrawString("Red Slime", new REMOPoint(750, 200), Color.Black);
                    StandAlone.DrawString("Option", new REMOPoint(770, 250), Color.Black);
                    StandAlone.DrawString("Attack Increase 70%", new REMOPoint(720, 290), Color.Black);
                    break;
                case "YS":
                    StandAlone.DrawString("Yellow Slime", new REMOPoint(750, 200), Color.Black);
                    StandAlone.DrawString("Option", new REMOPoint(770, 250), Color.Black);
                    StandAlone.DrawString("Critical Chance 50%", new REMOPoint(720, 290), Color.Black);
                    break;
                case "BS":
                    StandAlone.DrawString("Blue Slime", new REMOPoint(750, 200), Color.Black);
                    StandAlone.DrawString("Option", new REMOPoint(770, 250), Color.Black);
                    StandAlone.DrawString("Critical Damage 300%", new REMOPoint(720, 290), Color.Black);
                    break;
                case "GS":
                    StandAlone.DrawString("Green Slime", new REMOPoint(750, 200), Color.Black);
                    StandAlone.DrawString("Option", new REMOPoint(770, 250), Color.Black);
                    StandAlone.DrawString("Unbreakable 50%", new REMOPoint(720, 290), Color.Black);
                    break;




            }
            Discard.DrawWithAccent(Color.Black,Color.Red);
            if (board.ContainsCursor())
                Cursor.Draw(Color.Black);
        });

    }

    public static class Pickaxe
    {
        public static int Power = 10;
        private static int level = 1;
        public static int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                Power = level * 9 + (int)Math.Pow(1.5, level);
                TuningScene.Reinforcefee = (int)(50 * (Math.Pow(1.5, level - 1)));
                TuningScene.ReinforcePossibility = (float)(1.0 * Math.Pow(0.98, level - 1));
                TuningScene.ReinforceButton.ButtonGraphic = new GfxStr("Reinforce(R) : " + TuningScene.Reinforcefee + "G", new REMOPoint(400, 200));
            }
        }
        public static Tuple<string, double>[] Enchants = new Tuple<string, double>[] {new Tuple<string, double>("",0), new Tuple<string, double>("", 0) , new Tuple<string, double>("", 0) };
        public static void Enchant(int SlotNum)
        {
            if(TuningScene.EnchantFee==TuningScene.EnchantNormalFee)
            {
                double r = StandAlone.Random();
                if(r<0.2)
                {
                    Enchants[SlotNum] = new Tuple<string, double>("A", StandAlone.Random(0.2, 2));
                }
                else if(r<0.3)
                    Enchants[SlotNum] = new Tuple<string, double>("UB", StandAlone.Random(0, 0.8));
                else if (r<0.6)
                {
                    Enchants[SlotNum] = new Tuple<string, double>("BD", StandAlone.Random(0.0, 1));
                }
                else if(r<0.8)
                    Enchants[SlotNum] = new Tuple<string, double>("CC", StandAlone.Random(0.0, 1));
                else if(r<0.9)
                    Enchants[SlotNum] = new Tuple<string, double>("CD", StandAlone.Random(0.0, 10));
                else
                    Enchants[SlotNum] = new Tuple<string, double>("AM", StandAlone.Random(0.0, 10));


            }
        }

        public static int GetPower()
        {
            int result=Power;
            double a = 1;
            for(int i=0;i<3;i++)
            {
                if(Enchants[i].Item1=="A")
                {
                    a += (Enchants[i].Item2-1);
                }
            }
            if(Pet.OptionSlot.Item1=="A")
                a += (Pet.OptionSlot.Item2 - 1);

            result = (int)Math.Max(0, result * a);
            return result;
        }

        public static double GetBD()
        {
            double bd = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Enchants[i].Item1 == "BD")
                {
                    bd += (Enchants[i].Item2);
                }
            }
            return bd;
        }

        public static bool CheckBreak()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Enchants[i].Item1 == "UB")
                {
                    if (StandAlone.Random() < Enchants[i].Item2)
                        return false;
                }
            }

            if (Pet.OptionSlot.Item1 == "UB")
            {
                if (StandAlone.Random() < Pet.OptionSlot.Item2)
                    return false;
            }
            return true;

        }

        public static bool CheckCritical()
        {
            double cc = 0.1;
            for (int i = 0; i < 3; i++)
            {
                if (Enchants[i].Item1 == "CC")
                {
                    cc += (Enchants[i].Item2);
                }
            }
            if(Pet.OptionSlot.Item1=="CC")
                cc += (Pet.OptionSlot.Item2);

            if (StandAlone.Random() < cc)
                return true;
            else
                return false;
        }
        public static double GetCD()
        {
            double cd = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Enchants[i].Item1 == "CD")
                {
                    cd += (Enchants[i].Item2);
                }
            }
            if (Pet.OptionSlot.Item1 == "CD")
                cd += Pet.OptionSlot.Item2;
            return cd;
        }
        public static double Get(string text)
        {
            double result=0;
            for (int i = 0; i < 3; i++)
            {
                if (Enchants[i].Item1 == text)
                {
                    result += (Enchants[i].Item2);
                }
            }
            return result;
        }


    }

    public static class UserInterface
    {
        public static ulong Gold = 0;
        public static int EnchantStone = 0;
        public static int Trash = 0;
        public static Button TuningSceneButton = new Button(new GfxStr("Go To Blacksmith(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(TuningScene.scn); });
        public static Button MiningSceneButton = new Button(new GfxStr("Go To Mine(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(MiningScene.scn); });
        public static Button LandFillSceneButton = new Button(new GfxStr("Go To Landfill(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(LandFillScene.scn);
        });
        public static Button TuningSceneButton2 = new Button(new GfxStr("Go To Blacksmith(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(TuningScene.scn);
        });
        public static Button MiningSceneButton2 = new Button(new GfxStr("Go To Mine(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(MiningScene.scn);
        });
        public static Button LandFillSceneButton2 = new Button(new GfxStr("Go To Landfill(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(LandFillScene.scn);
        });



        public static Aligned<Button> CurrentButtons=new Aligned<Button>(new REMOPoint(800,100),new REMOPoint(0,40)); 


        public static void SetButtons(params Button[] buttons)
        {
            CurrentButtons.Clear();
            foreach(Button b in buttons)
            {
                CurrentButtons.Add(b);
            }
            CurrentButtons.Align();

        }
        public static void Update()
        {
            CurrentButtons.Align();
            foreach(Button b in CurrentButtons.Components)
            {
                b.Enable();
                if (User.JustLeftClicked(b))
                    break;
            }
            if (LandFillScene.EggTimer > 0)
            {
                LandFillScene.EggTimer--;
                if(LandFillScene.EggTimer==0)
                {
                    Pet.GetPet();
                }
            }
        }
        public static void Draw()
        {
            StandAlone.DrawString(20, "Gold : " + Gold+"G", new REMOPoint(400, 50), Color.Gold);
            if(Trash>0)
            {
                StandAlone.DrawString(20, "Trash : " + Trash, new REMOPoint(400, 90), Color.White);
                StandAlone.DrawString(20, "Trash : " + Trash, new REMOPoint(400, 90), Color.Gray * 0.6f);
            }
            if(EnchantStone> 0 ||Pickaxe.Enchants[0].Item1 != "" || Pickaxe.Enchants[1].Item1 != "" || Pickaxe.Enchants[2].Item1 != "")
                StandAlone.DrawString(20, "Enchant Stone : " + EnchantStone+"S", new REMOPoint(700, 50), Color.SkyBlue);


            foreach (Button b in CurrentButtons.Components)
            {
                b.DrawWithAccent(Color.White, Color.Red);
            }
            StandAlone.DrawString("", new REMOPoint(800, 50), Color.White);

        }
    }
    public static class MiningScene
    {

        public static Gfx2D PickaxeGrapic = new Gfx2D("MINE.Pickaxe", Rock.Graphic.Center + new REMOPoint(70, -100), 0.5f);
        public static class Rock
        {
            public static Gfx2D Graphic=new Gfx2D("MINE.Rock",new Rectangle(150,100,200,200));
            private static int level=1;
            public static int Level
            {
                get { return level; }
                set 
                {
                    level = value;
                    MaxHP = (int)(30 * Math.Pow(1.5, level - 1));
                    CurrentHP = MaxHP;
                    MaxDEF = (int)(Math.Pow(1.7, level - 1));
                    CurrentDEF = MaxDEF;
                
                
                }
            }
            public static int MaxHP=30;
            public static int MaxDEF=1;
            public static int CurrentHP=30;
            public static int CurrentDEF=1;

            public static SimpleGauge HPGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 370, 1000, 15)));
            public static SimpleGauge DEFGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 420, 1000, 15)));


            public static void Update()
            {
                CurrentDEF = (int)Math.Max(0, MaxDEF * (1 - Pickaxe.GetBD()));
                HPGauge.Update(CurrentHP, MaxHP);
                DEFGauge.Update(CurrentDEF, MaxDEF);
                if(User.JustLeftClicked(Rock.Graphic)||User.JustPressed(Keys.Z)||MiningTimer>5)
                {
                    MiningTimer = 0;
                    PickaxeGrapic.Rotate = (float)StandAlone.Random()*1.5f;
                    if(!Pickaxe.CheckCritical())
                        Rock.CurrentHP -= Math.Max(0, Pickaxe.GetPower() - Rock.CurrentDEF);
                    else
                        Rock.CurrentHP -= Math.Max(0, (int)(Pickaxe.GetPower()*(2+Pickaxe.GetCD()) - Rock.CurrentDEF));
                }
                if(StandAlone.FrameTimer%60==0)
                {
                    if(Pickaxe.Get("AM")>0)
                        PickaxeGrapic.Rotate = (float)StandAlone.Random() * 1.5f;
                    if (!Pickaxe.CheckCritical())
                        Rock.CurrentHP -= (int)Math.Max(0, Pickaxe.GetPower()*Pickaxe.Get("AM") - Rock.CurrentDEF);
                    else
                        Rock.CurrentHP -= Math.Max(0, (int)(Pickaxe.GetPower()*Pickaxe.Get("AM") * (2 + Pickaxe.GetCD()) - Rock.CurrentDEF));

                }

                if (Rock.CurrentHP<=0)
                {
                    MusicBox.PlaySE("Mining",0.15f);
                    Rock.CurrentHP = Rock.MaxHP;
                    GetReward();
                }
                if (User.JustPressed(Keys.S))
                    NextRockButton.ButtonClickAction();
                if (User.JustPressed(Keys.A))
                    PrevRockButton.ButtonClickAction();

            }
            public static void GetReward()
            {
                UserInterface.Gold += (ulong)(10*Math.Pow(1.5,Level-1));
                UserInterface.EnchantStone += Level / 5;
            }

            public static void Draw()
            {
                Color[] RockColors = new Color[] { Color.White, Color.Orange, Color.DarkRed, Color.Aqua, Color.LightCoral, Color.Pink,Color.Lavender, Color.PaleVioletRed,Color.BlueViolet,Color.LightGreen,Color.LightSeaGreen };
                Rock.Graphic.Draw(RockColors[Math.Min(RockColors.Length-1, Level/5)]);


                if (Rock.Graphic.ContainsCursor())
                    Rock.Graphic.Draw(Color.Red);
                StandAlone.DrawString("level "+level+" Rock", Rock.Graphic.Center - new REMOPoint(50, 20), Color.Black);
                HPGauge.Draw(Color.Red);
                StandAlone.DrawString("ROCK HP : "+Rock.CurrentHP, HPGauge.Graphic.Pos-new REMOPoint(0,30), Color.Red);
                DEFGauge.Draw(Color.Green);
                StandAlone.DrawString("ROCK DEF :"+Rock.CurrentDEF, DEFGauge.Graphic.Pos - new REMOPoint(0, 30), Color.Green);

            }
        }

        public static int MiningTimer = 0;

        public static Button PrevRockButton = new Button(new GfxStr("Prev Rock(A)", new REMOPoint(450,300)), () => 
        {
            Rock.Level = Math.Max(1, Rock.Level - 1);
        });
        public static Button NextRockButton = new Button(new GfxStr("Next Rock(S)", PrevRockButton.Pos+new REMOPoint(150,0)), () =>
        {
            Rock.Level = Math.Min(45,Rock.Level+1);

        });


        public static Scene scn = new Scene(() =>
        {
            if(LandFillScene.trigger==1)
                UserInterface.SetButtons(UserInterface.TuningSceneButton,UserInterface.LandFillSceneButton2);
            else
                UserInterface.SetButtons(UserInterface.TuningSceneButton);

            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);

        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Right))
                Projectors.Projector.SwapTo(TuningScene.scn);
            if (User.JustPressed(Keys.Left) && LandFillScene.trigger == 1)
                Projectors.Projector.SwapTo(LandFillScene.scn);

            if (User.Pressing(Keys.Z)|| User.Pressing(Keys.X)|| User.Pressing(Keys.C)|| User.Pressing(Keys.V))
                MiningTimer++;
            Rock.Update();
            PrevRockButton.Enable();
            NextRockButton.Enable();
            Pet.Update();

        }, () =>
        {
            Rock.Draw();
            PickaxeGrapic.Draw();

            if (Pickaxe.Level >= 10)
                PickaxeGrapic.Draw(Color.LightBlue);
            StandAlone.DrawString("Press Z to Mine!(Z)", new REMOPoint(450,260), Color.White);
            UserInterface.Draw();
            PrevRockButton.DrawWithAccent(Color.White, Color.Red);
            NextRockButton.DrawWithAccent(Color.White, Color.Red);

            Pet.Draw();

            Cursor.Draw(Color.White);
        });
    }

    public static class TuningScene
    {
        public static int Reinforcefee = 50;
        public static float ReinforcePossibility = 1.0f;
        public static Button ReinforceButton = new Button(new GfxStr("Reinforce(R) : " + Reinforcefee + "G", new REMOPoint(400, 200)), () =>
                 {
                     if (UserInterface.Gold >= (ulong)Reinforcefee)
                     {
                         UserInterface.Gold -= (ulong)Reinforcefee;
                         if (StandAlone.Random() < ReinforcePossibility)
                         {
                           //Reinforce
                           MusicBox.PlaySE("MINE.Smith", 0.5f);
                             Fader.Add(new GfxStr(PickaxeLevelString), 60, Color.Yellow);
                             Pickaxe.Level += 1;
                         }
                         else if (Pickaxe.CheckBreak() && (!Protected.isChecked || LandFillScene.Item1_Count == 0))
                         {
                           //Break
                           MusicBox.PlaySE("Break");
                             Fader.Add(new GfxStr(PickaxeLevelString), 60, Color.Red);
                             UserInterface.Trash += Pickaxe.Level;
                             Pickaxe.Level = Math.Max(1, LandFillScene.Item2_Count * 5);
                             Pickaxe.Enchants = new Tuple<string, double>[] { new Tuple<string, double>("", 0), new Tuple<string, double>("", 0), new Tuple<string, double>("", 0) };
                             EnchantSlot.Make();
                             if (UserInterface.Trash >= 30 && LandFillScene.trigger == -1)
                             {
                                 LandFillScene.trigger = 0;
                             }
                         }
                         else
                         {
                             MusicBox.PlaySE("NotBreakedSE");
                         }


                         if (Protected.isChecked && LandFillScene.Item1_Count > 0)
                         {
                             LandFillScene.Item1_Count--;
                             LandFillScene.Used_Item1_Count++;
                         }


                     }
                 });
        public static readonly int EnchantNormalFee = 100;
        public static int EnchantFee = EnchantNormalFee;
        public static Button EnchantButton = new Button(new GfxStr("Enchant(E)", ReinforceButton.Pos + new REMOPoint(0, 120)), () =>
         {
             if (UserInterface.EnchantStone >= EnchantSlot.GetCost())
             {
                 if (EnchantSlot.GetCost() > 0)
                     MusicBox.PlaySE("EnchantSE", 0.5f);
                 UserInterface.EnchantStone -= EnchantSlot.GetCost();

                 for (int i = 0; i < 3; i++)
                 {
                     if (EnchantSlot.SelectedSlots.HasFlag(EnchantSlot.Flags[i]))
                     {
                         Pickaxe.Enchant(i);
                         TuningScene.EnchantSlot.Make();
                         EnchantSlot.Slots.Align();
                         Fader.Add(new GfxStr(EnchantSlot.Slots[i]), 50, Color.Yellow);
                     }
                 }
                 EnchantSlot.SelectedSlots = EnchantSlot.SelectedSlot.None;

             }



         });

        public static class EnchantSlot
        {
            [Flags]
            public enum SelectedSlot
            {
                None = 0,
                Slot1 = 1,
                Slot2 = 2,
                Slot3 = 4,
            }

            public static Aligned<GfxStr> Slots = new Aligned<GfxStr>(new REMOPoint(70, 270), new REMOPoint(0, 40));
            public static SelectedSlot SelectedSlots = SelectedSlot.None;
            public static SelectedSlot[] Flags = new SelectedSlot[] { SelectedSlot.Slot1, SelectedSlot.Slot2, SelectedSlot.Slot3 };

            public static void Make()
            {
                Slots.Clear();
                for (int i = 0; i < 3; i++)
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("Slot " + (i + 1) + "(" + (i + 1) + ")" + " : ");
                    if (Pickaxe.Enchants[i].Item1 == "A")
                    {
                        s.Append("Attack Increase ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100) - 100);
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "BD")
                    {
                        s.Append("Defense Breaking ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100));
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "CC")
                    {
                        s.Append("Critical Chance ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100));
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "CD")
                    {
                        s.Append("Critical Damage ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100));
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "AM")
                    {
                        s.Append("Auto Mining(1sec) ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100));
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "UB")
                    {
                        s.Append("Unbreakable ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100));
                        s.Append("%");
                    }





                    Slots.Add(new GfxStr(s.ToString()));
                }

            }
            public static void Update()
            {
                Slots.Align();
                if (User.JustPressed(Keys.D1) || User.JustLeftClicked(Slots[0]))
                {
                    SelectedSlots ^= SelectedSlot.Slot1;
                }
                if (User.JustPressed(Keys.D2) || User.JustLeftClicked(Slots[1]))
                {
                    SelectedSlots ^= SelectedSlot.Slot2;
                }
                if (User.JustPressed(Keys.D3) || User.JustLeftClicked(Slots[2]))
                {
                    SelectedSlots ^= SelectedSlot.Slot3;
                }
            }

            public static int GetCost()
            {
                int c = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (EnchantSlot.SelectedSlots.HasFlag(EnchantSlot.Flags[i]))
                        c++;
                }

                return EnchantFee * c;
            }
            public static void Draw()
            {
                StandAlone.DrawString("Enchant Slot", new REMOPoint(70, 220), Color.LightBlue);

                for (int i = 0; i < 3; i++)
                {
                    if (SelectedSlots.HasFlag(Flags[i]))
                    {
                        Slots[i].Draw(Color.Yellow);
                    }
                    else
                        Slots[i].Draw(Color.LightBlue);


                }
                if (EnchantSlot.SelectedSlots == EnchantSlot.SelectedSlot.None)
                {
                    StandAlone.DrawString("Cost : 0 (Please select slot!)", EnchantButton.Pos + new REMOPoint(0, 50), Color.LightBlue);
                }
                else
                {
                    StandAlone.DrawString("Cost : " + GetCost() + "S", EnchantButton.Pos + new REMOPoint(0, 50), Color.Yellow);
                }

            }
        }


        public static int PressingRTimer = 0;
        public static GfxStr PickaxeLevelString = new GfxStr("level " + Pickaxe.Level + " Pickaxe", new REMOPoint(70, 90));


        public static CheckBox Protected = new CheckBox(15, "Protect(P)", ReinforceButton.Pos + new REMOPoint(0, -50));


        public static Button LandfillChan = new Button(new Gfx2D("Garbage3", new REMOPoint(750, 300), 0.5f), () =>
        {

            ScriptScene.dialogLoader.EnterScript("MINE.Landfill", () =>
            {
                LandFillScene.trigger = 1;
                Projectors.Projector.SwapTo(TuningScene.scn);
            });

        });

        public static Button Present = new Button(new Gfx2D("Present", new REMOPoint(280, 30), 0.1f),()=>{ Projectors.Projector.SwapTo(PresentScene.scn); });

        public static Scene scn = new Scene(() =>
        {
            if(LandFillScene.trigger==1)
                UserInterface.SetButtons( UserInterface.LandFillSceneButton, UserInterface.MiningSceneButton);
            else
                UserInterface.SetButtons(UserInterface.MiningSceneButton);

            scn.InitOnce(() =>
            {
                EnchantSlot.Make();
                ScriptScene.dialogLoader.CGPipeline.Add("CG1", new Gfx2D("Garbage3", new REMOPoint(700, 200), 0.5f));
            });
        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Left))
                Projectors.Projector.SwapTo(MiningScene.scn);
            if (User.JustPressed(Keys.Right) && LandFillScene.trigger == 1)
                Projectors.Projector.SwapTo(LandFillScene.scn);


            ReinforceButton.Enable();
            if (User.Pressing(Keys.R))
                PressingRTimer++;
            if (User.JustPressed(Keys.R)||PressingRTimer>10)
            {
                PressingRTimer = 0;
                ReinforceButton.ButtonClickAction();
            }
            if(User.JustPressed(Keys.E))
            {
                EnchantButton.ButtonClickAction();
            }

            if (UserInterface.EnchantStone > 0)
                EnchantButton.Enable();
            EnchantSlot.Update();
            Protected.Description.Text = "Protect Destruction(P)" + "("+ LandFillScene.Item1_Count + ")";

            if (LandFillScene.Item1_Count > 0)
            {
                Protected.Update();
            }
            else
            {
                Protected.isChecked = false;
            }
            if (User.JustPressed(Keys.P))
                Protected.isChecked = !Protected.isChecked;

            PickaxeLevelString.Text = "level " + Pickaxe.Level + " Pickaxe";

            if (LandFillScene.trigger == 0)
                LandfillChan.Enable();
            if (Pickaxe.Level >= 30)
                Present.Enable();
        }, () =>
        {
            StandAlone.DrawString("(Let's make level 30!)", new REMOPoint(70, 50), Color.Gray);

            StandAlone.DrawString("level " + Pickaxe.Level + " Pickaxe", new REMOPoint(70, 90), Color.White);
            PickaxeLevelString.Draw();
            StringBuilder PowerDescription = new StringBuilder();
            PowerDescription.Append("Power : ");
            PowerDescription.Append(Pickaxe.GetPower());
            int powerChanged = Pickaxe.GetPower() - Pickaxe.Power;
            if(powerChanged!=0)
            {
                PowerDescription.Append("(");
                if (powerChanged > 0)
                    PowerDescription.Append("+");
                PowerDescription.Append(powerChanged + ")");
            }
            StandAlone.DrawString(PowerDescription.ToString(), new REMOPoint(70, 130), Color.White);
            StandAlone.DrawString("Possibility : " + (int)(ReinforcePossibility*100)+"%", ReinforceButton.Pos + new REMOPoint(0, 40), Color.White);
            UserInterface.Draw();
            ReinforceButton.DrawWithAccent(Color.Gold, Color.Red);
            if(UserInterface.EnchantStone>0)
            {
                EnchantButton.DrawWithAccent(Color.LightBlue, Color.Red);
                EnchantSlot.Draw();
            }
            if(Pickaxe.Enchants[0].Item1!=""|| Pickaxe.Enchants[1].Item1 != ""|| Pickaxe.Enchants[2].Item1 != "")
            {
                EnchantButton.DrawWithAccent(Color.LightBlue, Color.Red);
                EnchantSlot.Draw();
            }
           if (LandFillScene.Item1_Count > 0)
                Protected.Draw(Color.White);
            if (LandFillScene.trigger == 0)
                LandfillChan.DrawWithAccent(Color.White,Color.Red);

            if (Pickaxe.Level >= 30)
                Present.DrawWithAccent(Color.White,Color.Pink);

            Fader.DrawAll();
            Cursor.Draw(Color.White);
        });

    }

    public static class PresentScene
    {
        
        public static Scene scn = new Scene(() =>
        {

        }, () =>
        {
            if(User.JustLeftClicked())
            {
                Projectors.Projector.SwapTo(TuningScene.scn);
            }

        }, () =>
        {
            StandAlone.DrawFullScreen("Ending");
            Cursor.Draw(Color.White);
        });




    }


    public static class LandFillScene
    {
        public static int trigger=-1;
        [Flags]
        public enum ItemSelection
        {
            None = 0, Item1 = 1, Item2 = 2, Item3 =3,
        }
        public static ItemSelection SelectedItem = ItemSelection.None;
        public static Gfx2D Item1 = new Gfx2D("MINE.Item1", new REMOPoint(300, 150), 0.4f);
        public static int Item1_Count = 0;
        public static int Used_Item1_Count = 0;
        public static int Item2_Count = 0;
        public static int Item3_Count = 0;
        public static int EggTimer = 0;
        public static Gfx2D Item2 = new Gfx2D("MINE.Item2", new REMOPoint(400, 150), 0.4f);
        public static Gfx2D Item3 = new Gfx2D("MINE.Egg", new REMOPoint(500, 150), 0.4f);

        public static Gfx2D GarbageGirl = new Gfx2D("MINE.Garbage3", new REMOPoint(0, 190), 0.5f);
        public static GfxStr CurrentTalk = new GfxStr("Hello.. Mister... What do you want to exchange?", new REMOPoint(300, 400));
        public static Button BuyButton = new Button(new Gfx2D("BuyButton", new REMOPoint(700, 200), 0.5f), () =>
              {
                  if(UserInterface.Trash>=GetPrice())
                  {
                      if (SelectedItem == ItemSelection.Item1)
                      {
                          UserInterface.Trash -= GetPrice();
                          Item1_Count++;
                      }
                      if (SelectedItem == ItemSelection.Item2)
                      {
                          UserInterface.Trash -= GetPrice();
                          Item2_Count++;
                      }
                      if (SelectedItem == ItemSelection.Item3)
                      {
                          if (Pet.PetCode==""&&EggTimer==0)
                          {
                              UserInterface.Trash -= GetPrice();
                              Item3_Count++;
                              Pet.PetGraphic.Sprite = "Egg2";
                              EggTimer = 3600;
                          }
                          else
                          {
                              CurrentTalk.Text = "You already have one...";
                          }

                      }

                  }
              });

        public static int GetPrice()
        {
            if (SelectedItem == ItemSelection.Item1)
            {
                return 5 * (Item1_Count + Used_Item1_Count+ 1);
            }
            else if (SelectedItem == ItemSelection.Item2)
            {
                return 100 * (int)Math.Pow(4,Item2_Count);
            }
            else if (SelectedItem == ItemSelection.Item3)
            {
                return 30 * (Item3_Count+1);
            }

            return -1;
        }
        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons(UserInterface.MiningSceneButton2, UserInterface.TuningSceneButton2);
            CurrentTalk.Text = "Hello.. Mister... What do you want to exchange?";
        }, () =>
        {
            if (User.JustPressed(Keys.Left))
                Projectors.Projector.SwapTo(TuningScene.scn);
            if (User.JustPressed(Keys.Right))
                Projectors.Projector.SwapTo(MiningScene.scn);
            if(User.JustLeftClicked(Item1)||User.JustPressed(Keys.D1))
            {
                if (SelectedItem != ItemSelection.Item1)
                    SelectedItem = ItemSelection.Item1;
                else
                    SelectedItem = ItemSelection.None;
            }
            if (User.JustLeftClicked(Item2) || User.JustPressed(Keys.D2))
            {
                if (SelectedItem != ItemSelection.Item2)
                    SelectedItem = ItemSelection.Item2;
                else
                    SelectedItem = ItemSelection.None;
            }
            if (User.JustLeftClicked(Item3) || User.JustPressed(Keys.D3))
            {
                if (SelectedItem != ItemSelection.Item3)
                    SelectedItem = ItemSelection.Item3;
                else
                    SelectedItem = ItemSelection.None;
            }
            if (User.JustPressed(Keys.B))
                BuyButton.ButtonClickAction();


            BuyButton.Enable();

            UserInterface.Update();
        }, () =>
        {
            UserInterface.Draw();
            Item1.Draw();
            Item2.Draw();
            Item3.Draw();
            GarbageGirl.Draw();
            CurrentTalk.Draw(Color.White);
            if(SelectedItem==ItemSelection.Item1)
            {
                Item1.Draw(Color.Red * 0.3f);
                StandAlone.DrawString("Protection Charm", Item1.Pos + new REMOPoint(0, 90),Color.Yellow);
                StandAlone.DrawString("Protects breaking of pickaxe(1 time)", Item1.Pos + new REMOPoint(0, 120), Color.White);
                StandAlone.DrawString("Price : " + GetPrice() + " Trash", Item1.Pos + new REMOPoint(0, 160), Color.White) ;
                BuyButton.DrawWithAccent(Color.White, Color.Pink);
            }
            else if (SelectedItem == ItemSelection.Item2)
            {
                Item2.Draw(Color.Red * 0.3f);
                StandAlone.DrawString("Iron Ore", Item1.Pos + new REMOPoint(0, 90), Color.Yellow);
                StandAlone.DrawString("When you make new pickaxe, it's level would be "+5*(Item2_Count+1)+".", Item1.Pos + new REMOPoint(0, 120), Color.White);
                StandAlone.DrawString("Price : "+GetPrice()+" Trash", Item1.Pos + new REMOPoint(0, 160), Color.White);
                BuyButton.DrawWithAccent(Color.White, Color.Pink);
            }
            else if (SelectedItem == ItemSelection.Item3)
            {
                Item3.Draw(Color.Red * 0.3f);
                StandAlone.DrawString("Weird Egg", Item1.Pos + new REMOPoint(0, 90), Color.Yellow);
                StandAlone.DrawString("This might contains a life...", Item1.Pos + new REMOPoint(0, 120), Color.White);
                StandAlone.DrawString("Price : " + GetPrice() + " Trash", Item1.Pos + new REMOPoint(0, 160), Color.White);
                BuyButton.DrawWithAccent(Color.White, Color.Pink);
            }
            else
                BuyButton.Draw(Color.Gray);

            StandAlone.DrawString("Protection Charm : " + Item1_Count, new REMOPoint(50, 80), Color.White);
            StandAlone.DrawString("Iron Ore : " + Item2_Count, new REMOPoint(50, 110), Color.White);


            Cursor.Draw(Color.White);
        });

    }


    public static class ScriptScene
    {
        public static DialogLoader dialogLoader = new DialogLoader(20, new REMOPoint(50,250), new REMOPoint(50, 300), Color.White,Color.Black);
    

    }

}
