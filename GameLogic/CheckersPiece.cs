using System.Collections;
using GameLogic.Abstract;

namespace GameLogic
{
    public abstract class CheckersPiece : Piece
    {
        protected PieceColor _Color;
        public PieceColor Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }


        public CheckersPiece(CheckersBoard board, int x, int y, PieceColor color)
            : base(board, x, y)
        {
            this._Color = color;

        }

        /// <summary>
        /// When overriden on a derivated class should return if the give piece can be eaten by this piece or not 
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public abstract bool CanEat(CheckersPiece piece);

        public abstract bool CanMakeMove(CheckersMove move);

        public abstract ArrayList PossibleMoves
        {
            get;
        }

        #region PossiblePaths

        protected abstract System.Drawing.Point[] IncMov { get; }

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
            ArrayList listToFill = new ArrayList();

            for (int i = 0; i < IncMov.Length; i++)
            {
                bool outOfBoard = false;
                ArrayList path = MovementInDirection(IncMov[i], ref outOfBoard);

                if (!eat && path != null && path.Count > 0)
                    this.Restriction1(listToFill, path, new BoardPosition(this.X, this.Y), ref countMovement);
                if (outOfBoard) continue;

                if (!outOfBoard && path != null)
                {
                    BoardPosition f = (path.Count == 0)
                        ? (new BoardPosition(this.X + IncMov[i].X, this.Y + IncMov[i].Y))
                        : (new BoardPosition(((BoardPosition)path[path.Count - 1]).X + IncMov[i].X, ((BoardPosition)path[path.Count - 1]).Y + IncMov[i].Y));

                    if (!this._ParentBoard.IsInsideBoard(f.X, f.Y)) continue;
                    CheckersPiece piece = this._ParentBoard.GetPieceAt(f.X, f.Y) as CheckersPiece;

                    if (piece.Color != this.Color)
                    {
                        BoardPosition c = new BoardPosition(f.X + IncMov[i].X, f.Y + IncMov[i].Y);

                        if ((this._ParentBoard.IsInsideBoard(c.X, c.Y) && this._ParentBoard.IsEmptyCell(c.X, c.Y)))
                        {
                            this._ParentBoard.RemovePiece(f);
                            BoardPosition myPosition = new BoardPosition(this.X, this.Y);
                            this._ParentBoard.MovePieceTo(this, c.X, c.Y);

                            ArrayList tmp = this.PossibleMovements(true);
                            this.ConfigurePath(tmp, myPosition);
                            foreach (ArrayList al in tmp) 
                                listToFill.Add(al);

                            this._ParentBoard.MovePieceTo(this, myPosition.X, myPosition.Y);
                            this._ParentBoard.PutPieceAt(f.X, f.Y, piece);

                            countMovement = (tmp.Count > 0) ? countMovement + 1 : countMovement;
                            continue;
                        }
                    }
                }		
            }

            if (countMovement == 0)
                listToFill.Add(new ArrayList(new BoardPosition[] { new BoardPosition(this.X, this.Y) }));

            return listToFill;
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
