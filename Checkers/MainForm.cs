using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameLogic;
using System.Collections;
using GameLogic.Abstract;

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
        BoardPosition _lastSelectedPosition = new BoardPosition(0, 4);
        bool _lastMoveOk = false;
        readonly ArrayList _moves = new ArrayList();
        CheckersMove _lastMove = null;
        int _computerLevel = 2;
        Bitmap _boardBitmap;
        private BoardDrawer _boardDrawer;
        IList<CheckersMove> _posiblemoves;
        private List<BoardPosition> _currentPath;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeBoardDrawing()
        {
            _boardBitmap = new Bitmap(BoardPicturebox.Width, BoardPicturebox.Height);
            Graphics.FromImage(_boardBitmap).FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, _boardBitmap.Width, _boardBitmap.Height));
            BoardPicturebox.Image = _boardBitmap;
            RepaintBoard();
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
            using (Graphics g = Graphics.FromImage(BoardPicturebox.Image))
            {
                _boardDrawer.PaintBoard(g, 0, 0, BoardPicturebox.Width, BoardPicturebox.Height);
                this.BoardPicturebox.Image = _boardBitmap;
            }
        }
        private void CheckStatus()
        {
            //if the lat move was valid
            if (_lastMoveOk)
            {
                //update to queen if can be
                BoardPosition lastposition = (BoardPosition)_lastMove.MovePath[_lastMove.MovePath.Length - 1];
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
        bool IsPrefix(List<BoardPosition> path, CheckersMove move)
        {
            if (path.Count > move.MovePath.Length)
                return false;
            for (int i = 0; i < path.Count; i++)
                if (path[i].X != move.MovePath[i].X || path[i].Y != move.MovePath[i].Y)
                    return false;
            return true;
        }

        bool Check(List<BoardPosition> test)
        {
            foreach (CheckersMove move in _posiblemoves)
                if (IsPrefix(test, move))
                    return true;
            return false;
        }

        private void white_OnGameOver(bool winner)
        {
            RepaintBoard();
            //System.Threading.Thread.Sleep(100);
            this.Refresh();
        }

        private void black_OnGameOver(bool winner)
        {
            RepaintBoard();
            //System.Threading.Thread.Sleep(100);
            this.Refresh();

            MessageBox.Show(winner ? "Player 2 Win!" : "Player 1 Win!");
        }


        #region White implemenation
        private void white_OnPlay(IGameStateForPlayer gamestate)
        {
            this.CurrentPlayer = WhitePlayer;
        }

        private void white_OnOtherPlayerMovePerformed(IPlayer player, IMove move)
        {
            moveButton.Enabled = WhitePlayer is HumanCheckersPlayer;

            this.Invalidate();
            RepaintBoard();
            //MessageBox.Show("White moved");
            this.Refresh();
            System.Threading.Thread.Sleep(1000);
        }

        private void white_OnInvalidMove(IMove move)
        {
            _lastMoveOk = false;
            this.CurrentPlayer = WhitePlayer;
            this.Invalidate();
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
            //MessageBox.Show("Black moved");
            this.Refresh();
            System.Threading.Thread.Sleep(1000);
        }

        private void black_OnInvalidMove(IMove move)
        {
            this.CurrentPlayer = BlackPlayer;
            _lastMoveOk = false;
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
            if (piece != null && piece.Color == CurrentPlayer.Color && _moves.Count > 0 && !_moves[0].Equals(newpos))
            {
                NewMove(piece,newpos);
            }
            else
            {
                //not double cell consecuttively
                if ((_moves.Count >= 1) && _moves[_moves.Count - 1].Equals(newpos)) return;

                if (_currentPath==null || _currentPath.Count==0)
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
            var newPath = new List<BoardPosition>(_currentPath);
           
            newPath.Add(p);

            foreach (BoardPosition pos in _currentPath)
                _boardDrawer.Selected[pos.X, pos.Y] = false;
            do
            {
                newPath.RemoveAt(newPath.Count - 1);
                newPath.Add(p);
                if (Check(newPath))
                {
                    _currentPath = newPath;
                    _moves.Add(newpos);
                    _boardDrawer.Selected[newpos.X, newpos.Y] = true;
                    break;
                }
                newPath.RemoveAt(newPath.Count - 1);
            } while (newPath.Count > 0);
            foreach (BoardPosition pos in _currentPath)
                _boardDrawer.Selected[pos.X, pos.Y] = true;
        }

        private void NewMove(CheckersPiece piece, BoardPosition position)
        {
            _moves.Clear();
            _moves.Add(position);
            _posiblemoves = piece.PossibleMoves;
            
            _currentPath = new List<BoardPosition>();
            _currentPath.Add(new BoardPosition(piece.X, piece.Y));
            _boardDrawer.ClearSelection();
            _boardDrawer.Selected[piece.X, piece.Y] = true;
        }

        private void MakeMove()
        {
            if (_moves.Count <= 1) ResetMove();
            else
            {
                var pos = new BoardPosition[_moves.Count];
                _moves.CopyTo(0, pos, 0, _moves.Count);
                bool eatMove = CheckersMove.IsEatMove(this.MyBoard, pos);
                
                var move = new CheckersMove(pos, eatMove);
                _lastMoveOk = true;
                ((HumanCheckersPlayer)CurrentPlayer).MakeAMove(move);

                if (_lastMoveOk) _lastMove = move;
                else _lastMove = null;

                CheckStatus();
                ResetMove();
            }
        }

        private void ResetMove()
        {
            this._moves.Clear();
            this._boardDrawer.ClearSelection();
            _currentPath.Clear();
            RepaintBoard();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            MakeMove();
        }

        private void BoardPicturebox_Resize(object sender, EventArgs e)
        {
            InitializeBoardDrawing();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Show();
            NewGame();
        }

        private void NewGame()
        {

            MyBoard = new CheckersBoard();
            MyBoard.InitializeBoard();

            _boardDrawer = new BoardDrawer(this.MyBoard);
            _boardDrawer.SelectionColor = Color.Yellow;

            //HumanCheckersPlayer white = new HumanCheckersPlayer(PieceColor.White);
            SimpleCheckersPlayer white = new SimpleCheckersPlayer(PieceColor.White, new AreaBoardEvaluator());
            white.MaxSearchDepth = 3;
            white.OnPerformMove += white_OnPerformMove;
            //white.OnPlay += white_OnPlay;
            white.OnOtherPlayerMovePerformed += white_OnOtherPlayerMovePerformed;
            white.OnInvalidMove += white_OnInvalidMove;
            white.OnGameOver += white_OnGameOver;

            SimpleCheckersPlayer black = new SimpleCheckersPlayer(PieceColor.Black, new PawnStrengthBoardEvaluator());
            black.MaxSearchDepth = 3;
            black.OnPerformMove += black_OnPerformMove;
            //black.OnPlay+=black_OnPlay;
            black.OnOtherPlayerMovePerformed += black_OnOtherPlayerMovePerformed;
            black.OnInvalidMove += black_OnInvalidMove;
            black.OnGameOver += black_OnGameOver;

            InitializeBoardDrawing();
            StartGame(white, black);
        }

        private void nowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}
