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

namespace FlickerGame
{

    public static class GameOverScene
    {
        public static GfxStr GameOverString = new GfxStr("Game Over", new Point(200, 200));
        public static Scene[] SceneList = new Scene[] { TutorialStage.scn, Stage2.scn, Stage1.scn, Stage3.scn };
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
        public static bool isSaved = false;
        
        public static Scene scn = new Scene(() =>
        {
            MusicBox.StopSong();
            isSaved = false;
        }, () =>
        {
            StandAlone.FrameTimer--;
            PauseMenus.Update();
            ScoreBoard.Update();
            
        }, () =>
        {
            Filter.Absolute(Functions.GameScreen, Color.Black * 0.6f);
            PauseMenus.Draw(Color.White);
            ScoreBoard.Draw();
        
            Cursor.Draw(Color.White);
        });
    }

    public static class MainScene
    {
        public static Gfx2D BigSquare = new Gfx2D(new Rectangle(0, 0, 800, 800));

        public static SimpleMenu MainMenus = new SimpleMenu(20, new REMOPoint(800, 200), new REMOPoint(0, 60),
            new string[] { "New Game", "Tutorial", "Settings", "ScoreBoard", "Exit" },
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
            ()=>
            {
                Projectors.Projector.SwapTo(ScoreBoardScene.scn);
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
                Functions.InitSetting();
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
            Filter.Absolute(Functions.GameScreen, Color.White);
            Filter.Absolute(Functions.GameScreen, Color.Black*Fader.Flicker(300));
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
                if (PlayerClass.isFlickering)
                    Enemies[i].MoveByVector(new REMOPoint(-1, 0), 2);
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
                if (PlayerClass.isFlickering)
                    Enemies[i].MoveByVector(new REMOPoint(-1, 0), 2);

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
                PlayerClass.player_hp -= (int)(EnemySet.Atk*1.5) + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            Enemies.GenTimer = StandAlone.Random(40,50);
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
                PlayerClass.player_hp -= (int)(EnemySet.Atk * 1.5) + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            FloorEnemies.GenTimer = StandAlone.Random(50, 110);
            FloorEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, 390, StandAlone.Random(5,10), StandAlone.Random(5, 10)))); // 적들을 생성합니다.
        });

        public static EnemySet BigEnemies = new EnemySet((i) =>
        {
            BigEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 9 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 점점 빨라집니다.
            BigEnemies.Enemies[i].Zoom(BigEnemies.Enemies[i].Center, 0.984f);
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {
                PlayerClass.CurrentEnemy = BigEnemies.Enemies[i];
                PlayerClass.player_hp -= (int)(EnemySet.Atk * 1.5) + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            BigEnemies.GenTimer = StandAlone.Random(400, 500);
            Gfx2D g;
            g = new Gfx2D(new Rectangle(1000, StandAlone.Random(230, 300), 1000, 1000));
            BigEnemies.Enemies.Add(g); // 적들을 생성합니다.
            g.Center = new REMOPoint(g.Center.X, StandAlone.Random(0, 450));
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
        public static float Score;

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
            if (PlayerClass.isFlickering)
                Score += 0.5f;
            Score += (Score/60) * 0.001f;

            //Set Stage Color
            int interval = 3000;
            int groundNumber = (int)(Score / interval) % StarColors.Count;
            StageColor = new Color[] { StarColors[groundNumber], StarColors[(groundNumber + 1) % StarColors.Count] * ((float)(Score % interval) / (float)interval), Color.Black * 0.3f };

            HpGauge.Update(PlayerClass.player_hp,PlayerClass.PlayerHpMax);
            StarGauge.Update(PlayerClass.StarTimer, PlayerClass.StarTimerMax);
            HealStackGauge.Update(PlayerClass.healstack, PlayerClass.healstackMax);
        }

        public static void Draw()
        {
            if (PlayerClass.isFlickering&&SettingScene.BackgroundFlick)
            {
                Filter.Absolute(Functions.GameScreen, StandAlone.RandomPick(StarColors) * 0.2f);
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
            StandAlone.DrawString(30, "Distance : " + ((int)InGameInterface.Score / 60).ToString() + "m", new Point(600, 430), Color.White);
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
            Stage2_Enemies.BigEnemies.Enemies.Clear();
            scn.bgm = "Journey";

        }, () =>
        {
            Functions.GameUpdate();
            Stage2_Enemies.Enemies.Update();
            Stage1_Enemies.HealEnemies.Update();
            Stage2_Enemies.FloorEnemies.Update();
            if(StandAlone.FrameTimer>2000)
                Stage2_Enemies.BigEnemies.Update();



        }, () =>
        {
            Stage2_Enemies.Enemies.Draw(InGameInterface.StageColor);
            Stage2_Enemies.FloorEnemies.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);
            Stage2_Enemies.BigEnemies.Draw(InGameInterface.StageColor);
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
            if (PlayerClass.isFlickering)
                PlayerClass.StarTimer += 30;
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
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {

                PlayerClass.CurrentEnemy = SinEnemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
        }, () => {
            SinEnemies.GenTimer = StandAlone.Random(60, 80);
            SinEnemies.Enemies.Add(new Gfx2D(new Rectangle(1000, StandAlone.Random(150, 350), 30, 30))); // 적들을 생성합니다.
        });

        public static EnemySet BigEnemies = new EnemySet((i) => {
            BigEnemies.Enemies[i].MoveByVector(new Point(-10, 0), 5 + 0.03 * (StandAlone.FrameTimer / 100));//적들은 조금씩 점점 빨라집니다.
        }, (i) => {
            if (!PlayerClass.isFlickering && PlayerClass.DamageTimer == 0)
            {

                PlayerClass.CurrentEnemy = BigEnemies.Enemies[i];
                PlayerClass.player_hp -= EnemySet.Atk + StandAlone.FrameTimer / 500;//적들은 점점 강해집니다.
                PlayerClass.DamageTimer = PlayerClass.DamageTimerMax;
                PlayerClass.DamageColor = Color.Red;
            }
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
            Stage1_Enemies.BigEnemies.Enemies.Clear();
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

        public static Scene[] WholeSceneList = new Scene[] { ScoreBoardScene.scn, PauseScene.scn, MainScene.scn, Stage1.scn, Stage2.scn, Stage3.scn, TutorialStage.scn, SettingScene.scn  };
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
                Projectors.Projector.Load(PauseScene.scn);
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
                Filter.Absolute(Functions.GameScreen, Color.Black * (0.7f * (1-StandAlone.FrameTimer / (float)FadeInTime)));
            }

        }


        public static Rectangle GameScreen = new Rectangle(0, 0, 1000, 500);
        public static void SetScreen()
        {
            if(SettingScene.SmallMode)
            {
                StandAlone.FullScreen = new Rectangle(0, 0, 500, 250);
                StandAlone.FontZoom = 0.5f;
                foreach(Scene s in WholeSceneList)
                {
                    s.Camera.Zoom = 0.5f;
                }
                Cursor.CursorCoefficient = 2f;
            }
            else
            {
                StandAlone.FullScreen = new Rectangle(0, 0, 1000, 500);
                StandAlone.FontZoom = 1f;
                foreach (Scene s in WholeSceneList)
                {
                    s.Camera.Zoom = 1f;
                }
                Cursor.CursorCoefficient = 1f;
            }
        }

        public static void InitSetting()
        {
            TxtEditor.MakeTextFile("Data", "Settings");
            SettingScene.SettingScripter.AddRule("BackgroundFlick", (s) => {
                if (s == "True")
                {
                    SettingScene.BackgroundFlick.isChecked = true;
                }
                else
                {
                    SettingScene.BackgroundFlick.isChecked = false;
                }
            });
            SettingScene.SettingScripter.AddRule("SmallMode", (s) => {
                if (s == "True")
                {
                    SettingScene.SmallMode.isChecked = true;
                }
                else
                {
                    SettingScene.SmallMode.isChecked = false;
                }
            });
            if (TxtEditor.ReadAllLines("Data", "Settings").Length == 0)
            {
                Dictionary<string, string> lines = new Dictionary<string, string>();
                lines.Add("BackgroundFlick", "True");
                lines.Add("SmallMode", "False");
                TxtEditor.WriteAllLines("Data", "Settings", new string[] { Scripter.BuildLine(lines) });
            }
            else
            {
                SettingScene.SettingScripter.ReadTxt("Data", "Settings");
            }
            Functions.SetScreen();

        }

        public static void BuildSettingScript()
        {
            Dictionary<string, string> lines = new Dictionary<string, string>();
            lines.Add("BackgroundFlick", SettingScene.BackgroundFlick.isChecked.ToString());
            lines.Add("SmallMode", SettingScene.SmallMode.isChecked.ToString());
            TxtEditor.WriteAllLines("Data", "Settings", new string[] { Scripter.BuildLine(lines) });
        }
    }
    public static class ScoreBoard
    {
        public static List<Tuple<string, int, int, string>> ScoreSet = new List<Tuple<string, int, int, string>>();
        public static Scripter ScoreReader = new Scripter();
        private static string _name;
        private static int _stage;
        private static int _score;
        private static string _time;
        public static TypeWriter Typer = new TypeWriter();
        public static Button SaveScoreButton = new Button(new GfxStr(20, "Save Score", new REMOPoint(500, 300)), () =>
           {
               DateTime localTIme = DateTime.Now;
               if (Projectors.Projector.Loaded(Stage1.scn) || (Projectors.Projector.Loaded(TutorialStage.scn) && TutorialStage.TutorialState >= 5))
                   AddScore(Typer.TypeLine, 1, (int)InGameInterface.Score / 60, localTIme.ToString("MM/dd/yyyy HH:mm"));
               if(Projectors.Projector.Loaded(Stage2.scn))
                   AddScore(Typer.TypeLine, 2, (int)InGameInterface.Score / 60, localTIme.ToString("MM/dd/yyyy HH:mm"));
               if (Projectors.Projector.Loaded(Stage3.scn))
                   AddScore(Typer.TypeLine, 3, (int)InGameInterface.Score / 60, localTIme.ToString("MM/dd/yyyy HH:mm"));
               ValidityCheck();
               SaveScore();
               TxtEditor.WriteAllLines("Data", "Hash",new string[] { TxtEditor.HashTxt("Data", "Score") });
           });

        public static void ValidityCheck()
        {
            string[] hashes = TxtEditor.ReadAllLines("Data", "Hash");
            string hash = "";
            if (hashes.Length > 0)
                hash = hashes[0];
            if (!TxtEditor.ValidateTxt("Data", "Score", hash))
            {//통과를 못할 경우 삭제 후 재지정
                TxtEditor.DeleteFile("Data", "Score");
                TxtEditor.MakeTextFile("Data", "Score");
            }
        }


        //<Name>Blabla<Stage>1<Score>300 <-> Tuple(Blabla, 1, 300)
        public static void Init()
        {
            //스코어를 저장할 스크립트 파일 생성
            TxtEditor.MakeTextFile("Data", "Score");
            //해시를 저장할 스크립트파일 생성
            TxtEditor.MakeTextFile("Data", "Hash");
            //스크립트를 읽어들이는 룰 지정
            ScoreReader.AddRule("Name", (s) => { _name = s; });
            ScoreReader.AddRule("Stage", (s) => { _stage = Int32.Parse(s); });
            ScoreReader.AddRule("Score", (s) =>
             {
                 _score = Int32.Parse(s);
             });
            ScoreReader.AddRule("Time", (s) =>
            {
                _time = s;
                AddScore(_name, _stage, _score,_time);
            });
            ValidityCheck();
            //스크립트 파일을 읽어옵니다.
            ScoreReader.ReadTxt("Data", "Score");
            if (ScoreSet.Count > 0)
                Typer.TypeLine = ScoreSet[ScoreSet.Count - 1].Item1;
            else
                Typer.TypeLine = "NoName";
        }
        public static void AddScore(string Name, int Stage, int Score, string Time)
        {
            ScoreSet.Add(new Tuple<string, int, int, string>(Name, Stage, Score, Time));
        }
        /// <summary>
        /// 지금까지의 Score Set을 지정된 txt파일에 저장합니다.
        /// </summary>
        public static void SaveScore()
        {
            List<string> text = new List<string>();
            foreach (Tuple<string, int, int, string> t in ScoreSet)
            {
                Dictionary<string, string> line = new Dictionary<string, string>();
                line.Add("Name", t.Item1);
                line.Add("Stage", t.Item2.ToString());
                line.Add("Score", t.Item3.ToString());
                line.Add("Time", t.Item4.ToString());
                text.Add(Scripter.BuildLine(line));
            }
            TxtEditor.WriteAllLines("Data", "Score", text.ToArray());            
        }

        public static void Update()
        {
            Typer.Update();
            if (Typer.TypeLine.Length > 10)
                Typer.TypeLine = Typer.TypeLine.Substring(0, 10);

            if(!PauseScene.isSaved)
            {
                GfxStr s = (GfxStr)(SaveScoreButton.ButtonGraphic);
                s.Text = "Save Score";
                SaveScoreButton.ButtonGraphic = s;
                SaveScoreButton.Enable();
                if(User.JustLeftClicked(SaveScoreButton))
                {
                    PauseScene.isSaved = true;
                }
            }
            else
            {
                GfxStr s = (GfxStr)(SaveScoreButton.ButtonGraphic);
                s.Text = "Saved.";
                SaveScoreButton.ButtonGraphic = s;

            }
        }
        public static void Draw()
        {
            StandAlone.DrawString(20, "Input your Name : " + Typer.TypeLine, new REMOPoint(500, 200), Color.White);
            StandAlone.DrawString(20, "Score :" + (int)InGameInterface.Score/60, new REMOPoint(500,250),Color.White);
            if(!PauseScene.isSaved)
                SaveScoreButton.DrawWithAccent(Color.White, Color.Red);
            else
                SaveScoreButton.Draw(Color.Gray);
        }

    }

    public static class ScoreBoardScene
    {
        public static List<Tuple<string, int, int, string>> Stage1_Scores = new List<Tuple<string, int, int, string>>();
        public static List<Tuple<string, int, int, string>> Stage2_Scores = new List<Tuple<string, int, int, string>>();
        public static List<Tuple<string, int, int, string>> Stage3_Scores = new List<Tuple<string, int, int, string>>();
        public static List<Tuple<string, int, int, string>>[] StageScores = new List<Tuple<string, int, int, string>>[] {
        Stage1_Scores, Stage2_Scores,Stage3_Scores
        };
        public static int CurrentStage = 2;
        public static int currentPage = 0;
        public static int CurrentPage
        {
            get { return currentPage; }
            set { if (value >= 0 && value <= CurrentPageMax)
                {
                    currentPage = value;
                }
            }
        }
        public static int CurrentPageMax = 0;

        public static void ChangeShowingStage(int StageNum)
        {
            CurrentStage = StageNum;
            CurrentPage = 0;
            CurrentPageMax = (StageScores[StageNum - 1].Count-1) / PageCapacity;
        }
        public static int PageCapacity = 10;
        public static Button GoBackButton = new Button( new GfxStr(20,"Go Back", new REMOPoint(500, 420)), () =>
         {
             Projectors.Projector.SwapTo(MainScene.scn);
         });
        public static SimpleMenu StageMenu = new SimpleMenu(20, new REMOPoint(100, 420), new REMOPoint(100, 0),
            new string[] { "Stage 1", "Stage 2", "Stage 3"}, 
            () => { ChangeShowingStage(1); }, () => { ChangeShowingStage(2); }, () => { ChangeShowingStage(3); });

        public static Button NextPageButton = new Button(new GfxStr(20,"Next", new REMOPoint(580, 230)), () =>
        {
            CurrentPage++;
        });
        public static Button PrevPageButton = new Button(new GfxStr(20, "Prev", new REMOPoint(500, 230)), () =>
        {
            CurrentPage--;
        });


        public static Scene scn = new Scene(() =>
        {
            foreach (List<Tuple<string, int, int,string>> t in StageScores)
            {
                t.Clear();
            }


            foreach (Tuple<string,int,int,string> t in ScoreBoard.ScoreSet)
            {
                StageScores[t.Item2 - 1].Add(t);
            }
            foreach (List<Tuple<string, int, int,string>> t in StageScores)
            {
                t.Sort((x, y) => y.Item3.CompareTo(x.Item3));
            }

            scn.bgm = "StillGood";

        }, () =>
        {
            GoBackButton.Enable();
            StageMenu.Update();
            if(User.JustPressed(Keys.Right))
            {
                CurrentPage++;
            }
            if (User.JustPressed(Keys.Left))
            {
                CurrentPage--;
            }
            NextPageButton.Enable();
            PrevPageButton.Enable();
        }, () =>
        {
            StandAlone.DrawString(20, "Stage "+(CurrentStage)+" Scores", new REMOPoint(400,30), Color.White);
            StandAlone.DrawString("name               score          time", new REMOPoint(100, 70), Color.White);
            int s = CurrentPage * PageCapacity;
            int f = s + 9;
            for (int i=s;i<StageScores[CurrentStage-1].Count&&i<=f; i++)
            {
                Tuple<string, int, int,string> t = StageScores[CurrentStage - 1][i];
                StandAlone.DrawString(t.Item1, new REMOPoint(100, 110 + (i - s) * 30), Color.White);
                StandAlone.DrawString(t.Item3.ToString(), new REMOPoint(250, 110 + (i - s) * 30), Color.White);
                StandAlone.DrawString(t.Item4, new REMOPoint(350, 110 + (i - s) * 30), Color.White);


            }
            GoBackButton.DrawWithAccent(Color.White,Color.Red);
            StageMenu.Draw(Color.White);
            if(CurrentPageMax>=1)
            {
                NextPageButton.DrawWithAccent(Color.White, Color.Red);
                PrevPageButton.DrawWithAccent(Color.White, Color.Red);
                StandAlone.DrawString(20, "Page " + (CurrentPage + 1), PrevPageButton.Pos + new REMOPoint(0, 50), Color.White);
            }
            Cursor.Draw(Color.White);
        });
    }


    public static class TestBoard
    {
        public static TypeWriter Typer = new TypeWriter();
        public static Gfx2D TyperLine = new Gfx2D(new Rectangle(100, 100, 300, 50));
        public static Button SaveButton = new Button(new GfxStr(20, "Save", new REMOPoint(400, 400)), () => 
        {
            ScoreBoard.SaveScore(); 
        });
        public static CheckBox check1 = new CheckBox(20, "Test", new REMOPoint(300, 300));
        public static Scene scn = new Scene(() =>
        {
            ScoreBoard.Init();
            ScoreBoard.SaveScore();
        }, () =>
        {

             Typer.Update();
            if (Typer.TypeLine.Length > 10)
                Typer.TypeLine = Typer.TypeLine.Substring(0, 1);
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
        public static Scripter SettingScripter = new Scripter();
        public static GfxStr Credit = new GfxStr("KoreanFont", "Credit: Songs made by 구재영, 계한용, 김홍래. The game is made by REMO Engine, RealMono Inc.", new REMOPoint(50,450));
        public static VolumeBar SongVolumeBar = new VolumeBar(new Gfx2D(new Rectangle(50, 80, 300, 50)), "WhiteSpace", 20, (f) => { MusicBox.SongVolume = f; MusicBox.SEVolume = f; });
        public static Button GoBack = new Button(new GfxStr(20, "Go Back", new REMOPoint(100, 360)),()=> { Projectors.Projector.SwapTo(MainScene.scn); });
        public static CheckBox BackgroundFlick = new CheckBox(20, "Enable Background Flickering", new REMOPoint(50, 250));
        public static CheckBox SmallMode = new CheckBox(20, "Small window mode", new REMOPoint(50, 300));
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
            PlayerClass.player_hp = 10000;

            SongVolumeBar.Enable();
            GoBack.Enable();
            bool currentBackgroundFlick = BackgroundFlick;
            bool currentSmallMode = SmallMode;
            BackgroundFlick.Update();
            SmallMode.Update();
            Functions.SetScreen();
            if(currentBackgroundFlick!=BackgroundFlick||currentSmallMode!=SmallMode)
            {
                Functions.BuildSettingScript();
            }
        }, () =>
        {
            PlayerClass.Draw(Color.Orange);
            Fader.DrawAll();
            InGameInterface.Ground.Draw(InGameInterface.StageColor);
            Stage1_Enemies.HealEnemies.Draw(Color.Yellow);

            Filter.Absolute(Functions.GameScreen, Color.Black * (0.8f+Fader.Flicker(1000)*0.15f));

            StandAlone.DrawString("Volume", new REMOPoint(50, 50), Color.White);
            SongVolumeBar.Draw(Color.Gray, Color.White);
            GoBack.DrawWithAccent(Color.White,Color.Red);

            Credit.Draw(Color.White);
            Cursor.Draw(Color.White);
            BackgroundFlick.Draw(Color.White);
            SmallMode.Draw(Color.White);
        });

    }
}




