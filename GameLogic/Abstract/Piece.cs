using System;

namespace GameLogic.Abstract
{
    public abstract class Piece : ICloneable
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Board ParentBoard { get; set; }

        public Piece(Board parent, int x, int y)
        {
            this.X = x;
            this.Y = y;
            ParentBoard = parent;
        }

        public abstract bool CanMoveTo(int newx, int newy);
        
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
