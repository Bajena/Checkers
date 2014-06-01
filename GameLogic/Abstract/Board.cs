using System;

namespace GameLogic.Abstract
{
    public abstract class Board
    {
        protected Piece[,] _BoardMatrix;

        protected int _height;
        protected int _width;

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }


        public Board(int width, int heigth)
        {
            this._width = width;
            this._height = heigth;
            _BoardMatrix = new Piece[width, heigth];

        }


        public abstract void InitializeBoard();

        /// <summary>
        /// Removes a piece from a board given its coordinates and return true or false depeendig if there was a piece there or not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool RemovePiece(int x, int y)
        {
            if (!IsInsideBoard(x, y)) return false;

            if (_BoardMatrix[x, y] != null)
            {
                _BoardMatrix[x, y] = null;
                return true;
            }
            else return false;
        }


        public virtual bool IsInsideBoard(BoardPosition bp)
        {
            return IsInsideBoard(bp.X, bp.Y);
        }

        public virtual bool IsInsideBoard(int x, int y)
        {
            return ((0 <= x) && (x < Width) && (0 <= y) && (y < Height));
        }

        public bool RemovePiece(Piece p)
        {
            return RemovePiece(p.X, p.Y);
        }

        public bool RemovePiece(BoardPosition pos)
        {
            return RemovePiece(pos.X, pos.Y);
        }

        public virtual bool IsEmptyCell(int x, int y)
        {
            if (!IsInsideBoard(x, y)) return false;
            return (_BoardMatrix[x, y] == null);
        }

        public virtual void MovePieceTo(Piece p, int newx, int newy)
        {

            this.RemovePiece(p);
            this.PutPieceAt(newx, newy, p);

        }

        public virtual void PutPiece(Piece p)
        {
            PutPieceAt(p.X, p.Y, p);
        }

        public virtual void PutPieceAt(BoardPosition pos, Piece p)
        {
            PutPieceAt(pos.X, pos.Y, p);
        }

        public virtual void PutPieceAt(int x, int y, Piece p)
        {
            if (IsInsideBoard(x, y))
            {
                _BoardMatrix[x, y] = p;
                p.X = x;
                p.Y = y;
                p.ParentBoard = this;
            }
            else throw new Exception("Out of the board");
        }

        public Piece GetPieceAt(BoardPosition pos)
        {
            return GetPieceAt(pos.X, pos.Y);
        }

        public Piece GetPieceAt(int x, int y)
        {
            if (IsInsideBoard(x, y)) return _BoardMatrix[x, y];
            else return null;
        }



    }
}
