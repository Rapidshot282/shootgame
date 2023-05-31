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
            // Code for initializing the form and any other necessary setup
        }
    }
}
