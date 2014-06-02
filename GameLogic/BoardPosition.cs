using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class BoardPosition
    {
        public int X;
        public int Y;
        public BoardPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool Equals(BoardPosition bp1, BoardPosition bp2)
        {
            if (bp1 != null) return bp1.Equals(bp2);
            if (bp2 != null) return bp2.Equals(bp2);
            return (bp1 == bp2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoardPosition)) return false;
            else
            {
                return ((((BoardPosition)obj).X == X) && (((BoardPosition)obj).Y == Y));

            }
        }

        public override int GetHashCode()
        {

            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return "(" + this.X + ";" + this.Y + ")";
        }
    }
}
