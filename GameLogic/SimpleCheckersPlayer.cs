using System;
using System.Windows.Forms;
using BoardEngine;
using GamesPackage;

namespace GameLogic
{
	/// <summary>
	/// Summary description for SimpleCheckersPlayer.
	/// </summary>
	public class SimpleCheckersPlayer: CheckersPlayer	
	{
		protected PieceColor _Color;
		
		public PieceColor Color 
		{
			get 
			{
				return  _Color;
			}
			set 
			{
			}
		}
		#region Constructors

		/// <summary>
		/// Creates an instance of the SimpleChessPlayer class given its Player Color (Black or White)
		/// </summary>
		/// <param name="color">The piece's color for this player</param>
		public SimpleCheckersPlayer(PieceColor color)
		{
			this._Color = color;			
			
		}

		#endregion
		#region IPlayer Members

		public event PerformMove OnOtherPlayerMovePerformed;

		public void MovedPerformed(IPlayer player, IMove move)
		{
			if (OnOtherPlayerMovePerformed!=null) OnOtherPlayerMovePerformed(player,move);
			// TODO:  Add HumanCheckersPlayer.MovedPerformed implementation
		}

		public void MakeAMove(CheckersMove move) 
		{
			if (OnPerformMove!=null) OnPerformMove(this,move);
		}

		CheckersMove sMove;
		Random r=new Random();

		protected int Inc(int a, int b)
		{
			if(a<b)
				return 1;
			else
				if(a==b)
					return 0;
				else
					return -1;
		}

		protected CheckersBoard PerformMove(CheckersBoard board, CheckersMove move)
		{
			CheckersBoard newBoard=(CheckersBoard)board.Clone();

			int x1=move.MovePath[0].X;
			int y1=move.MovePath[0].Y;

			CheckersPiece mPiece=(CheckersPiece)newBoard.GetPieceAt(x1, y1);

			for(int i=0; i<move.MovePath.Length-1; i++)
			{
				int x2=move.MovePath[i+1].X;
				int y2=move.MovePath[i+1].Y;
				int incX=Inc(x1, x2);
				int incY=Inc(y1, y2);

				while(x1!=x2 || y1!=y2)
				{
					CheckersPiece piece=newBoard.GetPieceAt(x1, y1) as CheckersPiece;
					if(piece!=null)
					{
						newBoard.RemovePiece(piece);
					}
					x1+=incX;
					y1+=incY;
				}
			}

			if(y1==0 && mPiece.Color==PieceColor.White)
			{
				//System.Windows.Forms.MessageBox.Show("Queen");
				newBoard.PutPieceAt(x1, y1, new Queen(newBoard, x1, y1, PieceColor.White));
			}
			else
				if(y1==7 && mPiece.Color==PieceColor.Black)
				{
					//System.Windows.Forms.MessageBox.Show("Queen");
					newBoard.PutPieceAt(x1, y1, new Queen(newBoard, x1, y1, PieceColor.Black));
				}
			else
				newBoard.PutPieceAt(x1, y1, mPiece);

			return newBoard;
		}

