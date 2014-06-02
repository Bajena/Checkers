using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameLogic.Abstract;

namespace GameLogic
{
    public class Pawn : CheckersPiece
    {
        private int[] XMov;
        private int[] YMov;

        private int[] XEat = { -1, 1, -1, 1 };
        private int[] YEat = { -1, -1, 1, 1 };

        List<BoardPosition> aux;

        protected void EatMoves(int x, int y, List<CheckersMove> moves)
        {
            bool wayFound = false;

            aux.Add(new BoardPosition(x, y));

            for (int i = 0; i < XEat.Length; i++)
            {
                int newX = x + XEat[i];
                int newY = y + YEat[i];
                if (this.ParentBoard.IsInsideBoard(newX, newY))
                {
                    CheckersPiece piece = this.ParentBoard.GetPieceAt(newX, newY) as CheckersPiece;
                    if (piece != null && piece.Color != this.Color)
                    {
                        int newX2 = newX + XEat[i];
                        int newY2 = newY + YEat[i];

                        if (aux.Count > 1)
                        {
                            if (aux[aux.Count - 2].X == newX2 && aux[aux.Count - 2].Y == newY2)
                            continue;
                            
                        } 

                        if (this.ParentBoard.IsInsideBoard(newX2, newY2) &&
                            this.ParentBoard.GetPieceAt(newX2, newY2) == null)
                        {
                            wayFound = true;

                            this.ParentBoard.RemovePiece(this);
                            this.ParentBoard.PutPieceAt(newX2, newY2, this);
                            this.ParentBoard.RemovePiece(piece);

                            EatMoves(newX2, newY2, moves);

                            this.ParentBoard.PutPieceAt(newX, newY, piece);
                            this.ParentBoard.RemovePiece(this);
                            this.ParentBoard.PutPieceAt(x, y, this);
                        }
                    }
                }
            }

            if (!wayFound && (this.X != aux[0].X || this.Y != aux[0].Y))
            {
                BoardPosition[] arr = new BoardPosition[aux.Count];
                for (int j = 0; j < aux.Count; j++)
                    arr[j] = aux[j];
                moves.Add(new CheckersMove(arr, true));
            }

            aux.RemoveAt(aux.Count - 1);
        }

        protected void EatMoves(List<CheckersMove> moves)
        {
            aux = new List<BoardPosition>();
            EatMoves(X, Y, moves);
        }

        public override IList<CheckersMove> PossibleMoves
        {
            get { return CalculatePossibleMoves(); }
        }

        private List<CheckersMove> CalculatePossibleMoves()
        {
            var moves = new List<CheckersMove>();
            EatMoves(moves);
            if (moves.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int newX = this.X + XMov[i];
                    int newY = this.Y + YMov[i];
                    if (this.ParentBoard.IsInsideBoard(newX, newY))
                    {
                        var piece = this.ParentBoard.GetPieceAt(newX, newY) as CheckersPiece;
                        if (piece == null)
                            moves.Add(
                                new CheckersMove(
                                new BoardPosition[] {
															new BoardPosition(this.X, this.Y),
															new BoardPosition(newX, newY)
														},
                                false
                                )
                                );
                    }
                }
            }
            else
            {
                int max = 0;
                for (int j = 0; j < moves.Count; j++)
                {
                    CheckersMove move = moves[j] as CheckersMove;
                    if (move.MovePath.Length > max)
                        max = move.MovePath.Length;
                }
                for (int k = 0; k < moves.Count; k++)
                    if (((CheckersMove)moves[k]).MovePath.Length != max)
                        moves.RemoveAt(k--);
            }
            return moves;
        }

        public Pawn(CheckersBoard board, int x, int y, PieceColor color)
            : base(board, x, y, color)
        {
            if (Color == PieceColor.White)
            {
                XMov = new[] { -1, 1 };
                YMov = new[] { -1, -1 };
            }
            else if (color == PieceColor.Black)
            {
                XMov = new[] { 1, -1 };
                YMov = new[] { 1, 1 };
            }

        }

        public override bool CanEat(CheckersPiece piece)
        {
            return false;
        }

        protected bool MoveTo(int newx, int newy, bool firststep)
        {
            if (this.X == newx && this.Y == newy) return true;
            if (firststep)
            {
                for (int i = 0; i < 2; i++)
                {
                    int tempx = this.X + XMov[i];
                    int tempy = this.Y + YMov[i];
                    if ((tempx == newx && tempy == newy) && (ParentBoard.IsInsideBoard(tempx, tempy) && (ParentBoard.IsEmptyCell(tempx, tempy))))
                    {
                        return true;
                    }
                }
            }

            bool result = false;
            for (int i = 0; i < 2; i++)
            {
                int tempx = this.X + XMov[i];
                int tempy = this.Y + YMov[i];

                CheckersPiece piece = (CheckersPiece)ParentBoard.GetPieceAt(tempx, tempy);
                //if there is an enemy piece there and I can eat
                if (piece != null && (piece.Color != this.Color) && ParentBoard.IsEmptyCell(tempx + XMov[i], tempy + YMov[i]))
                {
                    //remove the piece I am going to eat
                    ParentBoard.RemovePiece(piece);
                    //move this piece to the future position
                    int oldx = this.X;
                    int oldy = this.Y;
                    ParentBoard.RemovePiece(this);
                    ParentBoard.PutPieceAt(tempx + XMov[i], tempy + YMov[i], this);
                    //call this method recursively
                    result = result || MoveTo(newx, newy, false);

                    //backtrack
                    //move this piece back
                    ParentBoard.RemovePiece(this);
                    ParentBoard.PutPieceAt(oldx, oldy, this);
                    //put the other piece agai on its position.
                    ParentBoard.PutPiece(piece);

                    if (result) break;
                }

            }

            return result;

        }

