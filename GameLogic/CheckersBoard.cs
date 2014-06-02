using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Abstract;

namespace GameLogic
{

	public class CheckersBoard : Board,IGameStateForPlayer,IGameState
	{
		#region private and protected variables
		
		internal ArrayList BlackLog = new ArrayList(12);
		internal ArrayList WhiteLog = new ArrayList(12);


		#endregion

		public CheckersBoard() : base(8,8) 
		{
		}

	    public List<CheckersPiece> GetPiecesOfColor(PieceColor color)
	    {
	       var pieces = new List<CheckersPiece>();

	        foreach (var piece in BoardMatrix)
	        {
	            if (piece!=null && (piece as CheckersPiece).Color==color)
                    pieces.Add((CheckersPiece) piece);
	        }

	        return pieces;
	    }

		public IList<CheckersMove> RightMoves(PieceColor color)
		{
			var moves=new List<CheckersMove>();

		    //getting all the moves
			bool eatMoveExists=false;
			int maxLength = 0;

		    foreach (var checkersPiece in GetPiecesOfColor(color))
		    {
		        foreach(CheckersMove move in checkersPiece.PossibleMoves) {
					moves.Add(move);
		            if (move.EatMove)
		                eatMoveExists = true;

					if(move.MovePath.Length>maxLength)
						maxLength=move.MovePath.Length;
				}
		    }

			//looking for right moves...
		    return moves.Where(move => eatMoveExists == move.EatMove && maxLength == move.MovePath.Length).ToList();
		}

		public void Clear() 
		{
			WhiteLog.Clear();
			BlackLog.Clear();
			BoardMatrix = new Piece[8,8];
		}

		public override void InitializeBoard()
		{
			this.Clear();
			
			for(int i=0 ; i<8 ; i++) 
			{
				for(int j=0; j < 8; j++ )
				{
					bool xeven = (i % 2 == 0);
					bool yeven = (j % 2 == 0);
					if (!(xeven ^ yeven))
					{
						if (j<=2) PutPieceAt(i,j,new Pawn(this,i,j,PieceColor.Black));
						else if(j>=5) PutPieceAt(i,j,new Pawn(this,i,j,PieceColor.White));
					}
				}
			}


		}
		
		public object Clone() {			
			CheckersBoard newBoard=new CheckersBoard();
			for(int x=0; x<newBoard.Width; x++)
				for(int y=0; y<newBoard.Height; y++) {
					CheckersPiece piece=this.GetPieceAt(x, y) as CheckersPiece;
					if(piece!=null) {
						CheckersPiece clone=(CheckersPiece)piece.Clone();
						newBoard.PutPieceAt(x, y, clone);
					}
				}
			return newBoard;
		}

		public override string ToString()
		{
			string s="";
			foreach(CheckersPiece piece in WhiteLog) {
				s+="w"+piece.ToString();
			}
				s+='\n';

			foreach(CheckersPiece piece in BlackLog) {
				s+="b"+piece.ToString();
			}
			
			return s;
		}

		public override bool Equals(object obj) {
			CheckersBoard other = (CheckersBoard)obj;
			return (this.ToString().Equals(other.ToString()));
		}

	}
	
}
