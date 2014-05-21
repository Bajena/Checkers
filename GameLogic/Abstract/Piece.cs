using System;

namespace GameLogic.Abstract
{
    public abstract class Piece : ICloneable
    {
        internal protected int _X;
        internal protected int _Y;

        public int X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }

        public int Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }

        protected Board _ParentBoard;

        public Board ParentBoard
        {
            get
            {
                return _ParentBoard;
            }
            set
            {
                if (value != null) _ParentBoard = value;
            }
        }

        public Piece(Board parent, int x, int y)
        {
            this._X = x;
            this._Y = y;
            _ParentBoard = parent;
        }

        public abstract bool CanMoveTo(int newx, int newy);

        public bool CanMoveTo(BoardPosition newPos)
        {
            return CanMoveTo(newPos.X, newPos.Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Piece)) return false;
            Piece p = (Piece)obj;
            return p.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return this.X + "," + this.Y;
        }
        #region ICloneable Members

        public abstract object Clone();


        #endregion
    }
}
