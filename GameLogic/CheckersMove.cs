using System;
using GameLogic.Abstract;

namespace GameLogic
{
	public class CheckersMove:IMove
	{
		public bool EatMove;
		public BoardPosition[] MovePath;

		public override string ToString()
		{
			string s=this.EatMove+" ";
			for(int i=0; i<this.MovePath.Length; i++)
				s+=this.MovePath[i];
			return s;
		}

		public override bool Equals(object obj)
		{
			CheckersMove move=obj as CheckersMove;
			if(obj!=null)
			{
				if(this.EatMove==move.EatMove && this.MovePath.Length==move.MovePath.Length)
				{
					for(int i=0; i<this.MovePath.Length; i++)
						if(!this.MovePath[i].Equals(move.MovePath[i]))
							return false;
					return true;
				}
				else
					return false;
			}
			else
				throw new ArgumentException();
		}

		public CheckersMove(BoardPosition[] positions, bool EatMove)
		{
			this.MovePath = positions;
			this.EatMove=EatMove;
		}

		protected static int Inc(int a, int b)
		{
			if(a>b)
				return -1;
			else if(a==b)
				return 0;
			else
				return 1;
		}

		public static bool IsEatMove(CheckersBoard board, BoardPosition[] pos)
		{
			if(board!=null && pos!=null && pos.Length>=2)
			{
				int x1=pos[0].X;
				int y1=pos[0].Y;
				//System.Windows.Forms.MessageBox.Show(x1+" "+y1);
				if(board.IsInsideBoard(x1, y1))
				{
					CheckersPiece piece=board.GetPieceAt(x1, y1) as CheckersPiece;
					if(piece!=null)
					{
						int x2=pos[1].X;
						int y2=pos[1].Y;
						//System.Windows.Forms.MessageBox.Show(x2+" "+y2);
						if(board.IsInsideBoard(x2, y2))
						{
							if(Math.Abs(x1-x2)==Math.Abs(y1-y2))
							{
								int incX=Inc(x1, x2);
								int incY=Inc(y1, y2);
								//System.Windows.Forms.MessageBox.Show(incX+" "+incY);
								x1+=incX;
								y1+=incY;
								//System.Windows.Forms.MessageBox.Show(x1+" "+y1);
								while(x1!=x2 || y1!=y2)
								{
									CheckersPiece piece2=board.GetPieceAt(x1, y1) as CheckersPiece;
									if(piece2!=null)
									{
										//System.Windows.Forms.MessageBox.Show(x1+" "+y1);
										//System.Windows.Forms.MessageBox.Show(piece.Color+" - "+piece2.Color);
										return piece.Color!=piece2.Color;
									}
									x1+=incX;
									y1+=incY;
									//System.Windows.Forms.MessageBox.Show(x1+" "+y1);
								}
							}
							return false;
						}
						else
							throw new ArgumentException();
					}
					else
						throw new ArgumentException();
				}
				else
					throw new ArgumentException();
			}
			else
				throw new ArgumentException();
		}
	}
}
