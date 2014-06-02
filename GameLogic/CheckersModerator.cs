using System;
using System.Linq;
using GameLogic.Abstract;

namespace GameLogic
{
	public class CheckersModerator : Moderator
	{
        int _whitePieces = 12;
        int _blackPieces = 12;

	    private int _movesWithoutChange = 0;
	    private const int MaxMovesWithoutChange = 50;

	    protected bool IsDraw
	    {
	        get
	        {
	            return _movesWithoutChange >= MaxMovesWithoutChange;
	            
	        }
	    }

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

		public override PlayerGameResult GetPlayerGameResult(IPlayer player)
		{
			if (!isGameFinished()) 
                return PlayerGameResult.NotEnded;
			var checkersPlayer = (CheckersPlayer)player;
			var currentGameState = (CheckersBoard)CurrentGameState;

		    if (IsDraw)
		        return PlayerGameResult.Draw;

		    return (currentGameState.RightMoves(checkersPlayer.Color).Count > 0)
		        ? PlayerGameResult.Win
		        : PlayerGameResult.Lose;
		}

		private int GetCloser(int k, int goal) 
		{
			if (k<goal) 
                return k+1;
		    if (k>goal) 
                return k-1;
		    return k;
		}

		protected override void PerformMove(IGameState state, IMove move)
		{
			CheckersBoard board = (CheckersBoard)state;
			CheckersMove checkersMove = (CheckersMove)move;
			CheckersPiece startPiece = (CheckersPiece)board.GetPieceAt(checkersMove.MovePath[0]);

			if (startPiece!=null) 
			{
				board.RemovePiece(startPiece.X,startPiece.Y);
				
				for(int i=0;i<checkersMove.MovePath.Length - 1; i++) 
				{
                    var currentPosition = checkersMove.MovePath[i];
					var nextPosition =  checkersMove.MovePath[i+1];
					
					int currentX = currentPosition.X;
					int currentY = currentPosition.Y;
					
					while(!(currentX==nextPosition.X && currentY == nextPosition.Y)) 
					{
						currentX = GetCloser(currentX,nextPosition.X);
						currentY = GetCloser(currentY,nextPosition.Y);
						bool removed = board.RemovePiece(currentX,currentY);

						if (removed && (startPiece.Color == PieceColor.Black)) _whitePieces--;
						else if (removed && (startPiece.Color == PieceColor.White)) _blackPieces--;

					}
				}

				BoardPosition pos=checkersMove.MovePath[checkersMove.MovePath.Length-1];

				if(pos.Y==0 && startPiece.Color==PieceColor.White)
				{
					board.PutPieceAt(pos, new Queen(board, pos.X, pos.Y, PieceColor.White));
				}
				else
					if(pos.Y==7 && startPiece.Color==PieceColor.Black)
				{
					board.PutPieceAt(pos, new Queen(board, pos.X, pos.Y, PieceColor.Black));
				}
				else
					board.PutPieceAt((BoardPosition)checkersMove.MovePath[checkersMove.MovePath.Length-1],startPiece);

			    if (checkersMove.EatMove)
			        _movesWithoutChange = -1;

			    _movesWithoutChange++;

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
                    }
                    if (otherMove.EatMove)
                        eatMove = true;
                }
		    }

			bool isPossible = startPiece.PossibleMoves.Cast<CheckersMove>().Contains(checkersMove);
			return 
                eatMove==checkersMove.EatMove && 
                checkersMove.MovePath.Length==maxEat && 
                isPossible;
		}
        
		public override bool isGameFinished()
		{
			CheckersBoard board = (CheckersBoard)CurrentGameState;
			CheckersPlayer player=(CheckersPlayer)CurrentPlayer;

			return board.RightMoves(player.Color).Count==0 || IsDraw;
		
		}
	}
}
