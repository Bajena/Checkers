using System;
using GameLogic.Abstract;

namespace GameLogic
{
	public interface CheckersPlayer : IPlayer
	{
		PieceColor Color 
		{
			get; set;
		}
	}

	public class HumanCheckersPlayer: CheckersPlayer	
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


		public HumanCheckersPlayer(PieceColor color)
		{
			this._Color = color;			
			
		}

		public event PerformMove OnOtherPlayerMovePerformed;

		public void MovedPerformed(IPlayer player, IMove move)
		{
			if (OnOtherPlayerMovePerformed!=null) OnOtherPlayerMovePerformed(player,move);
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

		public event PerformMove OnPerformMove;
		public event InvalidMoveDelegate OnInvalidMove;
		public void InvalidMove(IMove move)
		{
			if (OnInvalidMove!=null) OnInvalidMove(move);
			
		}

	}
}
