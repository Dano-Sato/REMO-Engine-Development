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
using REMO_Engine_Developer;

namespace REMO_Engine_Developer
{
    public static class ExampleScene1 // 사각형이 튕기는 씬입니다.
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 200, 50, 50));
        public static Gfx2D sqr2 = new Gfx2D(new Rectangle(200, 400, 300, 20));

        public static Vector2 v = new Vector2(0, 0);
        public static Vector2 g = new Vector2(0, 0.2f);
        public static double speed = 0;
        public static void MoveSquare() => sqr.Pos += v;


        public static Scene scn = new Scene(
            () =>
            {

            },
            () =>
            {
                v += g;//이 물체는 중력을 받는다.

                MoveSquare();//속도벡터에 의해 물체를 움직인다.
                if (Rectangle.Intersect(sqr.Bound, sqr2.Bound) != Rectangle.Empty)//충돌 판정
                {
                    v = -v;//바에 충돌할 경우, 속도벡터가 바뀐다.
                    MoveSquare();
                }
            },
            () =>
            {
                sqr.Draw(Color.White);
                sqr2.Draw(Color.Red);

            }
            );

    }

    public static class ExampleGame1 // 점프하며 사각형들을 피하는 게임입니다.
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(50, 300, 50, 50));
        public static Vector2 v = new Vector2(0, 0);
        public static Vector2 g = new Vector2(0, 1);
        public static Gfx2D Ground = new Gfx2D(new Rectangle(0, 350, 1000, 350));
        public static void MoveSqr() => sqr.Pos += v;
        public static List<Gfx> Enemies = new List<Gfx>();
        public static int Score = 0;
        public static int GameOverTimer = 0;
        public static Scene scn = new Scene(() => { }, () =>
        {
            if (GameOverTimer > 0)
                GameOverTimer--;
            if (sqr.Pos.Y < 300)//스퀘어가 공중에 떴을 때
                v += g;
            MoveSqr();
            if (sqr.Pos.Y > 300)
                sqr.Pos = new Point(50, 300);//스퀘어는 바닥을 뚫을 수 없다.
            if (sqr.Pos.Y < 0)
                sqr.Pos = new Point(50, 0);//스퀘어는 천장을 뚫을 수 없다.

            if (User.Pressing(Keys.Space))
                v = new Vector2(0, -15);
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].MoveByVector(new Point(-1, 0), 3 + 0.1 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                if (Rectangle.Intersect(Enemies[i].Bound, sqr.Bound) != Rectangle.Empty)//적과 부딪치면 스코어가 초기화됩니다.
                {
                    Score = 0;
                    GameOverTimer = 30;
                }

                if (Enemies[i].Pos.X < -30)//이미 지나간 적은 제거되어 점수가 됩니다.
                {
                    Enemies.RemoveAt(i);
                    i--;
                    Score += 10;
                }
            }
            if (StandAlone.FrameTimer % Math.Max(20, 70 - StandAlone.FrameTimer / 100) == 0)
            {
                Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30, 30))); // 적들을 생성합니다.
            }




        }, () =>
        {
            sqr.Draw(Color.White, Color.Red * (GameOverTimer * 0.1f));
            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].Draw(Color.White, Color.Red * GameOverTimer * 0.1f);
            Ground.Draw(Color.White, Color.Red * GameOverTimer * 0.1f);
            StandAlone.DrawString("SCORE : " + Score, new Point(200, 400), Color.Black);
        });


    }
    public static class FlickerScene
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(0, 300, 5, 5));
        public static Gfx2D sqr2 = new Gfx2D(new Rectangle(500, 400, 100, 100));
        public static Gfx2D sqr3 = new Gfx2D(new Rectangle(300, 400, 100, 100));
        public static Scene scn = new Scene(() => {
        }, () => {
            sqr.Pos += new Point(1, 0);
            //sqr.MoveByVector(new Point(1,0), 1.0);
            sqr.Pos = new REMOPoint(sqr.Pos.X, (int)(-Fader.Flicker(100) * 100) + 300);
            Fader.Add(new Gfx2D(sqr.Bound), 1000, Color.White);
        }, () => {
            sqr.Draw(Color.White);
            sqr2.Draw(Color.White, Color.Red * Fader.Flicker(100));
            sqr3.Draw(Color.White, Color.Green * Fader.Flicker(100));
            Fader.Draw(Color.White);
        });
    }
    
}