		protected double SelectMove(CheckersBoard board, PieceColor color, int level, double alpha, double beta)
		{
			System.Collections.ArrayList moves=board.RightMoves(color);
			
			
			//System.Windows.Forms.MessageBox.Show(moves.Count +" posibles jugadas");
			if(level==0 || moves.Count==0)
				return Eval(board, level);
			else
			{
				//if have only one oportunnity the return this one and save time
				if (level == LEVELS+(int)this.Color && (moves.Count == 1)) {
					sMove = moves[0] as CheckersMove;
					return 0;
                }
                				
				double max=(this.Color==color)? double.MinValue: double.MaxValue;
				for(int i=0;i<moves.Count;i++)
				{
					CheckersMove move = moves[i] as CheckersMove;

					CheckersBoard newBoard=PerformMove(board, move);
                    
					double eval=SelectMove(newBoard, (PieceColor)(((int)color+1)%2), level-1,alpha,beta);
												
					if(color==this.Color) {
						if(max<eval) 
						{
							max=eval;

							if(level==LEVELS+(int)this.Color) 
							{
								sMove=move;
								//System.Windows.Forms.MessageBox.Show("Got a move "+sMove + " level :"+LEVELS);
							}
						
							//**** Added by Leonardo
							if (beta<=max) break;
							else alpha = Math.Max(alpha,max);
							//****
						}
						else
							if(max==eval)
							{
								if((level==LEVELS+(int)this.Color) && (r.Next(9)%2==0))  
								{
									sMove=move;
									//System.Windows.Forms.MessageBox.Show("Got a move "+sMove + " level :"+LEVELS);
								}
							}
					}
					else {
						if(max>eval) max=eval;

						///****** Added by leonardo
						if (alpha>=max) break;
						else beta = Math.Min(beta,max);
						///****

					}
					//System.Windows.Forms.MessageBox.Show(newBoard.ToString()+'\n'+"Level "+level+" Color "+color+" Max "+max+" eval "+eval);
				}
				return max;
			}
		}
		
		protected double Eval(CheckersBoard board, int level)
		{
			
			double myval = 0;
			double enemyval = 0;
			for(int x=0; x<board.Width; x++)
				for(int y=0; y<board.Height; y++)
				{
					CheckersPiece piece=board.GetPieceAt(x, y) as CheckersPiece;
					if(piece!=null)
					{
						int factor  = (piece.Color == PieceColor.White)?(7-y):(y);
						if (piece.Color == this.Color) 
						{
							if (piece is Pawn) myval+=100 + (factor*factor);
							else 
							{
								
								myval+=200;
								if (y==0) 
								{
									if (x==0) myval-=40;
									else myval-=20;
								}
								else if (y == 7) 
								{
									if (x==7) myval-=40;
									else myval-=20;                                                                        
								}
							}
						}
						else 
						{
							if (piece is Pawn) enemyval+=100 + (factor*factor);
							else  
							{
								enemyval+=200;
								if (y==0) 
								{
									if (x==0) enemyval-=40;
									else enemyval-=20;
								}
								else if (y == 7) 
								{
									if (x==7) enemyval-=40;
									else enemyval-=20;                                                                        
								}
							}
						}
					}
				}

			if (enemyval == 0) return 100000 + level*level;
			else if (myval == 0) return -100000 - level*level;
			return (myval - enemyval);
		}

		public int LEVELS=2;

		//public event PlayDelegate OnPlay;
		public void Play(IGameStateForPlayer info)
		{
			//if (OnPlay!=null) 
			//{
			//	OnPlay(info);			
			//}

			CheckersBoard board=info as CheckersBoard;
		//	System.Windows.Forms.MessageBox.Show((board == null) + "");
			if(board!=null)
			{
				try {
//					DateTime begintime = DateTime.Now;
					SelectMove(board, this.Color, LEVELS+(int)this.Color,Int32.MinValue,Int32.MaxValue);
//					DateTime endtime = DateTime.Now;
//					TimeSpan span = endtime - begintime;
//					System.Windows.Forms.MessageBox.Show(span.ToString());
//				
				}catch(Exception ex) {
						MessageBox.Show(ex.Message);
				}
				this.MakeAMove(sMove);
			}
			else
				throw new ArgumentException();
		}

		public event GameOverDelegate OnGameOver;
		public void GameOver(bool winner)
		{
			if (OnGameOver!=null) OnGameOver(winner);
		}

		public void GameHalted(Exception cause)
		{
			
		}

		public event GamesPackage.PerformMove OnPerformMove;

		public event InvalidMoveDelegate OnInvalidMove;
		public void InvalidMove(IMove move)
		{
			if (OnInvalidMove!=null) OnInvalidMove(move);
		}

		#endregion
	}
}