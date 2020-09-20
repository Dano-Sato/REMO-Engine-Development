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

namespace MineCrazy
{


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
                if(r<0.3)
                {
                    Enchants[SlotNum] = new Tuple<string, double>("A", StandAlone.Random(0.2, 2));
                }
                else if(r<0.6)
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
        public static Button TuningSceneButton = new Button(new GfxStr("Go To Tuning Scene(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(TuningScene.scn); });
        public static Button MiningSceneButton = new Button(new GfxStr("Go To Mining Scene(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(MiningScene.scn); });
        public static Button LandFillSceneButton = new Button(new GfxStr("Go To Landfill Scene(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(LandFillScene.scn);
        });
        public static Button TuningSceneButton2 = new Button(new GfxStr("Go To Tuning Scene(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(TuningScene.scn);
        });
        public static Button MiningSceneButton2 = new Button(new GfxStr("Go To Mining Scene(→)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(MiningScene.scn);
        });
        public static Button LandFillSceneButton2 = new Button(new GfxStr("Go To Landfill Scene(←)", new REMOPoint(700, 100)), () => {
            Projectors.Projector.SwapTo(LandFillScene.scn);
        });



        public static Aligned<Button> CurrentButtons=new Aligned<Button>(new REMOPoint(700,100),new REMOPoint(0,40)); 


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
                if(User.JustLeftClicked(Rock.Graphic)||User.JustPressed(Keys.Z)|| User.JustPressed(Keys.X) || User.JustPressed(Keys.C) || User.JustPressed(Keys.V)||MiningTimer>5)
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

        public static Gfx2D Player = new Gfx2D("MINE.Player", PickaxeGrapic.Center-new REMOPoint(30,80), 0.5f);

        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons(UserInterface.TuningSceneButton,UserInterface.LandFillSceneButton2);

            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Right))
                Projectors.Projector.SwapTo(TuningScene.scn);
            if (User.JustPressed(Keys.Left))
                Projectors.Projector.SwapTo(LandFillScene.scn);

            if (User.Pressing(Keys.Z)|| User.Pressing(Keys.X)|| User.Pressing(Keys.C)|| User.Pressing(Keys.V))
                MiningTimer++;
            Rock.Update();
            PrevRockButton.Enable();
            NextRockButton.Enable();

        }, () =>
        {
            Rock.Draw();
            PickaxeGrapic.Draw();
            Player.Draw();

            if (Pickaxe.Level >= 10)
                PickaxeGrapic.Draw(Color.LightBlue);
            StandAlone.DrawString("Press Z,X,C,V to Mine!(Z,X,C,V)", new REMOPoint(450,260), Color.White);
            UserInterface.Draw();
            PrevRockButton.DrawWithAccent(Color.White, Color.Red);
            NextRockButton.DrawWithAccent(Color.White, Color.Red);

            Cursor.Draw(Color.White);
        });
    }

    public static class TuningScene
    {
        public static int Reinforcefee = 50;
        public static float ReinforcePossibility = 1.0f;
        public static Button ReinforceButton = new Button(new GfxStr("Reinforce(R) : " + Reinforcefee+"G", new REMOPoint(400, 200)), () =>
               {
                   if(UserInterface.Gold>=(ulong)Reinforcefee)
                   {
                       UserInterface.Gold -= (ulong)Reinforcefee;
                       if(StandAlone.Random()<ReinforcePossibility)
                           Pickaxe.Level += 1;
                       else if(!Protected.isChecked||LandFillScene.Item1_Count==0)
                       {
                           UserInterface.Trash += Pickaxe.Level;
                           Pickaxe.Level = Math.Max(1,LandFillScene.Item2_Count*5);
                           Pickaxe.Enchants = new Tuple<string, double>[] { new Tuple<string, double>("", 0), new Tuple<string, double>("", 0), new Tuple<string, double>("", 0) };
                           EnchantSlot.Make();
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
        public static Button EnchantButton = new Button(new GfxStr("Enchant(E)", ReinforceButton.Pos + new REMOPoint(0, 120)),()=> 
        { 
            if(UserInterface.EnchantStone>=EnchantSlot.GetCost())
            {
                UserInterface.EnchantStone -= EnchantSlot.GetCost();

                for (int i=0;i<3;i++)
                {
                    if(EnchantSlot.SelectedSlots.HasFlag(EnchantSlot.Flags[i]))
                    {
                        Pickaxe.Enchant(i);
                        TuningScene.EnchantSlot.Make();
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
                None=0,
                Slot1=1,
                Slot2=2,
                Slot3=4,
            }
            
            public static Aligned<GfxStr> Slots = new Aligned<GfxStr>(new REMOPoint(70, 270), new REMOPoint(0, 40));
            public static SelectedSlot SelectedSlots = SelectedSlot.None;
            public static SelectedSlot[] Flags = new SelectedSlot[] { SelectedSlot.Slot1, SelectedSlot.Slot2, SelectedSlot.Slot3 };

            public static void Make()
            {
                Slots.Clear();
                for(int i=0;i<3;i++)
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("Slot " + (i + 1) +"("+(i+1)+")"+" : ");
                    if (Pickaxe.Enchants[i].Item1 == "A")
                    {
                        s.Append("Attack Increase ");
                        s.Append((int)(Pickaxe.Enchants[i].Item2 * 100)-100);
                        s.Append("%");
                    }
                    if (Pickaxe.Enchants[i].Item1 == "BD")
                    {
                        s.Append("Breaking Defense ");
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




                    Slots.Add(new GfxStr(s.ToString()));
                }

            }
            public static void Update()
            {
                Slots.Align();
                if(User.JustPressed(Keys.D1)||User.JustLeftClicked(Slots[0]))
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

                for (int i=0;i<3;i++)
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
                    StandAlone.DrawString("Cost : " + GetCost()+"S", EnchantButton.Pos + new REMOPoint(0, 50), Color.Yellow);
                }

            }
        }


        public static int PressingRTimer = 0;

        public static CheckBox Protected = new CheckBox(15, "Protect(P)", ReinforceButton.Pos + new REMOPoint(0, -50));

        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons( UserInterface.LandFillSceneButton, UserInterface.MiningSceneButton);
            scn.InitOnce(() =>
            {
                EnchantSlot.Make();
            });
        }, () =>
        {
            UserInterface.Update();
            if (User.JustPressed(Keys.Left))
                Projectors.Projector.SwapTo(MiningScene.scn);
            if (User.JustPressed(Keys.Right))
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
           

        }, () =>
        {
            StandAlone.DrawString("(Let's make level 30!)", new REMOPoint(70, 50), Color.Gray);

            StandAlone.DrawString("level " + Pickaxe.Level + " Pickaxe", new REMOPoint(70, 90), Color.White);
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



            Cursor.Draw(Color.White);
        });

    }

    public static class LandFillScene
    {
        [Flags]
        public enum ItemSelection
        {
            None = 0, Item1 = 1, Item2 = 2
        }
        public static ItemSelection SelectedItem = ItemSelection.None;
        public static Gfx2D Item1 = new Gfx2D("MINE.Item1", new REMOPoint(300, 150), 0.4f);
        public static int Item1_Count = 0;
        public static int Used_Item1_Count = 0;
        public static int Item2_Count = 0;
        public static Gfx2D Item2 = new Gfx2D("MINE.Item2", new REMOPoint(400, 150), 0.4f);
        public static Gfx2D GarbageGirl = new Gfx2D("MINE.Garbage3", new REMOPoint(0, 190), 0.5f);
        public static GfxStr CurrentTalk = new GfxStr("Hello.. Mister... What do you want to exchange?", new REMOPoint(300, 400));
        public static Button BuyButton = new Button(new Gfx2D("BuyButton", new REMOPoint(700, 200), 0.5f), () =>
              {
                  if(UserInterface.Trash>=GetPrice())
                  {
                      UserInterface.Trash -= GetPrice();
                      if (SelectedItem == ItemSelection.Item1)
                          Item1_Count++;
                      if (SelectedItem == ItemSelection.Item2)
                          Item2_Count++;
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
            return -1;
        }
        public static Scene scn = new Scene(() =>
        {
            UserInterface.SetButtons(UserInterface.MiningSceneButton2, UserInterface.TuningSceneButton2);
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

            BuyButton.Enable();

            UserInterface.Update();
        }, () =>
        {
            UserInterface.Draw();
            Item1.Draw();
            Item2.Draw();
            GarbageGirl.Draw();
            CurrentTalk.Draw(Color.White);
            if(SelectedItem==ItemSelection.Item1)
            {
                Item1.Draw(Color.Red * 0.3f);
                StandAlone.DrawString("Protection Charm", Item1.Pos + new REMOPoint(0, 90),Color.Yellow);
                StandAlone.DrawString("Protects destruction of pickaxe(1 time)", Item1.Pos + new REMOPoint(0, 120), Color.White);
                StandAlone.DrawString("Price : " + GetPrice() + " Trash", Item1.Pos + new REMOPoint(0, 160), Color.White) ;
                BuyButton.DrawWithAccent(Color.White, Color.Pink);
            }
            else if (SelectedItem == ItemSelection.Item2)
            {
                Item2.Draw(Color.Red * 0.3f);
                StandAlone.DrawString("Iron Ore", Item1.Pos + new REMOPoint(0, 90), Color.Yellow);
                StandAlone.DrawString("When you make new pickaxe, it's level is "+5*(Item2_Count+1)+".", Item1.Pos + new REMOPoint(0, 120), Color.White);
                StandAlone.DrawString("Price : "+GetPrice()+" Trash", Item1.Pos + new REMOPoint(0, 160), Color.White);
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
        public static Scripter DialogReader = new Scripter();
        public static Scene scn = new Scene(() =>
        {
            scn.InitOnce(() =>
            {
                DialogReader.AddRule("n", (s) => { });
            });

        }, () =>
        {

        }, () =>
        {

        });

    }

}
