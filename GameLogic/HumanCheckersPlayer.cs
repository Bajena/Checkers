using System;
using BoardEngine;
using GamesPackage;

namespace GameLogic
{
	public interface CheckersPlayer : IPlayer
	{
		PieceColor Color 
		{
			get; set;
		}
	}
	/// <summary>
	/// Summary description for HumanChessPlayer.
	/// </summary>
	public class HumanCheckersPlayer: CheckersPlayer	
	{

		#region private and protected variables
		protected PieceColor _Color;
		
		#endregion

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
		/// Creates an instance of the HumanChessPlayer class given its Player Color (Black or White)
		/// </summary>
		/// <param name="color">The piece's color for this player</param>
		/// <param name="board">The board that will be used for playing</param>
		public HumanCheckersPlayer(PieceColor color)
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

		
		public event PlayDelegate OnPlay;
		public void Play(IGameStateForPlayer info)
		{
			if (OnPlay!=null) 
			{
				OnPlay(info);			
			}
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
