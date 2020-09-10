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
using System.Security.Cryptography;
using REMOEngine;
using System.Runtime.ExceptionServices;

namespace FlickerGame
{
  
    public static class GameOverScene
    {
        public static GfxStr GameOverString = new GfxStr("Game Over. Press R to Restart", new Point(200, 200));
        public static Scene[] SceneList = new Scene[] { TutorialStage.scn, Stage2.scn, Stage1.scn, Stage3.scn};
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
                foreach(Scene s in GameOverScene.SceneList)
                {
                    if (Projectors.Projector.Loaded(s))
                        Projectors.Projector.SwapTo(s);
                }
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
            /*Excluded 
            if(User.JustPressed(Keys.Escape))
            {
                Projectors.Projector.Unload(PauseScene.scn);
                Projectors.Projector.ResumeAll();
                foreach(Scene s in GameOverScene.SceneList)
                {
                    if(Projectors.Projector.Loaded(s))
                    {
                        MusicBox.PlaySong(s.bgm);
                    }
                }
            }*/
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
            new string[] { "New Game", "Tutorial", "Settings", "Exit" },
            () =>
            {
                ShowMainMenu = false;
            },
            () =>
            {
                Projectors.Projector.SwapTo(TutorialStage.scn);
            }
            ,
            () =>
            {
                Projectors.Projector.SwapTo(SettingScene.scn);
            }
            ,
            () =>
            {
                Game1.GameExit = true;
            }

            );

        public static bool ShowMainMenu = true;

        public static SimpleMenu SubMenus = new SimpleMenu(20, new REMOPoint(800, 200), new REMOPoint(0, 60),
            new string[] { "Stage 1", "Stage 2", "Stage 3", "Go Back" },
            () =>
            {
                Projectors.Projector.SwapTo(Stage1.scn);
            },
            () =>
            {
                Projectors.Projector.SwapTo(Stage2.scn);
            }
            ,
            ()=>
            {
                Projectors.Projector.SwapTo(Stage3.scn);
            },
            () =>
            {
                ShowMainMenu = true;
            }
            );
                
