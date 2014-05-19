using BoardEngine;

namespace GameLogic
{
	/// <summary>
	/// Summary description for SmartPlayer.
	/// </summary>
	public class SmartPlayer
	{
		public SmartPlayer()
		{
			
		}

		/// <summary>
		/// give a board returns the "best" move that the piece of that color should do
		/// </summary>
		/// <param name="board"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		public static CheckersMove GetBestMove(CheckersBoard board,PieceColor color) 
		{
			for(int i=0;i<8;i++) 
			{
				for(int j=0;j<8;j++) 
				{
					CheckersPiece piece = (CheckersPiece)board.GetPieceAt(i,j);
					//if there is a piece there
					if (piece!=null) 
					{
						
					}
				}
			}
			return null;
		}
	}
}
