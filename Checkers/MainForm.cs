using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoardEngine;
using GameLogic;
using GamesPackage;

namespace Checkers
{
    public partial class MainForm : Form
    {
        CheckersBoard MyBoard { get; set; }
        private CheckersModerator MyModerator { get; set; }
        public CheckersPlayer CurrentPlayer { get; set; }
        public CheckersPlayer BlackPlayer { get; set; }
        public CheckersPlayer WhitePlayer { get; set; }

        public MainForm()
        {
            InitializeComponent();
            MyBoard = new CheckersBoard();
            MyBoard.InitializeBoard();

            HumanCheckersPlayer white = new HumanCheckersPlayer(PieceColor.White);
            
            HumanCheckersPlayer black = new HumanCheckersPlayer(PieceColor.Black);

            StartGame(white,black);
        }

        private void StartGame(CheckersPlayer White, CheckersPlayer Black)
        {
            this.WhitePlayer = White;
            this.BlackPlayer = Black;
            this.MyBoard.InitializeBoard();
            CurrentPlayer = WhitePlayer;
            MyModerator = new CheckersModerator(this.MyBoard, White, Black);
            //RepaintBoard();
            MyModerator.StartGame();
        }

    }
}
