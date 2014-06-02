using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using GameLogic.Abstract;

namespace GameLogic
{
    public abstract class CheckersPiece : Piece
    {
        public PieceColor Color { get; set; }

        public CheckersPiece(CheckersBoard board, int x, int y, PieceColor color)
            : base(board, x, y)
        {
            this.Color = color;

        }

        /// <summary>
        /// When overriden on a derivated class should return if the give piece can be eaten by this piece or not 
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public abstract bool CanEat(CheckersPiece piece);

        //public abstract bool CanMakeMove(CheckersMove move);

        public abstract IList<CheckersMove> PossibleMoves
        {
            get;
        }

        #region PossiblePaths

        protected abstract Point[] IncMov { get; }

        public virtual BoardPosition[][] PossiblePaths
        {
            get
            {
                ArrayList listToFill = this.PossibleMovements(false);

                BoardPosition[][] bps = new BoardPosition[listToFill.Count][];
                for (int i = 0; i < listToFill.Count; i++)
                    bps[i] = (BoardPosition[])(((ArrayList)listToFill[i]).ToArray(typeof(BoardPosition)));
                return bps;
            }
        }

        protected ArrayList PossibleMovements(bool eat)
        {
            int countMovement = 0;
            var possibleMovements = new ArrayList();

            for (int i = 0; i < IncMov.Length; i++)
            {
                bool outOfBoard = false;
                ArrayList path = MovementInDirection(IncMov[i], ref outOfBoard);

                if (!eat && path != null && path.Count > 0)
                    this.Restriction1(possibleMovements, path, new BoardPosition(this.X, this.Y), ref countMovement);
                if (outOfBoard) continue;

                if (!outOfBoard && path != null)
                {
                    BoardPosition f = (path.Count == 0)
                        ? (new BoardPosition(this.X + IncMov[i].X, this.Y + IncMov[i].Y))
                        : (new BoardPosition(((BoardPosition)path[path.Count - 1]).X + IncMov[i].X, ((BoardPosition)path[path.Count - 1]).Y + IncMov[i].Y));

                    if (!this.ParentBoard.IsInsideBoard(f.X, f.Y)) continue;
                    CheckersPiece piece = this.ParentBoard.GetPieceAt(f.X, f.Y) as CheckersPiece;

                    if (piece.Color != this.Color)
                    {
                        BoardPosition c = new BoardPosition(f.X + IncMov[i].X, f.Y + IncMov[i].Y);

                        if ((this.ParentBoard.IsInsideBoard(c.X, c.Y) && this.ParentBoard.IsEmptyCell(c.X, c.Y)))
                        {
                            this.ParentBoard.RemovePiece(f);
                            BoardPosition myPosition = new BoardPosition(this.X, this.Y);
                            this.ParentBoard.MovePieceTo(this, c.X, c.Y);

                            ArrayList tmp = this.PossibleMovements(true);
                            this.ConfigurePath(tmp, myPosition);
                            foreach (ArrayList al in tmp) 
                                possibleMovements.Add(al);

                            this.ParentBoard.MovePieceTo(this, myPosition.X, myPosition.Y);
                            this.ParentBoard.PutPieceAt(f.X, f.Y, piece);

                            countMovement = (tmp.Count > 0) ? countMovement + 1 : countMovement;
                            continue;
                        }
                    }
                }		
            }

            if (countMovement == 0)
                possibleMovements.Add(new ArrayList(new BoardPosition[] { new BoardPosition(this.X, this.Y) }));

            return possibleMovements;
        }


        protected abstract System.Collections.ArrayList MovementInDirection(System.Drawing.Point increment, ref bool outOfBoard);

        protected virtual void ConfigurePath(ArrayList listToFill, BoardPosition p)
        {
            if (listToFill != null)
            {
                for (int i = 0; i < listToFill.Count; i++)
                {
                    ArrayList al = listToFill[i] as ArrayList;
                    if (al != null)
                        al.Insert(0, p);
                }
            }
        }


        private void Restriction1(ArrayList listToFill, ArrayList path, BoardPosition p, ref int countMovement)
        {
            ArrayList list = null;
            for (int i = 0; i < path.Count; i++)
            {
                list = new ArrayList();
                list.Add(p);
                list.Add(path[i]);
                listToFill.Add(list);
            }
            countMovement++;
        }


        #endregion PossiblePaths

    }
}
