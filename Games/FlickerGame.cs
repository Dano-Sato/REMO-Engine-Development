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

namespace FlickerGame
{
    public static class FlickerGame
    {
        public static Gfx2D Player = new Gfx2D(new Rectangle(200, 250, 40, 40));
        public static Gfx2D Ground = new Gfx2D(new Rectangle(0, 390, 1400, 500));
        public static Vector2 v = new Vector2(0, -1f);//물체속도
        public static Vector2 g = new Vector2(0, 1f);//중력 가속도
        public static void MoveSquare() => Player.Pos += v.ToPoint();
        public static List<Gfx> Enemies = new List<Gfx>();
        public static List<Gfx> HealEnemies = new List<Gfx>();
        public static List<Gfx> sinEnemies = new List<Gfx>();
        public static List<Gfx> floorEnemies = new List<Gfx>();
        public static List<Gfx> upEnemies = new List<Gfx>();
        public static List<Gfx> bigEnemies = new List<Gfx>();
        public static int jumpcount = 0;
        public static int starTimer = 300;

        public static int Score = 0;

        public static readonly int PlayerHpMax = 3000;

        public static readonly int healstackMax = 4;
        public static int player_hp = PlayerHpMax;
        public static int healstack = 0;
        public static int Atk = 400;
        public static int Heal = 300;

        public static int CoolTime = 0;
        public static int HpCoolTime = 0;
        public static int sinCoolTime = 0;
        public static int floorCoolTime = 0;
        public static int upCoolTime = 0;
        public static int bigCoolTime = 0;
        public static int LongjumpMax = 2;
        public static int JumpMax = 5;

        public static Gfx2D HpBar = new Gfx2D(new Rectangle(0, 420, 0, 10));
        public static int BarLength = 300;
        public static int jumpBarLength = 200;
        public static Gfx2D StarBar = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static Gfx2D StarBarBG = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static int StarBarLength = 270;

        public static Gfx2D healstackBar = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static int healstackBarLength = 270;

        public static Gfx CurrentEnemy;

        public static int Damagechecker = 0;
        public static int DamagecheckerMax = 30;
        public static int Starchecker = 0;

        public static Color DamageColor = Color.Red;

        public static List<Color> StarColors = new List<Color>(new Color[] { Color.DeepPink, Color.Yellow, Color.Blue, Color.DarkRed, Color.LightPink, Color.LightYellow, Color.BlueViolet, Color.Aqua, Color.DarkCyan, Color.Magenta });

