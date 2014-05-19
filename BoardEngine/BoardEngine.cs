using System;
using System.Data;


namespace BoardEngine
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
		
		protected Board  _ParentBoard;

		public Board ParentBoard 
		{
			get 
			{
				return _ParentBoard;
			}
			set 
			{
				if (value!=null) _ParentBoard = value;
			}
		}

		public Piece(Board parent,int x,int y)
		{
			this._X = x;
			this._Y = y;
			_ParentBoard = parent;
		}

		public abstract bool CanMoveTo(int newx,int newy);

		public bool CanMoveTo(BoardPosition newPos) 
		{
			return CanMoveTo(newPos.X,newPos.Y);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Piece)) return false;
			Piece p = (Piece)obj;
			return p.ToString()==this.ToString();
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// since can not be 2 piece in the same place this can be used has tostring and also as hashcode
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.X+ ","+this.Y;
		}
		#region ICloneable Members

		public abstract object Clone();
		

		#endregion
	}

	public class BoardPosition 
	{
		public int X;
		public int Y;
		public BoardPosition(int x,int y) 
		{
			this.X = x;
			this.Y = y;
		}

		public static bool Equals(BoardPosition bp1, BoardPosition bp2)
		{
			if (bp1!=null) return bp1.Equals(bp2);
			if (bp2!=null) return bp2.Equals(bp2);
			return (bp1==bp2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BoardPosition)) return false;
			else 
			{
				return ((((BoardPosition)obj).X==this.X) && (((BoardPosition)obj).Y == this.Y));

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

	public enum PieceColor 
	{
		White = 0,
		Black = 1
	}


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


		public Board(int width,int heigth) 
		{
			this._width = width;
			this._height = heigth;
			_BoardMatrix = new Piece[width,heigth];
			
		}


		public abstract void InitializeBoard(); 
		
		/// <summary>
		/// Removes a piece from a board given its coordinates and return true or false depeendig if there was a piece there or not
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public virtual bool RemovePiece(int x,int y) 
		{
			if (!IsInsideBoard(x,y)) return false;

			if (_BoardMatrix[x,y]!=null) 
			{
				_BoardMatrix[x,y]=null;
				return true;
			}
			else return false;
		}


		public virtual bool IsInsideBoard(BoardPosition bp) 
		{
			return IsInsideBoard(bp.X,bp.Y);
		}

		public virtual bool IsInsideBoard(int x,int y) 
		{
			return ((0<=x) && (x<Width) && (0<=y) && (y<Height));
		}

		/// <summary>
		/// Removes the given piece form the current board
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool RemovePiece(Piece p)
		{
			return RemovePiece(p.X,p.Y);
		}

		/// <summary>
		/// Removes the piece at that position
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public bool RemovePiece(BoardPosition pos) 
		{
			return RemovePiece(pos.X,pos.Y);
		}

		public virtual bool IsEmptyCell(int x,int y) 
		{
			if (!IsInsideBoard(x,y)) return false;
			return (_BoardMatrix[x,y] == null);
		}

		public virtual void MovePieceTo(Piece p,int newx,int newy) 
		{
			
			this.RemovePiece(p);
			this.PutPieceAt(newx,newy,p);
			
		}

		public virtual void PutPiece(Piece p) 
		{
			PutPieceAt(p.X,p.Y,p);
		}

		public virtual void PutPieceAt(BoardPosition pos,Piece p) 
		{
			PutPieceAt(pos.X,pos.Y,p);
		}

		public virtual void PutPieceAt(int x,int y,Piece p) 
		{
			if (IsInsideBoard(x,y)) {
				_BoardMatrix[x,y]=p;
				p._X = x;
				p._Y = y;
				p.ParentBoard = this;
			}
			else throw new Exception("Out of the board");
		}

		public Piece GetPieceAt(BoardPosition pos) 
		{
			return GetPieceAt(pos.X,pos.Y);
		}

		public Piece GetPieceAt(int x,int y) 
		{
			if (IsInsideBoard(x,y))	return _BoardMatrix[x,y];
			else return null;
		}


		
	}
}
