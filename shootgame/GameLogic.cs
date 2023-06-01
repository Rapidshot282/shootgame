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
        int maxEnemyCount = 10;
        int maxItemCount = 3;
        int score = 0;
        int playerSpeed = 30;
        int gameTime = 30;

        // flag 및 timer 설정
        bool playerLeftMovement = true;
        bool playerRightMovement = true;

        public Timer gameTimer = new Timer();
        private Timer projectileTimer = new Timer();
        private Timer countdownTimer = new Timer();

        private Form gameForm;
        private PictureBox player;
        private List<PictureBox> projectiles;
        private List<PictureBox> enemies;
        private List<PictureBox> timeItems;
        private List<PictureBox> healthItems;

        private Label lifeLabel;
        private Label scoreLabel;
        private Label timeLabel;

        private int visibleEnemyCount = 0;
        public GameLogic(Form form)
        {
            gameForm = form;
            lifeLabel = (Label)gameForm.Controls["lifeLabel"];
            scoreLabel = (Label)gameForm.Controls["scoreLabel"];
            timeLabel = (Label)gameForm.Controls["timeLabel"];

            gameTimer.Interval = 50; // 게임 타이머
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            projectileTimer.Interval = 100; // 탄환 타이머
            projectileTimer.Tick += ProjectileTimer_Tick;

            countdownTimer.Interval = 1000; // 게임시간 측정 (1초)
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            player = (PictureBox)gameForm.Controls["player"];
            projectiles = new List<PictureBox>();
            enemies = new List<PictureBox>();
            timeItems = new List<PictureBox>();
            healthItems = new List<PictureBox>();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            lifeLabel.Text = "Life: " + playerLife;
            scoreLabel.Text = "Score: " + score;

            MoveEnemies();
            MoveItem_health();
            MoveItem_time();

            CheckCollision();

            if (playerLife <= 0 || gameTime <= 0)
            {
                if (playerLife <= 0)
                {
                    EndGame_Life();
                    return;
                }
                if (gameTime <= 0)
                {
                    EndGame_Time();
                    return;
                }
            }

            if (visibleEnemyCount < maxEnemyCount)
            {
                SpawnEnemy();
            }

            if (gameTime < 30 && gameTime % 5 != 0) //수정필요 적과 비슷한 로직으로 (함수참고)
            {
                SpawnTimeItem();
                SpawnHealthItem();
            }

            UpdateProjectiles();
        }

        private void MoveEnemies()
        {
            foreach (PictureBox enemy in enemies)
            {
                enemy.Top += 30; // 적 속도

                if (enemy.Top >= gameForm.Height)
                {
                    enemy.Top = 0;
                }
            }
        }

        private void MoveItem_health()
        {
            foreach (PictureBox healthItem in healthItems)
            {
                healthItem.Top += 40; // 적 속도

                if (healthItem.Top >= gameForm.Height)
                {
                    healthItem.Top = 0;
                }
            }
        }

        private void MoveItem_time()
        {
            foreach (PictureBox timeItem in timeItems)
            {
                timeItem.Top += 40; // 적 속도

                if (timeItem.Top >= gameForm.Height)
                {
                    timeItem.Top = 0;
                }
            }
        }

        private void CheckCollision()
        {
            foreach (PictureBox enemy in enemies)
            {
                if (enemy.Bounds.IntersectsWith(player.Bounds) && enemy.Visible)
                {
                    enemy.Visible = false;
                    playerLife--;
                    score += 10;
                    visibleEnemyCount--; // Decrease the count when an enemy becomes invisible
                }
            }

            foreach (PictureBox projectile in projectiles)
            {
                foreach (PictureBox enemy in enemies)
                {
                    if (projectile.Bounds.IntersectsWith(enemy.Bounds) && enemy.Visible)
                    {
                        enemy.Visible = false;
                        projectile.Visible = false;
                        score += 10;
                        visibleEnemyCount--; // Decrease the count when an enemy becomes invisible
                    }
                }
            }
        }

        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                PictureBox projectile = projectiles[i];
                projectile.Top -= 20; // 탄환 속도

                if (projectile.Top <= 0)
                {
                    projectiles.RemoveAt(i);
                    gameForm.Controls.Remove(projectile);
                }
            }
        }

        private void ProjectileTimer_Tick(object sender, EventArgs e)
        {
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

        private void FireProjectile()
        {
            PictureBox projectile = new PictureBox();
            projectile.ImageLocation = "C:/Users/user/Downloads/projectile.png"; // 탄환 이미지
            projectile.Size = new Size(20, 20); // 탄환 사이즈
            projectile.SizeMode = PictureBoxSizeMode.StretchImage;
            projectile.Tag = "projectile";
            projectile.Left = player.Left + (player.Width / 2) - (projectile.Width / 2);
            projectile.Top = player.Top;
            gameForm.Controls.Add(projectile);
            projectiles.Add(projectile);
        }

        private void SpawnEnemy() 
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 50);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

            PictureBox enemy = new PictureBox();
            enemy.ImageLocation = "C:/Users/user/Downloads/pokemon-g605cbf575_640.png"; // 적 이미지
            enemy.Size = new Size(40, 30); // 적 사이즈
            enemy.SizeMode = PictureBoxSizeMode.StretchImage;
            enemy.Tag = "enemy";
            enemy.Left = spawnLocationX;
            enemy.Top = spawnLocationY;

            gameForm.Controls.Add(enemy);
            enemies.Add(enemy);

            visibleEnemyCount++;
        }

        private void SpawnTimeItem()
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 50);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

            PictureBox timeItem = new PictureBox();
            timeItem.ImageLocation = ""; // 시간 증가 아이템 이미지
            timeItem.Size = new Size(40, 30); // 아이템 사이즈
            timeItem.SizeMode = PictureBoxSizeMode.StretchImage;
            timeItem.Tag = "timeItem";
            timeItem.Left = spawnLocationX;
            timeItem.Top = spawnLocationY;

            gameForm.Controls.Add(timeItem);
            timeItems.Add(timeItem);
        }

        private void SpawnHealthItem()
        {
            Random random = new Random();

            int spawnLocationX = random.Next(0, gameForm.Width - 100);
            int spawnLocationY = 150 - random.Next(0, gameForm.Height - 50);

            PictureBox healthItem = new PictureBox();
            healthItem.ImageLocation = ""; // 라이프증가 아이템 이미지
            healthItem.Size = new Size(40, 30); // 아이템 사이즈
            healthItem.SizeMode = PictureBoxSizeMode.StretchImage;
            healthItem.Tag = "healthItem";
            healthItem.Left = spawnLocationX;
            healthItem.Top = spawnLocationY;

            gameForm.Controls.Add(healthItem);
            healthItems.Add(healthItem);
        }

        private void EndGame_Life()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! *라이프 소진*\n최종점수 : " + score, "게임");

            if (MessageBox.Show("다시 진행하시겠습니까?", "게임", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
                Application.Exit();
            }
        }

        private void EndGame_Time()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! *시간초과*\n최종점수 : " + score, "게임");

            if (MessageBox.Show("다시 진행하시겠습니까?", "게임", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
                Application.Exit();
            }
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                MovePlayerLeft();

            }
            if (e.KeyCode == Keys.Right)
            {
                MovePlayerRight();

            }
            if(e.KeyCode == Keys.Space)
            { 
                FireProjectile();
            }
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
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
            if (player.Left - playerSpeed >= 0)
            {
                player.Left -= playerSpeed;
            }
        }

        private void MovePlayerRight()
        {
            if (player.Left + player.Width + playerSpeed <= gameForm.Width)
            {
                player.Left += playerSpeed;
            }
        }

        private void StopPlayerMovement()
        {
            playerLeftMovement = false;
            playerRightMovement = false;
        }
    }
}