        public static Scene scn = new Scene(() =>
        {

            scn.bgm = "SummerNight";
            Enemies.Clear();
            HealEnemies.Clear();
            sinEnemies.Clear();
            floorEnemies.Clear();
            upEnemies.Clear();
            bigEnemies.Clear();
            jumpcount = 0;
            healstack = 0;
            Score = 0;
            starTimer = 300;
            player_hp = PlayerHpMax;
            Damagechecker = 0;
            Starchecker = 0;
            StarBarBG.Bound = new Rectangle(StarBarBG.Pos, new Point(270, 10));
            LongjumpMax = 2;
            JumpMax = 5;


            StandAlone.FrameTimer = 0;
            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
        },
            () =>
            {
                MoveSquare();//square moves by velocity vector.
                v += g;//The object affected by gravity.

                if (Player.Pos.Y > Ground.Pos.Y - Player.Bound.Height)
                {
                    Player.Pos = new Point(Player.Pos.X, Ground.Pos.Y - Player.Bound.Height);
                    jumpcount = 0;
                }

                Score += 1;

                //Process Key input

                if (User.JustPressed(Keys.Space) || User.JustPressed(Keys.Up))
                {
                    if (jumpcount < LongjumpMax + 1)
                    {
                        v = new Vector2(0, -14);
                        jumpcount += 1;
                    }
                    else if (jumpcount > LongjumpMax && jumpcount < JumpMax)
                    {
                        v = new Vector2(0, -10);
                        jumpcount += 1;
                    }
                }

                if (User.Pressing(Keys.Down))
                {
                    v = new Vector2(0, 20);
                }

                if(User.JustPressed(Keys.Escape))
                {
                    Projectors.Projector.PauseAll();
                    Projectors.Projector.Load(PauseScene.scn);
                }

          
                //무적 
                if (healstack >= healstackMax && starTimer > 0)
                {
                    if(starTimer==299)
                        MusicBox.PlaySE("SE");

                    player_hp += 2;
                    healstackBar.Bound = new Rectangle(healstackBar.Pos, new Point(0, 10));
                    Starchecker = 1;
                    starTimer -= 1;
                    StarBar.Bound = new Rectangle(StarBar.Pos, new Point(StarBarLength * starTimer / 300, 10));
                    JumpMax = 10;
                    LongjumpMax = 5;
                }

                //무적 아닌 평상시
                if (starTimer <= 0)
                {

                    LongjumpMax = 2;
                    JumpMax = 5;
                    Starchecker = 0;
                    healstack = 0;
                    starTimer = 300;
                }

                if (starTimer == 300)
                {
                    player_hp -= 1;
                    healstackBar.Bound = new Rectangle(healstackBar.Pos, new Point((int)(StarBarLength / healstackMax * healstack), 10));
                    //player_hp -= 2;
                }







                //힐 적 

                for (int i = 0; i < HealEnemies.Count; i++)//힐
                {
                    HealEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                    if (Rectangle.Intersect(HealEnemies[i].Bound, Player.Bound) != Rectangle.Empty)//적과 부딪치면 hp가 답니다.
                    {
                        healstack += 1;
                        if (healstack != healstackMax)
                            MusicBox.PlaySE("SE2");
                        player_hp += Heal;
                        HealEnemies.RemoveAt(i);
                        i--;
                    }
                    else if (HealEnemies[i].Pos.X < -500)
                    {
                        HealEnemies.RemoveAt(i);
                        i--;
                    }

                }

                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                    if(Enemies[i].Pos.X<-500)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < sinEnemies.Count; i++)//적
                {
                    sinEnemies[i].MoveByVector(new Point(-10, 0), 10 + 7 * (StandAlone.Random(-1, 2)));
                    if (sinEnemies[i].Pos.X < -500)
                    {
                        sinEnemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < floorEnemies.Count; i++)
                {
                    floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                    if (floorEnemies[i].Pos.X < -500)
                    {
                        floorEnemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < bigEnemies.Count; i++)
                {
                    bigEnemies[i].MoveByVector(new Point(-10, 0), 5 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                    if (bigEnemies[i].Pos.X < -500)
                    {
                        bigEnemies.RemoveAt(i);
                        i--;
                    }

                }
                //피격판정

                int DamageCoefficient = StandAlone.FrameTimer / 500;

                if (starTimer >= 300)
                {
                    if (Damagechecker == 0)
                    {
                        //적 
                        for (int i = 0; i < Enemies.Count; i++)
                        {
                            if (Rectangle.Intersect(Enemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != Enemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = Enemies[i];
                                player_hp -= Atk+DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Red;
                            }
                        }

                        //sin 적 
                        for (int i = 0; i < sinEnemies.Count; i++)//적
                        {
                            if (Rectangle.Intersect(sinEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != sinEnemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = sinEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Blue;
                            }
                        }
                        //바닥 적 
                        for (int i = 0; i < floorEnemies.Count; i++)
                        {
                            //floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                            if (Rectangle.Intersect(floorEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != floorEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = floorEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Green;

                            }
                        }
          
                        for (int i = 0; i < bigEnemies.Count; i++)
                        {
                            if (Rectangle.Intersect(bigEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != bigEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = bigEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Brown;

                            }
                        }
                    }
                }
                if (Damagechecker <= DamagecheckerMax && Damagechecker > 0)
                    Damagechecker -= 1;



                if (player_hp <= 0)
                {
                    Projectors.Projector.PauseAll();
                    Projectors.Projector.Load(GameOverScene.scn);
                }


                //적 생성
                if (CoolTime > 0)
                    CoolTime--;
                else
                {
                    CoolTime = StandAlone.Random(13, 25);
                    Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30, 30))); // 적들을 생성합니다.
                }


                if(StandAlone.FrameTimer>2000)
                {
                    //사인 적 생성
                    if (sinCoolTime > 0)
                        sinCoolTime--;
                    else
                    {
                        sinCoolTime = StandAlone.Random(60, 80);
                        sinEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 350), 30, 30))); // 적들을 생성합니다.
                    }
                }


