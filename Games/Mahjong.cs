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

namespace Mahjong
    {
        public class MJHand
        {
            public MJHand(string Code)
            {
                MJCode = Code;
            }
            public string MJCode;//Majong Code. A1~A9 : Man , B1~B9 : Balls , C1~C9 : Socks D1~D4 : Winds E1~E3 : bak bar jung
            public Gfx2D Tile = new Gfx2D(new Rectangle(0, 0, 50, 50));

            public void Draw()
            {
                Tile.Draw(Color.White);
                StandAlone.DrawString(MJCode, Tile.Center, Color.Red);
            }

        }

        public static class MJStack
        {
            public static List<String> MJStacks = new List<String>();


            private static void GetTiles(string Code, int count)
            {
                for (int i = 1; i <= count; i++)
                {
                    MJStacks.Add(Code + i);
                    MJStacks.Add(Code + i);
                    MJStacks.Add(Code + i);
                    MJStacks.Add(Code + i);

                }
            }
            private static void Swap<T>(IList<T> list, int indexA, int indexB)
            {
                T tmp = list[indexA];
                list[indexA] = list[indexB];
                list[indexB] = tmp;
            }
            public static void Build()
            {
                GetTiles("A", 9);
                GetTiles("B", 9);
                GetTiles("C", 9);
                GetTiles("D", 4);
                GetTiles("E", 3);

                //Shuffle
                for (int i = 0; i < 1000; i++)
                {
                    int r = StandAlone.Random(0, MJStacks.Count - 1);
                    Swap<String>(MJStacks, r, (2 * r) % MJStacks.Count);
                }


                //Get Hands
                for (int i = 0; i < 34; i++)
                    GetTiles();


                for (int i = 0; i < 3; i++)
                    GetMarket();

                MyHands.Sort();
            }

            public static void GetTiles()
            {
                MyHands.Add(MJStacks[0], MyHands.Tiles);
                MJStacks.RemoveAt(0);
            }

            public static void GetMarket()
            {
                MyHands.Add(MJStacks[0], MyHands.Market);
                MJStacks.RemoveAt(0);
            }
        }

        public static class MyHands
        {
            public static Aligned<Gfx2D> Tiles = new Aligned<Gfx2D>(new REMOPoint(100, 300), new REMOPoint(60, 0));
            public static Aligned<Gfx2D> Hands = new Aligned<Gfx2D>(new REMOPoint(100, 600), new REMOPoint(60, 0));
            public static Aligned<Gfx2D> Market = new Aligned<Gfx2D>(new REMOPoint(100, 800), new REMOPoint(60, 0));
            public static Dictionary<Gfx2D, String> Codes = new Dictionary<Gfx2D, string>();


            public static void Sort(Aligned<Gfx2D> tiles)
            {
                tiles.Components.Sort(delegate (Gfx2D p1, Gfx2D p2)
                {
                    return Codes[p1].CompareTo(Codes[p2]);
                });
                tiles.Align();
            }

            public static void Sort()
            {
                Sort(Tiles);
                Sort(Hands);
                Sort(Market);
            }

            public static void Add(String Code, Aligned<Gfx2D> tiles)
            {
                Gfx2D g = new Gfx2D(new Rectangle(0, 0, 50, 80));
                Codes.Add(g, Code);

                g.RegisterDrawAct(() =>
                {
                    g.Draw(Color.White);
                    if (g.ContainsCursor())
                        g.Draw(Color.Blue);
                    StandAlone.DrawString(Code, g.Center - new REMOPoint(5, 5), Color.Red);
                });
                tiles.Add(g);
                tiles.Align();
            }

            public static void Remove(int index, Aligned<Gfx2D> tiles)
            {
                Gfx2D g = tiles[index];
                Codes.Remove(g);
                tiles.RemoveAt(index);
            }


            public static void Swap(int index, Aligned<Gfx2D> From, Aligned<Gfx2D> To)
            {
                Gfx2D g = From[index];
                From.RemoveAt(index);
                To.Add(g);
                Sort();
            }

            public static void Align()
            {
                Tiles.Align();
            }

            public static void Draw()
            {
                Tiles.Draw();
                Hands.Draw();
                Market.Draw();
            }

        }

        public static class MJGame
        {
            public static int SelectedMarketIndex = -1;
            public static List<Gfx2D> SelectedDeal = new List<Gfx2D>();

            public static void Pay()
            {
                for (int i = 0; i < SelectedDeal.Count; i++)
                {
                    if (MyHands.Hands.Components.Contains(SelectedDeal[i]))
                        MyHands.Hands.Components.Remove(SelectedDeal[i]);
                    if (MyHands.Tiles.Components.Contains(SelectedDeal[i]))
                        MyHands.Tiles.Components.Remove(SelectedDeal[i]);
                }
                while (SelectedDeal.Count > 0)
                    SelectedDeal.RemoveAt(0);
                MyHands.Add(MyHands.Codes[MyHands.Market[SelectedMarketIndex]], MyHands.Hands);
            }
            public static Scene scn = new Scene(() =>
            {
                MJStack.Build();
                StandAlone.FullScreen = new Rectangle(0, 0, 2500, 1000);

            }, () =>
            {
                int k;

                if (SelectedMarketIndex == -1)
                {
                    if ((k = MyHands.Market.ClickedIndex) != -1)
                        SelectedMarketIndex = k;


                    //Make Hands
                    if ((k = MyHands.Tiles.ClickedIndex) != -1)
                    {
                        MyHands.Swap(k, MyHands.Tiles, MyHands.Hands);
                        MyHands.Hands.Align();
                        MyHands.Sort();
                    }

                    if ((k = MyHands.Hands.ClickedIndex) != -1)
                    {
                        MyHands.Swap(k, MyHands.Hands, MyHands.Tiles);
                        MyHands.Hands.Align();
                        MyHands.Sort();
                    }
                }
                else//Control Market
                {
                    if (MyHands.Market.ClickedIndex == SelectedMarketIndex)
                    {
                        SelectedMarketIndex = -1;
                        while (SelectedDeal.Count > 0)
                            SelectedDeal.RemoveAt(0);
                    }


                    //Select Deals to Pay

                    if ((k = MyHands.Tiles.ClickedIndex) != -1)
                    {
                        if (SelectedDeal.Contains(MyHands.Tiles[k]))
                            SelectedDeal.Remove(MyHands.Tiles[k]);
                        else if (SelectedDeal.Count <= 1)
                            SelectedDeal.Add(MyHands.Tiles[k]);
                        else if (SelectedDeal.Count == 2)
                        {
                            SelectedDeal.Add(MyHands.Tiles[k]);
                            Pay();
                        }

                    }

                    if ((k = MyHands.Hands.ClickedIndex) != -1)
                    {
                        if (SelectedDeal.Contains(MyHands.Hands[k]))
                            SelectedDeal.Remove(MyHands.Hands[k]);
                        else if (SelectedDeal.Count <= 1)
                            SelectedDeal.Add(MyHands.Hands[k]);
                        else if (SelectedDeal.Count == 2)
                        {
                            SelectedDeal.Add(MyHands.Hands[k]);
                            Pay();
                        }

                    }

                }

            }, () =>
            {
                MyHands.Draw();
                if (SelectedMarketIndex != -1)
                    Filter.Absolute(MyHands.Market[SelectedMarketIndex], Color.Yellow * 0.5f);
                for (int i = 0; i < SelectedDeal.Count; i++)
                    Filter.Absolute(SelectedDeal[i], Color.Yellow * 0.5f);
                StandAlone.DrawString("Dora : " + MJStack.MJStacks[MJStack.MJStacks.Count - 1], new REMOPoint(10, 10), Color.White);
                Cursor.Draw(Color.White);
            });
        }
    }
    