        public static Scene scn = new Scene(() =>
        {
            MusicBox.Mode = MusicBoxMode.FadeOut;
            BigSquare.Center = new REMOPoint(200, 200);
            Functions.SetScreen();
            scn.bgm = "Journey";
            scn.InitOnce(() =>
            {
                ScoreBoard.Init();
                SettingScene.BackgroundFlick.isChecked = true;
            });


        }, () =>
        {
            BigSquare.Rotate += 0.01f;
            if (ShowMainMenu)
                MainMenus.Update();
            else
                SubMenus.Update();
        }, () =>
        {
            Filter.Absolute(StandAlone.FullScreen, Color.White);
            Filter.Absolute(StandAlone.FullScreen, Color.Black*Fader.Flicker(300));
            BigSquare.Draw(Color.Black, Color.White*Fader.Flicker(300));
            StandAlone.DrawString(40, "Flicker", new REMOPoint(200, 100), Color.White);
            StandAlone.DrawString(40, "Flicker", new REMOPoint(200, 100), Color.Black*Fader.Flicker(300));
            if(ShowMainMenu)
                MainMenus.Draw(Color.Black, Color.White * Fader.Flicker(300));
            else
                SubMenus.Draw(Color.Black, Color.White * Fader.Flicker(300));
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
    
   

  
    public class EnemySet
    {
        public List<Gfx2D> Enemies = new List<Gfx2D>();
        public Action<int> MoveAction;
        public Action<int> IntersectAction;
        public Action GenAction;
        public static readonly int Atk = 400;
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
                    if (RemoveEnemy)
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

        public void RUpdate() // 회전하는 적일경우의 업데이트
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
                if (Enemies[i].RContains(PlayerClass.Player.Center) && PlayerClass.CurrentEnemy != Enemies[i])//적과 부딪치면 hp가 답니다. 충돌판정
                {
                    IntersectAction(i);
                    if (RemoveEnemy)
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

    public static class Stage2_Enemies
    {
        public static int Heal = 300;



        public static EnemySet Enemies = new EnemySet((i) => {
            Enemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
            Enemies.Enemies[i].Zoom(Enemies.Enemies[i].Center, 1.001f);
        }, (i) => {
            if(!PlayerClass.isFlickering&&PlayerClass.DamageTimer==0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk*2 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            Enemies.GenTimer = StandAlone.Random(30,40);
            Enemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), StandAlone.Random(2, 10), StandAlone.Random(2, 10)))); // 적들을 생성합니다.
        });

       
        public static EnemySet FloorEnemies = new EnemySet((i) =>
        {
           FloorEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
           FloorEnemies.Enemies[i].Zoom(FloorEnemies.Enemies[i].Center, 1.001f);
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk*2 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            FloorEnemies.GenTimer = StandAlone.Random(50, 100);
            FloorEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, 390, StandAlone.Random(5,10), StandAlone.Random(5, 10)))); // 적들을 생성합니다.
        });

        public static EnemySet BigEnemies = new EnemySet((i) =>
        {
            BigEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
            BigEnemies.Enemies[i].Zoom(FloorEnemies.Enemies[i].Center, -1.002f);
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk * 2 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            BigEnemies.GenTimer = StandAlone.Random(50, 100);
            BigEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, 390, StandAlone.Random(5, 10), StandAlone.Random(5, 10)))); // 적들을 생성합니다.
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
        public static int JumpMax = 4;// 총 5회 점프 가능

        public static int StarTimer = 0; // 별상태 시간
        public static int StarTimerMax = 300;

        public static int healstack = 0;
        public static readonly int healstackMax = 4;
        public static bool isFlickering = false;


        public static void Init()
        {
            player_hp = PlayerHpMax;
            healstack = 0;
            StarTimer = 300;
            isFlickering = false;
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
                JumpMax = 4;
                isFlickering = false;
                healstack = 0;
                StarTimer = 300;
            }

            if (StarTimer == 300)
            {
                player_hp -= 1;
            }

        }

        public static void Draw(Color c)
        {
            Player.Draw(c);


            //Afterimage effect
            Color FadeColor = c * 0.4f;

            if (isFlickering)
            {
                Color StarColor = StandAlone.RandomPick(InGameInterface.StarColors);
                Player.Draw(StarColor);
                Fader.Add(new Gfx2D(Player.Bound), 15, StarColor * 0.4f);
            }
            else
                Fader.Add(new Gfx2D(Player.Bound), (5 - JumpCount) * 5, FadeColor);


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
        public static SimpleGauge HealStackGauge = new SimpleGauge(new Gfx2D(new Rectangle(0, 480, 270, 10)));


        public static void Init()
        {
            Score = 0;
            StandAlone.FrameTimer = 0;
            Functions.SetScreen();
            StageColor = new Color[] { Color.Black };

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
            HealStackGauge.Update(PlayerClass.healstack, PlayerClass.healstackMax);
        }

        public static void Draw()
        {
            if (PlayerClass.isFlickering&&SettingScene.BackgroundFlick)
            {
                Filter.Absolute(StandAlone.FullScreen, StandAlone.RandomPick(StarColors) * 0.2f);
            }
            Ground.Draw(StageColor);
            HpGauge.Draw(Color.Orange);
            if(PlayerClass.isFlickering)
                HpGauge.Draw(StandAlone.RandomPick(StarColors));
            Filter.Absolute(StarGauge.MaxBound, Color.Black);
            if (PlayerClass.isFlickering)
                StarGauge.Draw(Color.LightYellow);
            else
                HealStackGauge.Draw(Color.LightGreen);
            StandAlone.DrawString(30, "Elapsed Time : " + (InGameInterface.Score / 60).ToString() + "s", new Point(600, 430), Color.White);
        }

    }


    public static class Stage2
    {
        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            Stage2_Enemies.Enemies.Enemies.Clear();
            Stage1_Enemies.HealEnemies.Enemies.Clear();
            Stage2_Enemies.FloorEnemies.Enemies.Clear();
            scn.bgm = "Journey";

        }, () =>
        {
            Functions.GameUpdate();
            Stage2_Enemies.Enemies.Update();
            Stage1_Enemies.HealEnemies.Update();
            Stage2_Enemies.FloorEnemies.Update();
         


        }, () =>
        {
            Stage2_Enemies.Enemies.Draw(InGameInterface.StageColor);
            Stage2_Enemies.FloorEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);
            Functions.GameDraw(Color.Orange);

        });

    }

    public static class Stage1_Enemies
    {
        public static int Heal = 300;



        public static EnemySet Enemies = new EnemySet((i) => {
            Enemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            Enemies.GenTimer = StandAlone.Random(13, 25);
            Enemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30,30))); // 적들을 생성합니다.
        });

        public static EnemySet HealEnemies = new EnemySet((i) => {
            HealEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
            HealEnemies.Enemies[i].Rotate += 0.1f;

        }, (i) => {
            PlayerClass.healstack += 1;
            if (PlayerClass.healstack != PlayerClass.healstackMax)
                MusicBox.PlaySE("SE2");
            PlayerClass.player_hp += Heal;
            HealEnemies.RemoveEnemy = true;
        }, () => {
        HealEnemies.GenTimer = StandAlone.Random(50, 100) + Math.Min(StandAlone.FrameTimer / 40, 120);
        Gfx2D g = new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 350), 20, 20));
        HealEnemies.Enemies.Add(g); // 적들을 생성합니다.

        });

        public static EnemySet FloorEnemies = new EnemySet((i) =>
        {
            FloorEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = FloorEnemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            FloorEnemies.GenTimer = StandAlone.Random(100, 250);
            FloorEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(370, 375), 50, 20))); // 적들을 생성합니다.
        });
        public static EnemySet SinEnemies = new EnemySet((i) => {
            SinEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 7 * (StandAlone.Random(-1, 2)));
        }, (i) => {
            PlayerClass.CurrentEnemy = SinEnemies.Enemies[i];
            PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
            PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
            PlayerClass.DamageColor = Color.Red;
        }, () => {
            SinEnemies.GenTimer = StandAlone.Random(60, 80);
            SinEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 350), 30, 30))); // 적들을 생성합니다.
        });

        public static EnemySet BigEnemies = new EnemySet((i) => {
            BigEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 5 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
        }, (i) => {
            PlayerClass.CurrentEnemy = BigEnemies.Enemies[i];
            PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
            PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
            PlayerClass.DamageColor = Color.Red;
        }, () => {
            BigEnemies.GenTimer = StandAlone.Random(600, 900);
            BigEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(100, 150), 120, 120))); // 적들을 생성합니다.
        });



    }

    public static class Stage1
    {
        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            scn.bgm = "SummerNight";
            Stage1_Enemies.Enemies.Enemies.Clear();
            Stage1_Enemies.HealEnemies.Enemies.Clear();
            Stage1_Enemies.FloorEnemies.Enemies.Clear();
            Stage1_Enemies.SinEnemies.Enemies.Clear();


        }, () =>
        {
            Functions.GameUpdate();
            Stage1_Enemies.Enemies.Update();
            Stage1_Enemies.HealEnemies.Update();
            if (StandAlone.FrameTimer > 500)
                Stage1_Enemies.FloorEnemies.Update();
            if (StandAlone.FrameTimer > 2000)
                Stage1_Enemies.SinEnemies.Update();
            if (StandAlone.FrameTimer > 4000)
                Stage1_Enemies.BigEnemies.Update();



        }, () =>
        {
            Functions.GameDraw(Color.Orange);
            Stage1_Enemies.Enemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);
            Stage1_Enemies.FloorEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.SinEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.BigEnemies.Draw(InGameInterface.StageColor);

        });

    }

    public static class TutorialStage
    {
        private static int _tutorialState = -1;
        public static int TutorialState
        {
            get
            {
                return _tutorialState;
            }
            set
            {
                if (value < Descriptions.Length)
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
            "Avoid other squares.",
        };

        public static readonly int MakeHealEnemy = 3;
        public static readonly int MakeOtherEnemy = 5;

        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            scn.bgm = "SummerNight";
            Stage1_Enemies.Enemies.Enemies.Clear();
            Stage1_Enemies.HealEnemies.Enemies.Clear();
            Stage1_Enemies.FloorEnemies.Enemies.Clear();
            Stage1_Enemies.SinEnemies.Enemies.Clear();
            TutorialState = 0;

        }, () =>
        {
            Functions.GameUpdate();

            if (TutorialState >= MakeHealEnemy)
            {

                //힐 적 생성 
                Stage1_Enemies.HealEnemies.Update();


                if (TutorialState >= MakeOtherEnemy)
                {
                    Stage1_Enemies.Enemies.Update();
                    if (StandAlone.FrameTimer > 500)
                        Stage1_Enemies.FloorEnemies.Update();
                    if (StandAlone.FrameTimer > 2000)
                        Stage1_Enemies.SinEnemies.Update();
                    if (StandAlone.FrameTimer > 4000)
                        Stage1_Enemies.BigEnemies.Update();
                }


            }

            if (TutorialState == 0 && User.JustPressed(Keys.Space))
                TutorialState++;
            if (TutorialState == 1 && PlayerClass.JumpCount == 4)
                TutorialState++;
            if (TutorialState == 2 && User.JustPressed(Keys.Down) && PlayerClass.Player.Pos.Y != 350)
                TutorialState++;
            if (TutorialState == 3 && PlayerClass.healstack == PlayerClass.healstackMax)
                TutorialState++;
            if (TutorialState == 4 && PlayerClass.StarTimer == 1)
            {
                TutorialState++;
                PlayerClass.Init();
                InGameInterface.Init();
                Stage1_Enemies.Enemies.Enemies.Clear();
                Stage1_Enemies.HealEnemies.Enemies.Clear();
                Stage1_Enemies.FloorEnemies.Enemies.Clear();
                Stage1_Enemies.SinEnemies.Enemies.Clear();

            }
            if (TutorialState == 5 && StandAlone.FrameTimer == 500)
            {
                Fader.Add(Description, 100, Color.White * 0.99f);
            }

            Description.Center = new REMOPoint(500, 250);

        }, () =>
        {
            Functions.GameDraw(Color.Orange);
            Stage1_Enemies.Enemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);
            Stage1_Enemies.FloorEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.SinEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.BigEnemies.Draw(InGameInterface.StageColor);

            if (TutorialState < 5 || StandAlone.FrameTimer <= 500)
                Description.Draw(Color.White);

            if (TutorialState == 1)
                StandAlone.DrawString(20, "Air Jump :" + Math.Max(0, PlayerClass.JumpCount), new REMOPoint(450, 180), Color.White);

        });

    }

    public static class Stage3
    {
        public static EnemySet Enemies = new EnemySet((i) => {
            Enemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
            Enemies.Enemies[i].Rotate += 0.01f;
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk*3 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            Enemies.GenTimer = StandAlone.Random(25, 36);
            Gfx2D g = new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 300), 30, 120));
            g.Rotate = (float)StandAlone.Random();
            Enemies.Enemies.Add(g); // 적들을 생성합니다.
        });
        public static EnemySet BigEnemies = new EnemySet((i) =>
        {
            BigEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 8 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
            BigEnemies.Enemies[i].Rotate += 0.03f;
        }, (i) =>
        {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk * 3 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () =>
        {
            BigEnemies.GenTimer = StandAlone.Random(400, 700);
            Gfx2D g = new Gfx2D(new Rectangle(1000, StandAlone.Random(0, 200), 50, 250));
            g.Rotate = (float)StandAlone.Random();
            BigEnemies.Enemies.Add(g); // 적들을 생성합니다.
        });

        public static EnemySet ReverseEnemies = new EnemySet((i) => {
            ReverseEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 10 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
            ReverseEnemies.Enemies[i].Rotate -= 0.01f;
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = Enemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk * 3 + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            ReverseEnemies.GenTimer = StandAlone.Random(50, 100);
            Gfx2D g = new Gfx2D(new Rectangle(1000, StandAlone.Random(200, 350), 30, 120));
            g.Rotate = (float)StandAlone.Random();
            ReverseEnemies.Enemies.Add(g); // 적들을 생성합니다.
        });

        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            scn.bgm = "StillGood";
            Enemies.Enemies.Clear();
            BigEnemies.Enemies.Clear();
            ReverseEnemies.Enemies.Clear();
            Stage1_Enemies.HealEnemies.Enemies.Clear();

        }, () =>
        {
            Functions.GameUpdate();
            Enemies.RUpdate();
            Stage1_Enemies.HealEnemies.Update();
            if(StandAlone.FrameTimer>500)
                ReverseEnemies.RUpdate();
            if (StandAlone.FrameTimer>3600)
                BigEnemies.RUpdate();

        }, () =>
        {

            Enemies.Draw(InGameInterface.StageColor);
            ReverseEnemies.Draw(InGameInterface.StageColor);
            BigEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);
            Functions.GameDraw(Color.Orange);
        });

    }

    public static class Functions
    {

        public static void GameUpdate()
        {
            InGameInterface.Update();
            PlayerClass.Update();
            //잔상은 뒤로 이동
            foreach (Color c in Fader.FadeAnimations.Keys)
            {
                foreach (Gfx g in Fader.FadeAnimations[c].Keys)
                {
                    g.MoveByVector(new Point(-1, 0), 10);
                }
            }
            if (PlayerClass.player_hp <= 0)
            {
                Projectors.Projector.PauseAll();
                Projectors.Projector.Load(GameOverScene.scn);
            }

            if (User.JustPressed(Keys.Escape))
            {
                Projectors.Projector.PauseAll();
                Projectors.Projector.Load(PauseScene.scn);
            }


        }

        public static void GameDraw(Color c)
        {
            PlayerClass.Draw(c);
            InGameInterface.Draw();
            Fader.DrawAll();
            int FadeInTime = 100;
            if (StandAlone.FrameTimer < FadeInTime)
            {
                Filter.Absolute(StandAlone.FullScreen, Color.Black * (0.7f * (1-StandAlone.FrameTimer / (float)FadeInTime)));
            }

        }


        public static void SetScreen()
        {
                StandAlone.FullScreen = new Rectangle(0,0,1000, 500);
        }
    }
    public static class ScoreBoard
    {
        public static List<Tuple<string, int, int>> ScoreSet = new List<Tuple<string, int, int>>();
        public static Scripter ScoreReader = new Scripter();
        private static string _name;
        private static int _stage;
        private static int _score;

        //<Name>Blabla<Stage>1<Score>300 <-> Tuple(Blabla, 1, 300)
        public static void Init()
        {
            //스코어를 저장할 스크립트 파일 생성
            TxtEditor.MakeTextFile("Data", "Score");
            //스크립트를 읽어들이는 룰 지정
            ScoreReader.AddRule("Name", (s) => { _name = s; });
            ScoreReader.AddRule("Stage", (s) => { _stage = Int32.Parse(s); });
            ScoreReader.AddRule("Score", (s) =>
            {
                _score = Int32.Parse(s);
                AddScore(_name, _stage, _score);
            });
            //스크립트 파일을 읽어옵니다.
            ScoreReader.ReadTxt("Data", "Score");
        }
        public static void AddScore(string Name, int Stage, int Score)
        {
            ScoreSet.Add(new Tuple<string, int, int>(Name, Stage, Score));
        }
        /// <summary>
        /// 지금까지의 Score Set을 지정된 txt파일에 저장합니다.
        /// </summary>
        public static void SaveScore()
        {
            List<string> text = new List<string>();
            foreach (Tuple<string, int, int> t in ScoreSet)
            {
                Dictionary<string, string> line = new Dictionary<string, string>();
                line.Add("Name", t.Item1);
                line.Add("Stage", t.Item2.ToString());
                line.Add("Score", t.Item3.ToString());
                text.Add(Scripter.BuildLine(line));
            }
            TxtEditor.WriteAllLines("Data", "Score", text.ToArray());

        }
    }


    public static class TestBoard
    {
        public static TypeWriter Typer = new TypeWriter();
        public static Gfx2D TyperLine = new Gfx2D(new Rectangle(100, 100, 300, 50));
        public static Button SaveButton = new Button(new GfxStr(20, "Save", new REMOPoint(400, 400)), () => 
        {
            ScoreBoard.AddScore(Typer.TypeLine, 1, 111);            
            ScoreBoard.SaveScore(); 
        });
        public static CheckBox check1 = new CheckBox(20, "Test", new REMOPoint(300, 300));
        public static Scene scn = new Scene(() =>
        {
            ScoreBoard.Init();
            ScoreBoard.AddScore("Test", 4, 200);
            ScoreBoard.SaveScore();
        }, () =>
        {

            if(Typer.TypeLine.Length<10)
                Typer.Update();
            SaveButton.Enable();
            check1.Update();
        }, () =>
        {
            TyperLine.Draw(Color.Red);
            StandAlone.DrawString(20, Typer.TypeLine, new REMOPoint(100, 100), Color.White);
            SaveButton.Draw();
            Cursor.Draw(Color.White);
            check1.Draw(Color.White);

        });

    }
    
    public static class SettingScene
    {
        public static GfxStr Credit = new GfxStr("KoreanFont", "Credit: Songs made by 구재영, 계한용, 김홍래. The game is made by REMO Engine, RealMono Inc.", new REMOPoint(50,450));
        public static VolumeBar SongVolumeBar = new VolumeBar(new Gfx2D(new Rectangle(50, 80, 300, 50)), "WhiteSpace", 20, (f) => { MusicBox.SongVolume = f; MusicBox.SEVolume = f; });
        public static Button GoBack = new Button(new GfxStr(20, "Go Back", new REMOPoint(50, 350)),()=> { Projectors.Projector.SwapTo(MainScene.scn); });
        public static CheckBox BackgroundFlick = new CheckBox(20, "Enable Background Flickering", new REMOPoint(50, 250));
        public static Scene scn = new Scene(() =>
        {
            PlayerClass.Init();
            InGameInterface.Init();
            Stage1_Enemies.HealEnemies.Enemies.Clear();
            scn.bgm = "SummerNight";
        }, () =>
        {
            Stage1_Enemies.HealEnemies.Update();
            Functions.GameUpdate();

            SongVolumeBar.Enable();
            GoBack.Enable();
            BackgroundFlick.Update();
        }, () =>
        {
            PlayerClass.Draw(Color.Orange);
            Fader.DrawAll();
            InGameInterface.Ground.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);

            Filter.Absolute(StandAlone.FullScreen, Color.Black * (0.8f+Fader.Flicker(1000)*0.15f));

            StandAlone.DrawString("Volume", new REMOPoint(50, 50), Color.White);
            SongVolumeBar.Draw(Color.Gray, Color.White);
            GoBack.DrawWithAccent(Color.White,Color.Red);

            Credit.Draw(Color.White);
            Cursor.Draw(Color.White);
            BackgroundFlick.Draw(Color.White);
        });

    }
}




