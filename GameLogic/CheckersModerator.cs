using System;
using System.Linq;
using GameLogic.Abstract;

namespace GameLogic
{
	public class CheckersModerator : Moderator
	{

        int _whitePieces = 12;
        int _blackPieces = 12;

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
						if (removed && (piece.Color == PieceColor.Black)) _whitePieces--;
						else if (removed && (piece.Color == PieceColor.White)) _blackPieces--;

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

		public override bool IsValidMove(IPlayer player, IMove move)
		{
			var checkersBoard = (CheckersBoard)CurrentGameState;
			var checkersMove = (CheckersMove)move;
			var checkersPlayer = (CheckersPlayer)player;
			var startPiece = (CheckersPiece)checkersBoard.GetPieceAt(checkersMove.MovePath[0]);

			if (startPiece==null) return false;
			if (startPiece.Color != checkersPlayer.Color) return false;
			
			int maxEat=0;
			bool eatMove=false;

		    foreach (var piece in checkersBoard.GetPiecesOfColor(startPiece.Color))
		    {
                var possibleMoves = piece.PossibleMoves;
                if (possibleMoves.Count > 0)
                {
                    var otherMove = possibleMoves[0] as CheckersMove;
                    if (maxEat < otherMove.MovePath.Length)
                    {
                        maxEat = otherMove.MovePath.Length;
                        //System.Windows.Forms.MessageBox.Show("Max="+max);
                    }
                    if (otherMove.EatMove)
                        eatMove = true;
                }
		    }

			//System.Windows.Forms.MessageBox.Show("Must eat: "+eatMove+" It eat:"+MyMove.EatMove);
			//System.Windows.Forms.MessageBox.Show("Max="+max+" move="+MyMove.MovePath.Length);
			bool isPossible = startPiece.PossibleMoves.Cast<CheckersMove>().Contains(checkersMove);
			//System.Windows.Forms.MessageBox.Show("Can Make It: "+canMakeIt);
			return 
                eatMove==checkersMove.EatMove && 
                checkersMove.MovePath.Length==maxEat && 
                isPossible;
		}
        
		public override bool isGameFinished()
		{
			CheckersBoard board = (CheckersBoard)_CurrentGameState;
			CheckersPlayer player=(CheckersPlayer)CurrentPlayer;
			//return (white == 0 || black == 0);
			return board.RightMoves(player.Color).Count==0;
		
		}




	}
}
