using System;
using GameLogic.Abstract;

namespace GameLogic
{
	public class CheckersModerator : Moderator
	{

		public CheckersModerator(CheckersBoard board, CheckersPlayer whiteplayer, CheckersPlayer blackplayer) : base(new IPlayer[2]{whiteplayer,blackplayer})
		{
			this.CurrentGameState = board;
		}

		protected override void InvokePlayMethod(IPlayer player)
		{
			try 
			{
				player.Play((IGameStateForPlayer)this.CurrentGameState);
			}
			catch(Exception ex) 
			{
				HaltGame(ex);
			}

		}

		public override bool IsWinner(IPlayer player)
		{
			if (!isGameFinished()) return false;
			CheckersPlayer myplayer = (CheckersPlayer)player;
			CheckersBoard MyBoard = (CheckersBoard)CurrentGameState;
			return (MyBoard.RightMoves(myplayer.Color).Count>0);
		}

		private int GetCloser(int k, int goal) 
		{
			if (k<goal) return k+1;
			else if (k>goal) return k-1;
			else return k;
		}

		protected override void PerformMove(IGameState state, IMove move)
		{
			CheckersBoard MyBoard = (CheckersBoard)state;
			CheckersMove MyMove = (CheckersMove)move;
			CheckersPiece piece = (CheckersPiece)MyBoard.GetPieceAt((BoardPosition)MyMove.MovePath[0]);
			if (piece!=null) 
			{
				MyBoard.RemovePiece(piece.X,piece.Y);
				
				//Clear all piece in the middle of the path (assuming that this method is called when the move is valid.
				for(int i=0;i<MyMove.MovePath.Length - 1; i++) 
				{
					//get the current position
                    BoardPosition current = (BoardPosition)MyMove.MovePath[i];
					BoardPosition next =  (BoardPosition)MyMove.MovePath[i+1];
					
					int cx = current.X;
					int cy = current.Y;
					
					//move close to reach the next position
					while(!(cx==next.X && cy == next.Y)) 
					{
						cx = GetCloser(cx,next.X);
						cy = GetCloser(cy,next.Y);
						bool removed = MyBoard.RemovePiece(cx,cy);
						if (removed && (piece.Color == PieceColor.Black)) white--;
						else if (removed && (piece.Color == PieceColor.White)) black--;

					}
				}

				BoardPosition pos=MyMove.MovePath[MyMove.MovePath.Length-1];

				if(pos.Y==0 && piece.Color==PieceColor.White)
				{
					//System.Windows.Forms.MessageBox.Show("Queen");
					MyBoard.PutPieceAt(pos, new Queen(MyBoard, pos.X, pos.Y, PieceColor.White));
				}
				else
					if(pos.Y==7 && piece.Color==PieceColor.Black)
				{
					//System.Windows.Forms.MessageBox.Show("Queen");
					MyBoard.PutPieceAt(pos, new Queen(MyBoard, pos.X, pos.Y, PieceColor.Black));
				}
				else
					MyBoard.PutPieceAt((BoardPosition)MyMove.MovePath[MyMove.MovePath.Length-1],piece);

			}
		}

		public override bool isValidMove(IPlayer player, IMove move)
		{
			CheckersBoard MyBoard = (CheckersBoard)this.CurrentGameState;
			CheckersMove MyMove = (CheckersMove)move;
			CheckersPlayer MyPlayer = (CheckersPlayer)player;
			CheckersPiece piece = (CheckersPiece)MyBoard.GetPieceAt((BoardPosition)MyMove.MovePath[0]);

			if (piece==null) return false;
			if (piece.Color != MyPlayer.Color) return false;
			
			int max=0;
			bool eatMove=false;

			for(int x=0; x<MyBoard.Width; x++)
				for(int y=0; y<MyBoard.Height; y++) {
					CheckersPiece aux=MyBoard.GetPieceAt(x, y) as CheckersPiece;
					if(aux!=null && aux.Color==piece.Color) {
						System.Collections.ArrayList list=aux.PossibleMoves;
						if(list.Count>0) {
							CheckersMove move2=list[0] as CheckersMove;
							if(max < move2.MovePath.Length) {
								max=move2.MovePath.Length;
								//System.Windows.Forms.MessageBox.Show("Max="+max);
							}
							if(move2.EatMove)
								eatMove=true;
						}
					}

				}

			//System.Windows.Forms.MessageBox.Show("Must eat: "+eatMove+" It eat:"+MyMove.EatMove);
			//System.Windows.Forms.MessageBox.Show("Max="+max+" move="+MyMove.MovePath.Length);
			bool canMakeIt=Contains(piece.PossibleMoves, MyMove);//bool canMakeIt=piece.CanMakeMove(MyMove);
			//System.Windows.Forms.MessageBox.Show("Can Make It: "+canMakeIt);
			return eatMove==MyMove.EatMove && MyMove.MovePath.Length==max && canMakeIt;
		}

		protected bool Contains(System.Collections.ArrayList list, CheckersMove move)
		{
			//System.Windows.Forms.MessageBox.Show(list.Count+"");
			foreach(CheckersMove m in list)
			{
				//System.Windows.Forms.MessageBox.Show(m.ToString()+'\n'+move.ToString());
				if(m.Equals(move))
					return true;
			}
			return false;
		}
		
		int white = 12;
		int black = 12;

		public override bool isGameFinished()
		{
			CheckersBoard board = (CheckersBoard)_CurrentGameState;
			CheckersPlayer player=(CheckersPlayer)this.CurrentPlayer;
			//return (white == 0 || black == 0);
			return board.RightMoves(player.Color).Count==0;
		
		}




	}
}