namespace SungHo_REMO_TestGame
{
    public static class SungHoScene
    {
        public static Gfx2D sqr = new Gfx2D(new Rectangle(200, 350, 40, 40));
        public static Gfx2D sqr1 = new Gfx2D(new Rectangle(0, 390, 1400, 500));
        public static Gfx2D sqrbg = new Gfx2D(new Rectangle(0, 0, 1200, 700));
        public static Vector2 v = new Vector2(0, -1f);//물체속도
        public static Vector2 g = new Vector2(0, 1f);//중력 가속도
        public static void MoveSquare() => sqr.Pos += v;
        public static List<Gfx> Enemies = new List<Gfx>();
        public static List<Gfx> HealEnemies = new List<Gfx>();
        public static List<Gfx> sinEnemies = new List<Gfx>();
        public static List<Gfx> floorEnemies = new List<Gfx>();
        public static List<Gfx> upEnemies = new List<Gfx>();
        public static List<Gfx> bigEnemies = new List<Gfx>();
        public static int jumpcount = 0;
        public static int GameOverTimer = 0;
        public static int starTimer = 300;

        public static int Score = 0;
        public static double wintojump = 1;
        public static readonly double Initial_WintoJump = 1.5;

        public static readonly int PlayerHpMax = 3000;

        public static readonly int healstackMax = 4;
        public static int player_hp = PlayerHpMax;
        public static int healstack = 0;
        public static int Atk = 400;
        public static int Heal = 300;

        public static Dictionary<Gfx, int> Enemyatk = new Dictionary<Gfx, int>();//EnemyHPs.Add(Enemies[i],5);

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
        public static Gfx2D WintojumpBar = new Gfx2D(new Rectangle(0, 450, 0, 10));
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


