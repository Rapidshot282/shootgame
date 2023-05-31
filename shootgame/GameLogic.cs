using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace shootgame
{
    public class GameLogic
    {
        // 플레이어 status
        int playerLife = 3;
        int maxEnemyCount = 8;
        int score = 0;
        int playerSpeed = 30;
        int gameTime = 60;

        //각종 플래그 및 타이머 설정
        bool playerLeftMovement = true;
        bool playerRightMovement = true;

        public Timer gameTimer = new Timer();
        private Timer projectileTimer = new Timer();
        private Timer countdownTimer = new Timer();

        private Form gameForm;
        private PictureBox player;
        private List<PictureBox> projectiles;
        private List<PictureBox> healthi;
        private List<PictureBox> timei;

        private Label lifeLabel;
        private Label scoreLabel;
        private Label timeLabel;

        public GameLogic(Form form)
        {
            gameForm = form;
            lifeLabel = (Label)gameForm.Controls["lifeLabel"];
            scoreLabel = (Label)gameForm.Controls["scoreLabel"];
            timeLabel = (Label)gameForm.Controls["timeLabel"];


            gameTimer.Interval = 50; // 인터벌 타임(게임시간)
            gameTimer.Tick += GameTimer_Tick; // 타임
            gameTimer.Start();

            projectileTimer.Interval = 100; // 인터벌 타임(탄환)
            projectileTimer.Tick += ProjectileTimer_Tick;

            countdownTimer.Interval = 1000; // 게임시간을 보여주기 위한 타임
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            player = (PictureBox)gameForm.Controls["player"];
            projectiles = new List<PictureBox>();
            healthi = new List<PictureBox>();
            timei = new List<PictureBox>();
        }

        // 게임 메인 타이머
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            lifeLabel.Text = "라이프 : " + playerLife;
            scoreLabel.Text = "점수 : " + score;
            foreach (Control control in gameForm.Controls)
            {
                if (control is PictureBox && control.Tag == "enemy")
                {
                    control.Top += 30; // 적의 속도

                    // 적 폼 바깥으로 나감을 체크
                    if (control.Top >= gameForm.Height)
                    {
                        control.Top = 0; 
                    }
                }
            }

            // 플레이어 충돌감지
            foreach (Control control in gameForm.Controls)
            {
                if (control is PictureBox && control.Tag == "enemy" && control.Bounds.IntersectsWith(gameForm.Controls["player"].Bounds) && control.Visible == true)
                {
                    if (projectiles.Contains(control))
                    {
                        control.Visible = false;
                        playerLife -= 1;
                        score += 10;
                    }
                    else
                    {
                        control.Visible = false;
                        playerLife -= 1;
                        SpawnSingleEnemy();
                    }
                }
                else if (control is PictureBox && control.Tag == "timeItem" && control.Bounds.IntersectsWith(gameForm.Controls["player"].Bounds) && control.Visible == true)
                {
                    control.Visible = false;
                    gameTime += 10;
                    score += 20;
                }
                else if (control is PictureBox && control.Tag == "healthItem" && control.Bounds.IntersectsWith(gameForm.Controls["player"].Bounds) && control.Visible == true)
                {
                    control.Visible = false;
                    playerLife += 1;
                    score += 20;
                }
            }

            // 라이프 갯수 체크
            if (playerLife <= 0 || gameTime <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("Gameover! \n최종점수 : " + score, "게임");
                if (MessageBox.Show("게임을 종료하시겠습니까?", "게임", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                    Application.Restart();
            }

            // 첫 적 스폰
            if (gameForm.Controls.OfType<PictureBox>().Count(enemy => enemy.Tag == "enemy") < maxEnemyCount)
            {
                SpawnEnemies();
            }

            // 탄환이 적을 맞추는지 확인
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                PictureBox projectile = projectiles[i];
                projectile.Top -= 20; // 탄환속도

                foreach (Control control in gameForm.Controls)
                {
                    if (control is PictureBox && control.Tag == "enemy" && control.Bounds.IntersectsWith(projectile.Bounds) && control.Visible == true)
                    {
                        control.Visible = false;
                        gameForm.Controls.Remove(projectile);
                        projectiles.Remove(projectile);
                        score += 10; // 점수
                        SpawnSingleEnemy();
                        break; 
                    }
                }

                // 화면 밖으로 나갈 시 탄환 삭제
                if (projectile.Top <= 0)
                {
                    gameForm.Controls.Remove(projectile);
                    projectiles.Remove(projectile);
                }
            }
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            // 키보드 입력

            switch (e.KeyCode)
            {
                case Keys.Left:
                    MovePlayerLeft();
                    break;
                case Keys.Right:
                    MovePlayerRight();
                    break;
                case Keys.Space:
                    FireProjectile();
                    break;
            }
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            // 키보드 입력x
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    StopPlayerMovement();
                    break;
            }
        }

        private void MovePlayerLeft()
        {
            player.Left -= playerSpeed;
        }

        private void MovePlayerRight()
        {
            player.Left += playerSpeed;
        }

        private void StopPlayerMovement()
        {
            playerLeftMovement = false;
            playerRightMovement = false;
        }

        private void ProjectileTimer_Tick(object sender, EventArgs e)
        {
            // 탄환 발사
            FireProjectile();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (gameTime == 0)
            {
                gameTimer.Stop();
            }

            else
            {
                gameTime--;
                timeLabel.Text = "시간: " + gameTime.ToString();
            }
        }

        private void FireProjectile() //탄환 생성
        {
            PictureBox projectile = new PictureBox();
            projectile.ImageLocation = "C:/Users/user/Downloads/projectile.png"; // 탄환 이미지
            projectile.Size = new System.Drawing.Size(20, 20); // 탄환 사이즈
            projectile.SizeMode = PictureBoxSizeMode.StretchImage;
            projectile.Tag = "projectile";
            projectile.Left = player.Left + (player.Width / 2) - (projectile.Width / 2);
            projectile.Top = player.Top;
            gameForm.Controls.Add(projectile);
            projectiles.Add(projectile);
        }

        private void SpawnSingleEnemy() //탄환으로 적 제거시 다시 소환
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 50);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);
              
            PictureBox enemy = new PictureBox();
            enemy.ImageLocation = "C:/Users/user/Downloads/pokemon-g605cbf575_640.png"; //적 이미지
            enemy.Size = new System.Drawing.Size(40, 30); //적 사이즈
            enemy.SizeMode = PictureBoxSizeMode.StretchImage;
            enemy.Tag = "enemy";
            enemy.Left = spawnLocationX;
            enemy.Top = spawnLocationY;

            gameForm.Controls.Add(enemy);
        }

        private void SpawnEnemies() //첫 적 소환
        {
            Random random = new Random();

            for (int i = 0; i < maxEnemyCount; i++)
            {
                int spawnLocationX = random.Next(0, gameForm.Width - 50);
                int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

                PictureBox enemy = new PictureBox();
                enemy.ImageLocation = "C:/Users/user/Downloads/pokemon-g605cbf575_640.png"; //적 이미지
                enemy.Size = new System.Drawing.Size(40, 30);
                enemy.SizeMode = PictureBoxSizeMode.StretchImage;
                enemy.Tag = "enemy";
                enemy.Left = spawnLocationX;
                enemy.Top = spawnLocationY;

                gameForm.Controls.Add(enemy);
            }
        }

        private void SpawnTimeItem() //탄환으로 적 제거시 다시 소환
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 50);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

            PictureBox timei = new PictureBox();
            timei.ImageLocation = ""; //시간 아이템 이미지
            timei.Size = new System.Drawing.Size(40, 30); //시간 아이템 사이즈
            timei.SizeMode = PictureBoxSizeMode.StretchImage;
            timei.Tag = "timeItem";
            timei.Left = spawnLocationX;
            timei.Top = spawnLocationY;

            gameForm.Controls.Add(timei);
        }

        private void SpawnHealthItem() //탄환으로 적 제거시 다시 소환
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 50);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

            PictureBox healthi = new PictureBox();
            healthi.ImageLocation = ""; //체력 아이템 이미지
            healthi.Size = new System.Drawing.Size(40, 30); //체력 아이템 사이즈
            healthi.SizeMode = PictureBoxSizeMode.StretchImage;
            healthi.Tag = "healthItem";
            healthi.Left = spawnLocationX;
            healthi.Top = spawnLocationY;

            gameForm.Controls.Add(healthi);
        }
    }
}