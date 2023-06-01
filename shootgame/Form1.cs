using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shootgame
{
    public partial class Form1 : Form
    {
        private GameLogic gameLogic;

        public Form1()
        {
            InitializeComponent();
            player.Load("C:/Users/user/source/repos/shootgame/shootgame/gameAsset/spaceShip.png");
            player.SizeMode = PictureBoxSizeMode.StretchImage;

            gameLogic = new GameLogic(this);
            KeyDown += Form1_KeyDown;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            gameLogic.HandleKeyDown(e);
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            gameLogic.HandleKeyUp(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
