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
using System.Collections;

namespace Checkers
{
    public partial class MainForm : Form
    {
        CheckersBoard MyBoard { get; set; }

        public int CellHeight
        {
            get { return BoardPicturebox.Height / 8; }
        }

        public int CellWidth
        {
            get { return BoardPicturebox.Width / 8; }
        }

        private CheckersModerator MyModerator { get; set; }
        public CheckersPlayer CurrentPlayer { get; set; }
        public CheckersPlayer BlackPlayer { get; set; }
        public CheckersPlayer WhitePlayer { get; set; }
        BoardPosition LastSelectedPosition = new BoardPosition(0, 4);
        bool lastMoveOk = false;
        ArrayList moves = new ArrayList();
        CheckersMove LastMove = null;
        int level = 2;
        Bitmap bitmap;
        private BoardDrawer bdrawer;
        ArrayList posiblemoves;
        private ArrayList currentPath;

        public MainForm()
        {
            InitializeComponent();
            MyBoard = new CheckersBoard();
            MyBoard.InitializeBoard();

            bdrawer = new BoardDrawer(this.MyBoard);
            bdrawer.SelectionColor = Color.Yellow;

            SimpleCheckersPlayer white = new SimpleCheckersPlayer(PieceColor.White);
            white.OnPerformMove += white_OnPerformMove;
            //white.OnPlay += white_OnPlay;
            white.OnOtherPlayerMovePerformed += white_OnOtherPlayerMovePerformed;
            white.OnInvalidMove += white_OnInvalidMove;
            white.OnGameOver += white_OnGameOver;

            HumanCheckersPlayer black = new HumanCheckersPlayer(PieceColor.Black);
            black.OnPerformMove += black_OnPerformMove;
            black.OnPlay+=black_OnPlay;
            black.OnOtherPlayerMovePerformed += black_OnOtherPlayerMovePerformed;
            black.OnInvalidMove += black_OnInvalidMove;
            black.OnGameOver += black_OnGameOver;

            bitmap = new Bitmap(this.Width, this.Height);
            Graphics.FromImage(bitmap).FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, this.Width, this.Height));
            StartGame(white,black);
        }

        void black_OnPerformMove(IPlayer player, IMove move)
        {
            this.CurrentPlayer = WhitePlayer;
            //this.Refresh();
        }

        void white_OnPerformMove(IPlayer player, IMove move)
        {
            this.CurrentPlayer = BlackPlayer;
            //this.Refresh();
        }


        public void RepaintBoard()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                bdrawer.PaintBoard(g, 0, 0, BoardPicturebox.Width, BoardPicturebox.Height);
                this.BoardPicturebox.Image = bitmap;
            }
        }
        private void CheckStatus()
        {
            //if the lat move was valid
            if (lastMoveOk)
            {
                //update to queen if can be
                BoardPosition lastposition = (BoardPosition)LastMove.MovePath[LastMove.MovePath.Length - 1];
                CheckersPiece piece = (CheckersPiece)MyBoard.GetPieceAt(lastposition);
                if (lastposition.Y == 0 && piece.Color == PieceColor.White && piece is Pawn)
                {
                    MyBoard.RemovePiece(piece);
                    MyBoard.PutPiece(new Queen(MyBoard, lastposition.X, lastposition.Y, PieceColor.White));
                }
                else if (lastposition.Y == 7 && piece.Color == PieceColor.Black && piece is Pawn)
                {
                    MyBoard.RemovePiece(piece);
                    MyBoard.PutPiece(new Queen(MyBoard, lastposition.X, lastposition.Y, PieceColor.Black));
                }
            }
        }
        bool IsPrefix(ArrayList path, CheckersMove move)
        {
            if (path.Count > move.MovePath.Length)
                return false;
            for (int i = 0; i < path.Count; i++)
                if (((BoardPosition)path[i]).X != move.MovePath[i].X || ((BoardPosition)path[i]).Y != move.MovePath[i].Y)
                    return false;
            return true;
        }

        bool Check(ArrayList test)
        {
            foreach (CheckersMove move in posiblemoves)
                if (IsPrefix(test, move))
                    return true;
            return false;
        }

        private void white_OnGameOver(bool winner)
        {
            RepaintBoard();
            System.Threading.Thread.Sleep(100);
            this.Refresh();
        }

        private void black_OnGameOver(bool winner)
        {
            RepaintBoard();
            System.Threading.Thread.Sleep(100);
            this.Refresh();

            //*Rayhand
            //SoundStorage.SoundPlayer.Play(SoundS["win"]);
            //*End
            if (winner) MessageBox.Show("Player 2 Win!");
            else MessageBox.Show("Player 1 Win!");
        }


        #region White implemenation
        private void white_OnPlay(IGameStateForPlayer gamestate)
        {
            this.CurrentPlayer = WhitePlayer;
        }

        private void white_OnOtherPlayerMovePerformed(IPlayer player, IMove move)
        {
            //this.Invalidate();
            //MessageBox.Show("negras jugaron");
            RepaintBoard();
            System.Threading.Thread.Sleep(100);
            this.Refresh();
        }

        private void white_OnInvalidMove(IMove move)
        {
            lastMoveOk = false;
            this.CurrentPlayer = WhitePlayer;
            //this.Invalidate();
            RepaintBoard();
        }

        #endregion

        #region black implementation
        private void black_OnPlay(IGameStateForPlayer gamestate)
        {
            this.CurrentPlayer = BlackPlayer;
        }

        private void black_OnOtherPlayerMovePerformed(IPlayer player, IMove move)
        {
            RepaintBoard();
            System.Threading.Thread.Sleep(100);
            this.Refresh();
        }

        private void black_OnInvalidMove(IMove move)
        {
            this.CurrentPlayer = BlackPlayer;
            lastMoveOk = false;
            RepaintBoard();
            //this.Invalidate();
        }

        #endregion

        private void StartGame(CheckersPlayer White, CheckersPlayer Black)
        {
            this.WhitePlayer = White;
            this.BlackPlayer = Black;
            this.MyBoard.InitializeBoard();
            CurrentPlayer = WhitePlayer;
            MyModerator = new CheckersModerator(this.MyBoard, White, Black);
            RepaintBoard();
            MyModerator.StartGame();
        }
        
        private void BoardPicturebox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(CurrentPlayer is HumanCheckersPlayer)) return;

            //else ...

            int xCell = (e.X) / CellWidth;
            int yCell = (e.Y) / CellHeight;
            
            bool xeven = (xCell % 2 == 0);
            bool yeven = (yCell % 2 == 0);

            if (!((xeven && yeven) || (!xeven && !yeven))) return;


            BoardPosition newpos = new BoardPosition(xCell, yCell);
            CheckersPiece piece = (CheckersPiece)MyBoard.GetPieceAt(newpos);

            //if the selected cell contains path piece of diferent color then return
            if ((piece != null) && piece.Color != CurrentPlayer.Color) return;

            //if the user is selected another piece of its color then clear the selection and select this piece
            if (piece != null && piece.Color == CurrentPlayer.Color && moves.Count > 0 && !moves[0].Equals(newpos))
            {
                NewMove(piece,newpos);
            }
            else
            {
                //not double cell consecuttively
                if ((moves.Count >= 1) && moves[moves.Count - 1].Equals(newpos)) return;

                if (currentPath==null || currentPath.Count==0)
                    NewMove(piece,newpos);
                else AddMoveToPath(newpos);
            }

            RepaintBoard();

            //*Rayhand
            GC.Collect();
            //*End
        }

        private void AddMoveToPath(BoardPosition newpos)
        {

            var p = new BoardPosition(newpos.X, newpos.Y);
            ArrayList newPath = currentPath.Clone() as ArrayList;
            newPath.Add(p);

            foreach (BoardPosition pos in currentPath)
                bdrawer.Selected[pos.X, pos.Y] = false;
            do
            {
                newPath.RemoveAt(newPath.Count - 1);
                newPath.Add(p);
                if (Check(newPath))
                {
                    currentPath = newPath;
                    moves.Add(newpos);
                    bdrawer.Selected[newpos.X, newpos.Y] = true;
                    break;
                }
                newPath.RemoveAt(newPath.Count - 1);
            } while (newPath.Count > 0);
            foreach (BoardPosition pos in currentPath)
                bdrawer.Selected[pos.X, pos.Y] = true;
        }

        private void NewMove(CheckersPiece piece, BoardPosition position)
        {
            moves.Clear();
            moves.Add(position);
            posiblemoves = piece.PossibleMoves;
            currentPath = new ArrayList();
            currentPath.Add(new BoardPosition(piece.X, piece.Y));
            bdrawer.ClearSelection();
            bdrawer.Selected[piece.X, piece.Y] = true;
        }

        private void MakeMove()
        {
            if (moves.Count <= 1) ResetMove();
            else
            {
                BoardPosition[] pos = new BoardPosition[moves.Count];
                moves.CopyTo(0, pos, 0, moves.Count);
                bool eatMove = CheckersMove.IsEatMove(this.MyBoard, pos);
                
                CheckersMove move = new CheckersMove(pos, eatMove);
                lastMoveOk = true;
                ((HumanCheckersPlayer)CurrentPlayer).MakeAMove(move);

                if (lastMoveOk) LastMove = move;
                else LastMove = null;

                CheckStatus();
                ResetMove();
            }
        }

        private void ResetMove()
        {
            this.moves.Clear();
            this.bdrawer.ClearSelection();
            currentPath.Clear();
            RepaintBoard();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            MakeMove();
        }
    }
}