                //힐 적 생성 
                if (HpCoolTime > 0)
                    HpCoolTime--;
                else
                {
                    HpCoolTime = StandAlone.Random(50, 100)+Math.Min(StandAlone.FrameTimer/40,120);
                    HealEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 350), 20, 20))); // 적들을 생성합니다.
                }


                if (StandAlone.FrameTimer > 500)
                {
                    //바닥적 생성
                    if (floorCoolTime > 0)
                        floorCoolTime--;
                    else
                    {
                        floorCoolTime = StandAlone.Random(100, 250);
                        floorEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(370, 375), 50, 20))); // 적들을 생성합니다.
                    }

                }




                if(StandAlone.FrameTimer>4000)
                {
                    //큰 적 생성 
                    if (bigCoolTime > 0)
                        bigCoolTime--;
                    else
                    {
                        bigCoolTime = StandAlone.Random(600, 900);
                        bigEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 150), 120, 120))); // 적들을 생성합니다.
                    }


                }

                HpBar.Bound = new Rectangle(HpBar.Pos, new Point(BarLength * player_hp / PlayerHpMax, 10));

                if (player_hp > 9000)
                    player_hp = 9000;




            },
            () =>
            {

                Filter.Absolute(StandAlone.FullScreen, Color.Black);
                if (Starchecker > 0)
                {
                    Filter.Absolute(StandAlone.FullScreen, Color.White * 0.2f);
                    Filter.Absolute(StandAlone.FullScreen, StandAlone.RandomPick(StarColors) * 0.2f);
                }

                for (int i = 0; i < Enemies.Count; i++)
                    Enemies[i].Draw(Color.White);

                for (int i = 0; i < sinEnemies.Count; i++)
                    sinEnemies[i].Draw(Color.White);

                for (int i = 0; i < HealEnemies.Count; i++)
                    HealEnemies[i].Draw(Color.Yellow);
                 
                for (int i = 0; i < floorEnemies.Count; i++)
                    floorEnemies[i].Draw(Color.White);

                for (int i = 0; i < bigEnemies.Count; i++)
                    bigEnemies[i].Draw(Color.White);




                Player.Draw(Color.White);

                Color FadeColor = Color.White * 0.4f;
                if (Starchecker == 0)
                    Fader.Add(new Gfx2D(Player.Bound), (5 - jumpcount) * 5, FadeColor);



                if (Starchecker > 0)
                {
                    Color StarColor = StandAlone.RandomPick(StarColors);
                    Player.Draw(StarColor);
                    Fader.Add(new Gfx2D(Player.Bound), 15, StarColor * 0.4f);
                }

                foreach (Color c in Fader.FadeAnimations.Keys)
                {
                    foreach (Gfx2D g in Fader.FadeAnimations[c].Keys)
                    {
                        g.MoveByVector(new Point(-1, 0), 10);
                    }
                }
                Fader.DrawAll();
                if (Damagechecker > 0)
                {
                    Player.Draw(Color.White);
                    Player.Draw(DamageColor * (Damagechecker / (float)DamagecheckerMax));
                }
                int interval = 3000;
                int groundNumber = (Score / interval) % StarColors.Count;
                Ground.Draw(StarColors[groundNumber],StarColors[(groundNumber+1)%StarColors.Count]*((float)(Score%interval)/(float)interval),Color.Black*0.3f);
                StarBarBG.Draw(Color.Black);
                HpBar.Draw(Color.White);
                if (healstack >= healstackMax)
                    HpBar.Draw(StandAlone.RandomPick(StarColors));
                StarBar.Draw(Color.LightYellow);

                healstackBar.Draw(Color.LightGreen);

                StandAlone.DrawString(30, "Elapsed Time : " + (Score / 60).ToString() + "s", new Point(600, 430), Color.White);

            });
    }




    public static class GameOverScene
    {
        public static GfxStr GameOverString = new GfxStr("Game Over. Press R to Restart", new Point(200, 200));
        public static Scene[] SceneList = new Scene[] { FlickerGame.scn, TutorialScene.scn, TestClass.scn };
        public static Scene scn = new Scene(() => {
        }, () => {
            if (User.JustPressed(Keys.R))
            {
                foreach(Scene s in SceneList)
                {
                    if (Projectors.Projector.Loaded(s))
                        Projectors.Projector.SwapTo(s);
                }

            }
        }, () => {
            GameOverString.Draw(Color.White);
        });
    }

    public static class PauseScene
    {
        public static SimpleMenu PauseMenus = new SimpleMenu(30, new REMOPoint(100, 100), new REMOPoint(0, 70),
            new string[] { "Restart", "Go to main menu" },
            () =>
            {
                if (Projectors.Projector.Loaded(FlickerGame.scn))
                    Projectors.Projector.SwapTo(FlickerGame.scn);
                if (Projectors.Projector.Loaded(TutorialScene.scn))
                    Projectors.Projector.SwapTo(TutorialScene.scn);
            },
            ()=>
            {
                Projectors.Projector.SwapTo(MainScene.scn);
            }
            );
        
        public static Scene scn = new Scene(() =>
        {
            MusicBox.StopSong();
        }, () =>
        {
            StandAlone.FrameTimer--;
            if(User.JustPressed(Keys.Escape))
            {
                Projectors.Projector.Unload(PauseScene.scn);
                Projectors.Projector.ResumeAll();
                MusicBox.PlaySong("SummerNight");
            }
            PauseMenus.Update();
        }, () =>
        {
            Filter.Absolute(StandAlone.FullScreen, Color.Black * 0.6f);
            PauseMenus.Draw(Color.White);
        
            Cursor.Draw(Color.White);
        });
    }

    public static class MainScene
    {
        public static Gfx2D BigSquare = new Gfx2D(new Rectangle(0, 0, 800, 800));

        public static SimpleMenu MainMenus = new SimpleMenu(20, new REMOPoint(800, 200), new REMOPoint(0, 60),
            new string[] { "New Game", "Tutorial", "Exit" },
            () =>
            {
                Projectors.Projector.SwapTo(FlickerGame.scn);
            },         
            () =>
            {
                Projectors.Projector.SwapTo(TutorialScene.scn);
            }
            ,
            () =>
            {
                Game1.GameExit = true;
            }
            
            );
        public static Scene scn = new Scene(() =>
        {
            MusicBox.Mode = MusicBoxMode.FadeOut;
            BigSquare.Center = new REMOPoint(200, 200);
            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
            scn.bgm = "Journey";
        }, () =>
        {
            BigSquare.Rotate += 0.01f;
            MainMenus.Update();

        }, () =>
        {
            Filter.Absolute(StandAlone.FullScreen, Color.White);
            Filter.Absolute(StandAlone.FullScreen, Color.Black*Fader.Flicker(300));
            BigSquare.Draw(Color.Black, Color.White*Fader.Flicker(300));
            StandAlone.DrawString(40, "Flicker", new REMOPoint(200, 100), Color.White);
            StandAlone.DrawString(40, "Flicker", new REMOPoint(200, 100), Color.Black*Fader.Flicker(300));
            MainMenus.Draw(Color.Black, Color.White * Fader.Flicker(300));

            if (BigSquare.RContains(Cursor.Pos))
            {
                Cursor.Draw(Color.White);
                Cursor.Draw(Color.Black * Fader.Flicker(300));
            }
            else
            {
                Cursor.Draw(Color.Black);
                Cursor.Draw(Color.White*Fader.Flicker(300));
            }

        });
    }
    
    public static class TutorialScene
    {
        public static Gfx2D Player = new Gfx2D(new Rectangle(200, 250, 40, 40));
        public static Gfx2D Ground = new Gfx2D(new Rectangle(0, 390, 1400, 500));
        public static Vector2 v = new Vector2(0, -1f);//물체속도
        public static Vector2 g = new Vector2(0, 1f);//중력 가속도
        public static void MoveSquare() => Player.Pos += v.ToPoint();
        public static List<Gfx> Enemies = new List<Gfx>();
        public static List<Gfx> HealEnemies = new List<Gfx>();
        public static List<Gfx> sinEnemies = new List<Gfx>();
        public static List<Gfx> floorEnemies = new List<Gfx>();
        public static List<Gfx> upEnemies = new List<Gfx>();
        public static List<Gfx> bigEnemies = new List<Gfx>();
        public static int jumpcount = 0;
        public static int starTimer = 300;

        public static int Score = 0;

        public static readonly int PlayerHpMax = 3000;

        public static readonly int healstackMax = 4;
        public static int player_hp = PlayerHpMax;
        public static int healstack = 0;
        public static int Atk = 400;
        public static int Heal = 300;


        public static int CoolTime = 0;
        public static int HpCoolTime = 0;
        public static int sinCoolTime = 0;
        public static int floorCoolTime = 0;
        public static int upCoolTime = 0;
        public static int bigCoolTime = 0;
        public static int LongjumpMax = 2;
        public static int JumpMax = 5;

        public static Gfx2D HpBar = new Gfx2D(new Rectangle(0, 420, 0, 10));
        public static int BarLength = 300;
        public static int jumpBarLength = 200;
        public static Gfx2D StarBar = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static Gfx2D StarBarBG = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static int StarBarLength = 270;

        public static Gfx2D healstackBar = new Gfx2D(new Rectangle(0, 480, 0, 10));
        public static int healstackBarLength = 270;

        public static Gfx CurrentEnemy;

        public static int Damagechecker = 0;
        public static int DamagecheckerMax = 30;
        public static int Starchecker = 0;

        public static Color DamageColor = Color.Red;




        //Variable for tutorial
        private static int _tutorialState=-1;
        public static int TutorialState {
            get
            {
                return _tutorialState;
            }
            set
            {
                if(value<Descriptions.Length)
                Description.Text = Descriptions[value];
                _tutorialState = value;
            }
        }
        public static GfxStr Description = new GfxStr(20, "", new REMOPoint(0, 0));

        public static string[] Descriptions = new string[]
        {
            "Press Space to jump.",
            "You can Air-jump up to 4 times.",
            "Press Down-arrow to Drop in the air.",
            "Eat Yellow squares.",
            "you are in invincible(Flicker) state.",
            "Avoid other White squares.",
        };

        public static readonly int MakeHealEnemy = 3;
        public static readonly int MakeOtherEnemy = 5;





        public static Scene scn = new Scene(() =>
        {

            scn.bgm = "SummerNight";
            Enemies.Clear();
            HealEnemies.Clear();
            sinEnemies.Clear();
            floorEnemies.Clear();
            upEnemies.Clear();
            bigEnemies.Clear();
            jumpcount = 0;
            healstack = 0;
            Score = 0;
            starTimer = 300;
            player_hp = PlayerHpMax;
            Damagechecker = 0;
            Starchecker = 0;
            StarBarBG.Bound = new Rectangle(StarBarBG.Pos, new Point(270, 10));
            LongjumpMax = 2;
            JumpMax = 5;
            StandAlone.FrameTimer = 0;
            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);



            TutorialState = 0;
        },
            () =>
            {
                MoveSquare();//square moves by velocity vector.
                v += g;//The object affected by gravity.

                if (Player.Pos.Y > Ground.Pos.Y - Player.Bound.Height)
                {
                    Player.Pos = new Point(Player.Pos.X, Ground.Pos.Y - Player.Bound.Height);
                    jumpcount = 0;
                }

                Score += 1;

                //Process Key input

                if (User.JustPressed(Keys.Space) || User.JustPressed(Keys.Up))
                {
                    if (jumpcount < LongjumpMax + 1)
                    {
                        v = new Vector2(0, -14);
                        jumpcount += 1;
                    }
                    else if (jumpcount > LongjumpMax && jumpcount < JumpMax)
                    {
                        v = new Vector2(0, -10);
                        jumpcount += 1;
                    }
                }

                if (User.Pressing(Keys.Down))
                {
                    v = new Vector2(0, 20);
                }

                if (User.JustPressed(Keys.Escape))
                {
                    Projectors.Projector.PauseAll();
                    Projectors.Projector.Load(PauseScene.scn);
                }


                //무적 
                if (healstack >= healstackMax && starTimer > 0)
                {
                    if (starTimer == 299)
                        MusicBox.PlaySE("SE");

                    player_hp += 2;
                    healstackBar.Bound = new Rectangle(healstackBar.Pos, new Point(0, 10));
                    Starchecker = 1;
                    starTimer -= 1;
                    StarBar.Bound = new Rectangle(StarBar.Pos, new Point(StarBarLength * starTimer / 300, 10));
                    JumpMax = 10;
                    LongjumpMax = 5;
                }

                //무적 아닌 평상시
                if (starTimer <= 0)
                {

                    LongjumpMax = 2;
                    JumpMax = 5;
                    Starchecker = 0;
                    healstack = 0;
                    starTimer = 300;
                }

                if (starTimer == 300)
                {
                    healstackBar.Bound = new Rectangle(healstackBar.Pos, new Point((int)(StarBarLength / healstackMax * healstack), 10));
                }






                //적 Update                
          
                //힐 적 

                for (int i = 0; i < HealEnemies.Count; i++)//힐
                {
                    HealEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                    if (Rectangle.Intersect(HealEnemies[i].Bound, Player.Bound) != Rectangle.Empty)//적과 부딪치면 hp가 답니다.
                    {
                        healstack += 1;
                        if (healstack != healstackMax)
                            MusicBox.PlaySE("SE2");
                        player_hp += Heal;
                        HealEnemies.RemoveAt(i);
                        i--;
                    }
                    else if (HealEnemies[i].Pos.X < -500)
                    {
                        HealEnemies.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                    if (Enemies[i].Pos.X < -500)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < sinEnemies.Count; i++)//적
                {
                    sinEnemies[i].MoveByVector(new Point(-10, 0), 10 + 7 * (StandAlone.Random(-1, 2)));
                    if (sinEnemies[i].Pos.X < -500)
                    {
                        sinEnemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < floorEnemies.Count; i++)
                {
                    floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                    if (floorEnemies[i].Pos.X < -500)
                    {
                        floorEnemies.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < bigEnemies.Count; i++)
                {
                    bigEnemies[i].MoveByVector(new Point(-10, 0), 5 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                    if (bigEnemies[i].Pos.X < -500)
                    {
                        bigEnemies.RemoveAt(i);
                        i--;
                    }

                }
                //피격판정

                int DamageCoefficient = StandAlone.FrameTimer / 500;

                if (starTimer >= 300)
                {
                    if (Damagechecker == 0)
                    {
                        //적 
                        for (int i = 0; i < Enemies.Count; i++)
                        {
                            if (Rectangle.Intersect(Enemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != Enemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = Enemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Red;
                            }
                        }

                        //sin 적 
                        for (int i = 0; i < sinEnemies.Count; i++)//적
                        {
                            if (Rectangle.Intersect(sinEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != sinEnemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = sinEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Blue;
                            }
                        }
                        //바닥 적 
                        for (int i = 0; i < floorEnemies.Count; i++)
                        {
                            //floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                            if (Rectangle.Intersect(floorEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != floorEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = floorEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Green;

                            }
                        }

                        for (int i = 0; i < bigEnemies.Count; i++)
                        {
                            if (Rectangle.Intersect(bigEnemies[i].Bound, Player.Bound) != Rectangle.Empty && CurrentEnemy != bigEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = bigEnemies[i];
                                player_hp -= Atk + DamageCoefficient;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Brown;

                            }
                        }
                    }
                }
                if (Damagechecker <= DamagecheckerMax && Damagechecker > 0)
                    Damagechecker -= 1;



                if (player_hp <= 0)
                {
                    Projectors.Projector.PauseAll();
                    Projectors.Projector.Load(GameOverScene.scn);
                }


                /*적 생성*/


                if(TutorialState>=MakeHealEnemy)
                {

                    //힐 적 생성 
                    if (HpCoolTime > 0)
                        HpCoolTime--;
                    else
                    {
                        HpCoolTime = StandAlone.Random(50, 100) + Math.Min(StandAlone.FrameTimer / 40, 120);
                        HealEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 350), 20, 20))); // 적들을 생성합니다.
                    }


                    if(TutorialState>=MakeOtherEnemy)
                    {
                        if (CoolTime > 0)
                            CoolTime--;
                        else
                        {
                            CoolTime = StandAlone.Random(13, 25);
                            Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30, 30))); // 적들을 생성합니다.
                        }


                        if (StandAlone.FrameTimer > 2000)
                        {
                            //사인 적 생성
                            if (sinCoolTime > 0)
                                sinCoolTime--;
                            else
                            {
                                sinCoolTime = StandAlone.Random(60, 80);
                                sinEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 350), 30, 30))); // 적들을 생성합니다.
                            }
                        }




                        if (StandAlone.FrameTimer > 500)
                        {
                            //바닥적 생성
                            if (floorCoolTime > 0)
                                floorCoolTime--;
                            else
                            {
                                floorCoolTime = StandAlone.Random(100, 250);
                                floorEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(370, 375), 50, 20))); // 적들을 생성합니다.
                            }

                        }




                        if (StandAlone.FrameTimer > 4000)
                        {
                            //큰 적 생성 
                            if (bigCoolTime > 0)
                                bigCoolTime--;
                            else
                            {
                                bigCoolTime = StandAlone.Random(600, 900);
                                bigEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 150), 120, 120))); // 적들을 생성합니다.
                            }


                        }
                    }
                   

                }
                HpBar.Bound = new Rectangle(HpBar.Pos, new Point(BarLength * player_hp / PlayerHpMax, 10));

                if (player_hp > 9000)
                    player_hp = 9000;



                //잔상 이동
                foreach (Color c in Fader.FadeAnimations.Keys)
                {
                    foreach (Gfx g in Fader.FadeAnimations[c].Keys)
                    {
                        g.MoveByVector(new Point(-1, 0), 10);
                    }
                }

                //Tutorial

                if (TutorialState == 0 && User.JustPressed(Keys.Space))
                    TutorialState++;
                if (TutorialState == 1 && jumpcount == 5)
                    TutorialState++;
                if (TutorialState == 2 && User.JustPressed(Keys.Down)&&Player.Pos.Y!=350)
                    TutorialState++;
                if (TutorialState == 3 && healstack == healstackMax)
                    TutorialState++;
                if (TutorialState == 4 && starTimer==1)
                {
                    TutorialState++;
                    Enemies.Clear();
                    HealEnemies.Clear();
                    sinEnemies.Clear();
                    floorEnemies.Clear();
                    upEnemies.Clear();
                    bigEnemies.Clear();
                    jumpcount = 0;
                    healstack = 0;
                    Score = 0;
                    starTimer = 300;
                    player_hp = PlayerHpMax;
                    Damagechecker = 0;
                    Starchecker = 0;
                    StarBarBG.Bound = new Rectangle(StarBarBG.Pos, new Point(270, 10));
                    LongjumpMax = 2;
                    JumpMax = 5;
                    StandAlone.FrameTimer = 0;
                }

                if(TutorialState==5&&StandAlone.FrameTimer==1000)
                {
                    Fader.Add(Description, 100, Color.White * 0.99f);
                }





                Description.Center = new REMOPoint(500, 250);


            },
            () =>
            {
                List<Color> StarColors = new List<Color>(new Color[] { Color.DeepPink, Color.Yellow, Color.Blue, Color.DarkRed, Color.LightPink, Color.LightYellow, Color.BlueViolet, Color.Aqua, Color.DarkCyan, Color.Magenta });

                Filter.Absolute(StandAlone.FullScreen, Color.Black);
                if (Starchecker > 0)
                {
                    Filter.Absolute(StandAlone.FullScreen, Color.White * 0.2f);
                    Filter.Absolute(StandAlone.FullScreen, StandAlone.RandomPick(StarColors) * 0.2f);
                }
                int interval = 3000;
                int groundNumber = (Score / interval) % StarColors.Count;
                Color[] StageColor = new Color[] { StarColors[groundNumber], StarColors[(groundNumber + 1) % StarColors.Count] * ((float)(Score % interval) / (float)interval), Color.Black * 0.3f };


                for (int i = 0; i < Enemies.Count; i++)
                    Enemies[i].Draw(StageColor);

                for (int i = 0; i < sinEnemies.Count; i++)
                    sinEnemies[i].Draw(StageColor);

                for (int i = 0; i < HealEnemies.Count; i++)
                    HealEnemies[i].Draw(Color.Yellow);

                for (int i = 0; i < floorEnemies.Count; i++)
                    floorEnemies[i].Draw(StageColor);

                for (int i = 0; i < bigEnemies.Count; i++)
                    bigEnemies[i].Draw(StageColor);




                Player.Draw(Color.White);

                Color FadeColor = Color.White * 0.4f;
                if (Starchecker == 0)
                    Fader.Add(new Gfx2D(Player.Bound), (5 - jumpcount) * 5, FadeColor);



                if (Starchecker > 0)
                {
                    Color StarColor = StandAlone.RandomPick(StarColors);
                    Player.Draw(StarColor);
                    Fader.Add(new Gfx2D(Player.Bound), 15, StarColor * 0.4f);
                }

             
                Fader.DrawAll();
                if (Damagechecker > 0)
                {
                    Player.Draw(Color.White);
                    Player.Draw(DamageColor * (Damagechecker / (float)DamagecheckerMax));
                }
                Ground.Draw(StarColors[groundNumber], StarColors[(groundNumber + 1) % StarColors.Count] * ((float)(Score % interval) / (float)interval), Color.Black * 0.3f);
                StarBarBG.Draw(Color.Black);
                HpBar.Draw(Color.White);
                if (healstack >= healstackMax)
                    HpBar.Draw(StandAlone.RandomPick(StarColors));
                StarBar.Draw(Color.LightYellow);

                healstackBar.Draw(Color.LightGreen);

                if(TutorialState>=5)
                    StandAlone.DrawString(30, "Elapsed Time : " + (Score / 60).ToString() + "s", new Point(600, 430), Color.White);

                if(TutorialState<5||StandAlone.FrameTimer<1000)
                    Description.Draw(Color.White);

                if (TutorialState == 1)
                    StandAlone.DrawString(20, "Air Jump :" + Math.Max(0, jumpcount - 1), new REMOPoint(450, 180), Color.White);
                
            });
    }

    public static class ScoreBoard
    {
        public static List<Tuple<string, int>> ScoreSet = new List<Tuple<string, int>>();
        public static Scripter ScoreReader = new Scripter();
        private static string _name;
        private static int _score;

        public static void Init()
        {
            TxtEditor.MakeTextFile("Data", "Score");
            ScoreReader.AddRule("Name", (s) => { _name = s; });
            ScoreReader.AddRule("Score", (s) => 
            { 
                _score = Int32.Parse(s);
                ScoreSet.Add(new Tuple<string, int>(_name, _score));           
            });
        }
    }


    public class EnemySet
    {
        public List<Gfx2D> Enemies = new List<Gfx2D>();
        public Action<int> MoveAction;
        public Action<int> IntersectAction;
        public Action GenAction;
        public static readonly int Atk=400;
        public int GenTimer = 0;

        public bool RemoveEnemy = false;

        public EnemySet(Action<int> moveAction, Action<int> intersectAction, Action genAction)
        {
            MoveAction = moveAction;
            IntersectAction = intersectAction;
            GenAction = genAction;
        }

        public void Update()
        {
            if (GenTimer > 0)
                GenTimer--;
            else
            {
                GenAction();
            }

            //Intersect Action
            for (int i = 0; i < Enemies.Count; i++)
            {
                MoveAction(i);
                if (Rectangle.Intersect(Enemies[i].Bound, PlayerClass.Player.Bound) != Rectangle.Empty && PlayerClass.CurrentEnemy != Enemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                {
                    IntersectAction(i);
                    if(RemoveEnemy)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                        RemoveEnemy = false;
                        continue;
                    }
                }
                if (Enemies[i].Pos.X < -500)
                {
                    Enemies.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(params Color[] c)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Draw(c);
            }
        }
    }

    public static class EnemyClass
    {
        public static int Heal = 300;



        public static EnemySet Enemies = new EnemySet((i) => {
            Enemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
            Enemies.Enemies[i].Zoom(Enemies.Enemies[i].Center, 1.001f);
        }, (i) => {
            if(!PlayerClass.isFlickering&&PlayerClass.DamageTimer==0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk*3 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            Enemies.GenTimer = StandAlone.Random(20, 30);
            Enemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), StandAlone.Random(2, 10), StandAlone.Random(2, 10)))); // 적들을 생성합니다.
        });

        public static EnemySet HealEnemies = new EnemySet((i) => {
            HealEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
        }, (i) => {
            PlayerClass.healstack += 1;
            if (PlayerClass.healstack != PlayerClass.healstackMax)
                MusicBox.PlaySE("SE2");
            PlayerClass.player_hp += Heal;
            HealEnemies.RemoveEnemy = true;
        }, () => {
            HealEnemies.GenTimer = StandAlone.Random(50, 100) + Math.Min(StandAlone.FrameTimer / 40, 120);
            HealEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 350), 20, 20))); // 적들을 생성합니다.

        });

        public static EnemySet FloorEnemies = new EnemySet((i) =>
        {
           FloorEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
           FloorEnemies.Enemies[i].Zoom(FloorEnemies.Enemies[i].Center, 1.001f);
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk*3 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            FloorEnemies.GenTimer = StandAlone.Random(50, 100);
            FloorEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, 390, StandAlone.Random(5,10), StandAlone.Random(5, 10)))); // 적들을 생성합니다.
        });

        

    }

    public static class PlayerClass
    {
        public static Gfx2D Player = new Gfx2D(new Rectangle(200, 250, 40, 40));
        public static readonly int PlayerHpMax = 3000;
        public static int player_hp = PlayerHpMax;

        public static Gfx CurrentEnemy;
        public static int DamageTimer = 0; // 타격을 받았을 때 빨개지는 시간을 측정하는 타이머입니다.
        public static int DamageTimerMax = 30;
        public static Color DamageColor = Color.Red;

        public static Vector2 v = new Vector2(0, -1f);//물체속도
        public static Vector2 g = new Vector2(0, 1f);//중력 가속도

        public static int JumpCount = 0;
        public static int LongjumpMax = 2; //처음 2회의 점프는 높게 뜁니다.
        public static int JumpMax = 5;// 총 5회 점프 가능

        public static int StarTimer = 0; // 별상태 시간
        public static int StarTimerMax = 300;

        public static int healstack = 0;
        public static readonly int healstackMax = 4;
        public static bool isFlickering = false;


        public static void Init()
        {
            player_hp = PlayerHpMax;
        }
    
        public static void Update()
        {


            //Process Movement
             
            Player.Pos += v.ToPoint();
            v += g;//The object affected by gravity.

            if (User.JustPressed(Keys.Space) || User.JustPressed(Keys.Up))
            {
                if (JumpCount < LongjumpMax + 1)
                {
                    v = new Vector2(0, -14);
                    JumpCount += 1;
                }
                else if (JumpCount > LongjumpMax && JumpCount < JumpMax)
                {
                    v = new Vector2(0, -10);
                    JumpCount += 1;
                }
            }

            if (User.Pressing(Keys.Down))
            {
                v = new Vector2(0, 20);
            }


            //Under the ground problem
            if (Player.Pos.Y > InGameInterface.Ground.Pos.Y - Player.Bound.Height)
            {
                Player.Pos = new Point(Player.Pos.X, InGameInterface.Ground.Pos.Y - Player.Bound.Height);
                JumpCount = 0;
            }

            if (DamageTimer > 0)
                DamageTimer--;


            //invincible mode

            if (healstack >= healstackMax && StarTimer > 0)
            {
                if (StarTimer == 299)
                    MusicBox.PlaySE("SE");

                player_hp += 2;
                isFlickering = true;
                StarTimer--;
                JumpMax = 10;
                LongjumpMax = 5;
            }

            //Go back to Usual mode
            if (StarTimer <= 0)
            {

                LongjumpMax = 2;
                JumpMax = 5;
                isFlickering = false;
                healstack = 0;
                StarTimer = 300;
            }

            if (StarTimer == 300)
            {
                player_hp -= 1;
            }

        }

        public static void Draw()
        {
            Player.Draw(Color.White);


            //Afterimage effect
            Color FadeColor = Color.White * 0.4f;
            if (StarTimer == 0)
                Fader.Add(new Gfx2D(Player.Bound), (5 - JumpCount) * 5, FadeColor);

            if (isFlickering)
            {
                Color StarColor = StandAlone.RandomPick(InGameInterface.StarColors);
                Player.Draw(StarColor);
                Fader.Add(new Gfx2D(Player.Bound), 15, StarColor * 0.4f);
            }

            //Damage Effect 
            if (DamageTimer > 0)
            {
                Player.Draw(Color.White);
                Player.Draw(DamageColor * (DamageTimer / (float)DamageTimerMax));
            }

        }
    }

    public static class InGameInterface
    {
        public static List<Color> StarColors = new List<Color>(new Color[] { Color.DeepPink, Color.Yellow, Color.Blue, Color.DarkRed, Color.LightPink, Color.LightYellow, Color.BlueViolet, Color.Aqua, Color.DarkCyan, Color.Magenta });
        public static Gfx2D Ground = new Gfx2D(new Rectangle(0, 390, 1400, 500));
        public static Color[] StageColor;
        public static int Score;

        public static SimpleGauge HpGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 420, 300, 10)));
        public static SimpleGauge StarGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 480, 270, 10)));


        public static void Init()
        {
            Score = 0;
            StandAlone.FrameTimer = 0;
            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);

        }

        public static void Update()
        {
            
            Score++;

            //Set Stage Color
            int interval = 3000;
            int groundNumber = (Score / interval) % StarColors.Count;
            StageColor = new Color[] { StarColors[groundNumber], StarColors[(groundNumber + 1) % StarColors.Count] * ((float)(Score % interval) / (float)interval), Color.Black * 0.3f };

            HpGauge.Update(PlayerClass.player_hp,PlayerClass.PlayerHpMax);
            StarGauge.Update(PlayerClass.StarTimer, PlayerClass.StarTimerMax);
        }

        public static void Draw()
        {
            Ground.Draw(StageColor);
            HpGauge.Draw(Color.White);
            Filter.Absolute(StarGauge.MaxBound, Color.Black);
            StarGauge.Draw(Color.LightYellow);
        }

    }


    public static class TestClass
    {
        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            scn.bgm = "ChanceOnFaith";
        }, () =>
        {
            InGameInterface.Update();
            PlayerClass.Update();
            EnemyClass.Enemies.Update();
            EnemyClass.HealEnemies.Update();
            EnemyClass.FloorEnemies.Update();
            //잔상은 뒤로 이동
            foreach (Color c in Fader.FadeAnimations.Keys)
            {
                foreach (Gfx g in Fader.FadeAnimations[c].Keys)
                {
                    g.MoveByVector(new Point(-1, 0), 10);
                }
            }
            if (player_hp <= 0)
            {
                Projectors.Projector.PauseAll();
                Projectors.Projector.Load(GameOverScene.scn);
            }


        }, () =>
        {
            EnemyClass.Enemies.Draw(InGameInterface.StageColor);
            EnemyClass.FloorEnemies.Draw(InGameInterface.StageColor);
            EnemyClass.HealEnemies.Draw(Color.Yellow);
            PlayerClass.Draw();
            InGameInterface.Draw();
            Fader.DrawAll();

            StandAlone.DrawString(PlayerClass.player_hp.ToString(), new REMOPoint(0, 0), Color.White);
        });

    }


}