        public static Scene scn = new Scene(() =>
        {
            /*foreach( Gfx g in EnemyHPs.Keys)
            {
                EnemyHPs[g]--;
            }
            /*
            for(int i=0;i<EnemyHPs.Keys.ToList().Count;i++)
            {
                EnemyHPs[EnemyHPs.Keys.ToList()[i]]--;
            }*/
            Enemies.Clear();
            HealEnemies.Clear();
            sinEnemies.Clear();
            floorEnemies.Clear();
            upEnemies.Clear();
            bigEnemies.Clear();
            jumpcount = 0;
            healstack = 0;
            GameOverTimer = 0;
            Score = 0;
            wintojump = Initial_WintoJump;
            starTimer = 300;
            player_hp = PlayerHpMax;
            Damagechecker = 0;
            Starchecker = 0;
            StarBarBG.Bound = new Rectangle(StarBarBG.Pos, new Point(270, 10));
            //scn.bgm = "TestWav";
            LongjumpMax = 2;
            JumpMax = 5;


            StandAlone.FrameTimer = 0;
            StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
        },
            () =>
            {
                if (GameOverTimer > 0)
                    GameOverTimer--;
                MoveSquare();//square moves by velocity vector.
                v += g;//The object affected by gravity.

                if (sqr.Pos.Y > sqr1.Pos.Y - sqr.Bound.Height)
                {
                    sqr.Pos = new REMOPoint(sqr.Pos.X, sqr1.Pos.Y - sqr.Bound.Height);
                    jumpcount = 0;
                    wintojump -= 1.0 / 60;
                }
                else
                    wintojump = Initial_WintoJump;

                Score += 10;
                if (User.JustPressed(Keys.Space))
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

                if (Math.Abs(wintojump) < 0.00001)
                {
                    Damagechecker = DamagecheckerMax;

                    player_hp = player_hp / 2;
                    wintojump = Initial_WintoJump;
                }

                //무적 
                if (healstack >= healstackMax && starTimer > 0)
                {
                    player_hp += 2;
                    healstackBar.Bound = new Rectangle(healstackBar.Pos, new Point(0, 10));
                    Starchecker = 1;
                    starTimer -= 1;
                    wintojump = 0.8;
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
                    if (Rectangle.Intersect(HealEnemies[i].Bound, sqr.Bound) != Rectangle.Empty)//적과 부딪치면 hp가 답니다.
                    {
                        healstack += 1;
                        player_hp += Heal;
                        HealEnemies.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                }
                for (int i = 0; i < sinEnemies.Count; i++)//적
                {
                    sinEnemies[i].MoveByVector(new Point(-10, 0), 10 + 7 * (StandAlone.Random(-1, 2)));
                }
                for (int i = 0; i < floorEnemies.Count; i++)
                {
                    floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                }
                /*for (int i = 0; i < upEnemies.Count; i++)
                {
                    upEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                    upEnemies[i].MoveByVector(new Point(0, -10), 10 + 7 * (StandAlone.Random(-400, 400) / 100));
                    upEnemies[i].MoveByVector(new Point(0, 10), 10 + 7 * (StandAlone.Random(-400, 400) / 100));
                }*/
                for (int i = 0; i < bigEnemies.Count; i++)
                {
                    bigEnemies[i].MoveByVector(new Point(-10, 0), 5 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                }
                //피격판정

                if (starTimer >= 300)
                {
                    if (Damagechecker == 0)
                    {
                        //적 
                        for (int i = 0; i < Enemies.Count; i++)
                        {
                            //Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
                            if (Rectangle.Intersect(Enemies[i].Bound, sqr.Bound) != Rectangle.Empty && CurrentEnemy != Enemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = Enemies[i];
                                player_hp -= Atk;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Red;
                            }
                        }

                        //sin 적 
                        for (int i = 0; i < sinEnemies.Count; i++)//적
                        {
                            //sinEnemies[i].MoveByVector(new Point(-10, 0), 10 + 7 * (StandAlone.Random(-400, 400) / 100));
                            /*sinEnemies[i].Pos = new Point(sinEnemies[i].Pos.X, (int)(Math.Sin(StandAlone.FrameTimer / 10.0) * 100)); 
                            sinEnemies[i].MoveByVector(new Point(0, -10), 10 + 7 * (StandAlone.Random(-10, 10) / 100));//적들이 이상합니다*/
                            if (Rectangle.Intersect(sinEnemies[i].Bound, sqr.Bound) != Rectangle.Empty && CurrentEnemy != sinEnemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                            {
                                CurrentEnemy = sinEnemies[i];
                                player_hp -= Atk;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Blue;
                            }
                        }
                        //바닥 적 
                        for (int i = 0; i < floorEnemies.Count; i++)
                        {
                            //floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                            if (Rectangle.Intersect(floorEnemies[i].Bound, sqr.Bound) != Rectangle.Empty && CurrentEnemy != floorEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = floorEnemies[i];
                                player_hp -= Atk;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Green;

                            }
                        }

                        /*for (int i = 0; i < upEnemies.Count; i++)
                        {
                            //floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                            if (Rectangle.Intersect(upEnemies[i].Bound, sqr.Bound) != Rectangle.Empty && CurrentEnemy != upEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = upEnemies[i];
                                player_hp -= 10;
                                Damagechecker = DamagecheckerMax;
                                DamageColor = Color.Yellow;
                            }
                        }*/

                        for (int i = 0; i < bigEnemies.Count; i++)
                        {
                            //floorEnemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
                            if (Rectangle.Intersect(bigEnemies[i].Bound, sqr.Bound) != Rectangle.Empty && CurrentEnemy != bigEnemies[i])//적과 부딪치면 hp가 답니다.
                            {
                                CurrentEnemy = bigEnemies[i];
                                player_hp -= Atk;
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

                //사인 적 생성
                if (sinCoolTime > 0)
                    sinCoolTime--;
                else
                {
                    sinCoolTime = StandAlone.Random(60, 80);
                    sinEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 350), 30, 30))); // 적들을 생성합니다.
                }

                //힐 적 생성 
                if (HpCoolTime > 0)
                    HpCoolTime--;
                else
                {
                    HpCoolTime = StandAlone.Random(150, 200);
                    HealEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 350), 20, 20))); // 적들을 생성합니다.
                }

