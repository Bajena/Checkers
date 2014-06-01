using System.Collections;
using System.Collections.Generic;
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

	        foreach (var piece in _BoardMatrix)
	        {
	            if (piece!=null && (piece as CheckersPiece).Color==color)
                    pieces.Add((CheckersPiece) piece);
	        }

	        return pieces;
	    }

		public ArrayList RightMoves(PieceColor color)
		{
			var moves=new ArrayList();
			var moves2=new ArrayList();
 
			//getting all the moves
			bool eatMove=false;
			int length=0;

			for(int x=0; x<this.Width; x++)
				for(int y=0; y<this.Height; y++) {
					CheckersPiece piece=this.GetPieceAt(x, y) as CheckersPiece;
					if(piece!=null && piece.Color==color)
						foreach(CheckersMove move in piece.PossibleMoves) {
							moves.Add(move);
							if(move.EatMove)
								eatMove=move.EatMove;
							if(move.MovePath.Length>length)
								length=move.MovePath.Length;
						}
				}

			//looking for right moves...
			for(int l=0; l<moves.Count; l++) {
				CheckersMove m2=moves[l] as CheckersMove;
				if(m2!=null && eatMove==m2.EatMove && length==m2.MovePath.Length)
					moves2.Add(m2);
			}
			return moves2;
		}

		public void Clear() 
		{
			WhiteLog.Clear();
			BlackLog.Clear();
			_BoardMatrix = new Piece[8,8];
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
