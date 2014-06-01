using System;
using System.Windows.Forms;
using GameLogic.Abstract;

namespace GameLogic
{
	/// <summary>
	/// Summary description for SimpleCheckersPlayer.
	/// </summary>
	public class SimpleCheckersPlayer: CheckersPlayer
    {
        public int MaxSearchDepth = 2;

		protected PieceColor _color;
        private IBoardEvaluator _boardEvaluator;
        CheckersMove selectedMove;
        Random r = new Random();



		public PieceColor Color 
		{
			get 
			{
				return  _color;
			}
			set 
			{
			}
		}
		#region Constructors

	    public SimpleCheckersPlayer(PieceColor color, IBoardEvaluator boardEvaluator)
		{
		    this._color = color;
		    _boardEvaluator = boardEvaluator;
		}

	    #endregion
		#region IPlayer Members

		public event PerformMove OnOtherPlayerMovePerformed;

		public void MovedPerformed(IPlayer player, IMove move)
		{
			if (OnOtherPlayerMovePerformed!=null) OnOtherPlayerMovePerformed(player,move);
		}

		public void MakeAMove(CheckersMove move) 
		{
			if (OnPerformMove!=null) OnPerformMove(this,move);
		}


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

			CheckersPiece movingPiece=(CheckersPiece)newBoard.GetPieceAt(x1, y1);

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

            //White queen
			if(y1==0 && movingPiece.Color==PieceColor.White)
			{
				newBoard.PutPieceAt(x1, y1, new Queen(newBoard, x1, y1, PieceColor.White));
			}
			else if(y1==7 && movingPiece.Color==PieceColor.Black) //black queen
				{
					newBoard.PutPieceAt(x1, y1, new Queen(newBoard, x1, y1, PieceColor.Black));
				}
			else //simply move piece
				newBoard.PutPieceAt(x1, y1, movingPiece);

			return newBoard;
		}

		protected double SelectMove(CheckersBoard board, PieceColor color, int currentSearchDepth, double alpha, double beta)
		{
			System.Collections.ArrayList moves=board.RightMoves(color);
			
			if(currentSearchDepth==0 || moves.Count==0)
				return _boardEvaluator.Eval(board, currentSearchDepth,Color);
			else
			{
				//if have only one oportunnity the return this one and save time
				if (currentSearchDepth == MaxSearchDepth && (moves.Count == 1)) {
					selectedMove = moves[0] as CheckersMove;
					return 0;
                }
                				
				double max=(this.Color==color)? double.MinValue: double.MaxValue;
				for(int i=0;i<moves.Count;i++)
				{
					CheckersMove move = moves[i] as CheckersMove;

					CheckersBoard newBoard=PerformMove(board, move);
                    
					double eval=SelectMove(newBoard, color == PieceColor.White ? PieceColor.Black : PieceColor.White, currentSearchDepth-1,alpha,beta);
					
					if(color==this.Color) {
						if(max<eval) 
						{
							max=eval;

							if(currentSearchDepth==MaxSearchDepth) 
							{
								selectedMove=move;
								//System.Windows.Forms.MessageBox.Show("Got a move "+selectedMove + " currentSearchDepth :"+MaxSearchDepth);
							}
						
							if (beta<=max) break;
							else alpha = Math.Max(alpha,max);
						}
						else if(max==eval)
							{
								if((currentSearchDepth==MaxSearchDepth) && (r.Next(9)%2==0))  
								{
									selectedMove=move;
									//System.Windows.Forms.MessageBox.Show("Got a move "+selectedMove + " currentSearchDepth :"+MaxSearchDepth);
								}
							}
					}
					else {
						if(max>eval) max=eval;

						if (alpha>=max) break;
						else beta = Math.Min(beta,max);

					}
					//System.Windows.Forms.MessageBox.Show(newBoard.ToString()+'\n'+"Level "+currentSearchDepth+" Color "+color+" Max "+max+" eval "+eval);
				}
				return max;
			}
		}
		

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
					SelectMove(board, this.Color, MaxSearchDepth,Int32.MinValue,Int32.MaxValue);
//					DateTime endtime = DateTime.Now;
//					TimeSpan span = endtime - begintime;
//					System.Windows.Forms.MessageBox.Show(span.ToString());
//				
				}catch(Exception ex) {
						MessageBox.Show(ex.Message);
				}
				this.MakeAMove(selectedMove);
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

		public event PerformMove OnPerformMove;
		public event InvalidMoveDelegate OnInvalidMove;
		public void InvalidMove(IMove move)
		{
			if (OnInvalidMove!=null) OnInvalidMove(move);
		}

		#endregion
	}
}