                //바닥적 생성
                if (floorCoolTime > 0)
                    floorCoolTime--;
                else
                {
                    floorCoolTime = StandAlone.Random(100, 250);
                    floorEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(370, 375), 50, 20))); // 적들을 생성합니다.
                }

                //위아래 적 생성
                /*if (upCoolTime > 0)
                    upCoolTime--;
                else
                {
                    upCoolTime = StandAlone.Random(100, 300);
                    upEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 250), 25, 25))); // 적들을 생성합니다.
                }*/

                //큰 적 생성 
                if (bigCoolTime > 0)
                    bigCoolTime--;
                else
                {
                    bigCoolTime = StandAlone.Random(600, 900);
                    bigEnemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 150), 120, 120))); // 적들을 생성합니다.
                }


                HpBar.Bound = new Rectangle(HpBar.Pos, new Point(BarLength * player_hp / PlayerHpMax, 10));
                WintojumpBar.Bound = new Rectangle(WintojumpBar.Pos, new Point((int)(jumpBarLength * wintojump / Initial_WintoJump), 10));

                if (player_hp > 9000)
                    player_hp = 9000;



            },
            () =>
            {
                List<Color> StarColors = new List<Color>(new Color[] { Color.DeepPink, Color.Yellow, Color.Blue, Color.DarkRed, Color.LightPink, Color.LightYellow, Color.BlueViolet, Color.Aqua, Color.DarkCyan, Color.Magenta });

                sqrbg.Draw(Color.Black);
                if (Starchecker > 0)
                    sqrbg.Draw(StandAlone.RandomPick(StarColors) * 0.4f);
                StandAlone.DrawString(jumpcount.ToString(), new Point(0, 0), Color.Red, Color.White);
                StandAlone.DrawString(Score.ToString(), new Point(0, 20), Color.Red, Color.White);
                StandAlone.DrawString(healstack.ToString(), new Point(30, 0), Color.Red, Color.White);

                for (int i = 0; i < Enemies.Count; i++)
                    Enemies[i].Draw(Color.White, Color.Red * GameOverTimer * 0.1f);

                for (int i = 0; i < sinEnemies.Count; i++)
                    sinEnemies[i].Draw(Color.Orange, Color.Red * GameOverTimer * 0.1f);

                for (int i = 0; i < HealEnemies.Count; i++)
                    HealEnemies[i].Draw(Color.Yellow, Color.Red * GameOverTimer * 0.1f);

                for (int i = 0; i < floorEnemies.Count; i++)
                    floorEnemies[i].Draw(Color.White, Color.Red * GameOverTimer * 0.1f);

                for (int i = 0; i < bigEnemies.Count; i++)
                    bigEnemies[i].Draw(Color.RosyBrown, Color.Red * GameOverTimer * 0.1f);




                sqr.Draw(Color.White);

                Color FadeColor = Color.White * 0.4f;
                if (Starchecker == 0)
                    Fader.Add(new Gfx2D(sqr.Bound), (5 - jumpcount) * 5, FadeColor);



                if (Starchecker > 0)
                {
                    Color StarColor = StandAlone.RandomPick(StarColors);
                    sqr.Draw(StarColor);
                    Fader.Add(new Gfx2D(sqr.Bound), 15, StarColor * 0.4f);
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
                    sqr.Draw(Color.White);
                    sqr.Draw(DamageColor * (Damagechecker / (float)DamagecheckerMax));
                }
                sqr1.Draw(Color.Red);
                StarBarBG.Draw(Color.Black);
                HpBar.Draw(Color.White);
                if (healstack >= healstackMax)
                    HpBar.Draw(StandAlone.RandomPick(StarColors));
                WintojumpBar.Draw(Color.White);
                StarBar.Draw(Color.LightYellow);

                healstackBar.Draw(Color.LightGreen);
            });
    }




    public static class GameOverScene
    {
        public static GfxStr GameOverString = new GfxStr("Game Over. Press R to Restart", new Point(200, 200));
        public static Scene scn = new Scene(() => {
        }, () => {
            if (User.JustPressed(Keys.R))
            {
                Projectors.Projector.SwapTo(SungHoScene.scn);
            }
        }, () => {
            GameOverString.Draw(Color.White);
        });
    }
}