        public override bool CanMoveTo(int newx, int newy)
        {

            if (!ParentBoard.IsInsideBoard(newx, newy)) return false;
            if (newx == this.X && this.Y == newy) return false;
            return MoveTo(newx, newy, true);


        }

        //public bool CanMakeMove(CheckersMove move, int start = 0)
        //{

        //    if (start == move.MovePath.Length - 1)
        //        return true;

        //    var nextPosition = move.MovePath[start + 1];

        //    //if is a simple move
        //    if (move.MovePath.Length == 2 && start == 0)
        //    {
        //        for (int i = 0; i < 2; i++)
        //        {
        //            int tempx = this.X + XMov[i];
        //            int tempy = this.Y + YMov[i];
        //            if ((tempx == nextPosition.X && tempy == nextPosition.Y) && (_ParentBoard.IsInsideBoard(tempx, tempy) && (_ParentBoard.IsEmptyCell(tempx, tempy))))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    bool result = false;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        int tempx = this.X + XMov[i];
        //        int tempy = this.Y + YMov[i];
        //        //System.Windows.Forms.MessageBox.Show(tempx + "," + tempy);
        //        if ((tempx + XMov[i] == nextPosition.X && tempy + YMov[i] == nextPosition.Y))
        //        {
        //            CheckersPiece piece = (CheckersPiece)_ParentBoard.GetPieceAt(tempx, tempy);
        //            //if there is an enemy piece there and I can eat
        //            if (piece != null && (piece.Color != this.Color) && _ParentBoard.IsEmptyCell(tempx + XMov[i], tempy + YMov[i]))
        //            {
        //                //at the moment the move is valid
        //                result = true;

        //                //remove the piece I am going to eat
        //                _ParentBoard.RemovePiece(piece);
        //                //move this piece to the future position
        //                int oldx = this.X;
        //                int oldy = this.Y;
        //                _ParentBoard.RemovePiece(this);
        //                _ParentBoard.PutPieceAt(tempx + XMov[i], tempy + YMov[i], this);

        //                //call this method recursively
        //                result = result && CanMakeMove(move, start + 1);

        //                //backtrack
        //                //move this piece back
        //                _ParentBoard.RemovePiece(this);
        //                _ParentBoard.PutPieceAt(oldx, oldy, this);
        //                //put the other piece agai on its position.
        //                _ParentBoard.PutPiece(piece);
        //            }
        //        }
        //    }

        //    return result;
        //}

        //public override bool CanMakeMove(CheckersMove move)
        //{
        //    //System.Windows.Forms.MessageBox.Show(ShowBoardPosition(this.PossiblePaths));
        //    return CanMakeMove(move);
        //}

        public override object Clone()
        {
            return new Pawn((CheckersBoard)this.ParentBoard, this.X, this.Y, this.Color);
        }

        protected override ArrayList MovementInDirection(Point increment, ref bool outOfBoard)
        {
            var path = new ArrayList();
            BoardPosition pos = null;

            pos = new BoardPosition(this.X + increment.X, this.Y + increment.Y);
            if (this.ParentBoard.IsInsideBoard(pos.X, pos.Y))
            {
                if (!this.ParentBoard.IsEmptyCell(pos.X, pos.Y))
                {
                    outOfBoard = false;
                    return path;
                }
                path.Add(pos);
            }
            outOfBoard = true;
            return path;
        }

        protected override System.Drawing.Point[] IncMov
        {
            get
            {
                return new[]
                {
                    new System.Drawing.Point(this.XMov[0], this.YMov[0]), new System.Drawing.Point(this.XMov[1], this.YMov[1])
                };
            }
        }

        #region USED IN DEBUG MODE
        static string ShowBoardPosition(BoardPosition[][] b)
        {
            string mov = "" + b.Length + "\n";
            foreach (BoardPosition[] p in b)
                mov += ShowPath(p) + "\n";
            return mov;
        }

        static string ShowPath(BoardPosition[] dest)
        {
            string mov = "";
            foreach (BoardPosition p in dest)
                mov += "[" + p.X + "," + p.Y + "],";
            return mov;
        }
        #endregion
    }
}
