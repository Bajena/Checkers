using System;

namespace GameLogic.Abstract
{
    public enum PlayerGameResult
    {
        NotEnded,
        Win,
        Lose,
        Draw
    }
	/// <summary>
	/// Defines what a player should implement
	/// </summary>
	public interface IPlayer 
	{
		/// <summary>
		/// Called by the moderator when this player tried to make a move and was an invalid one.
		/// </summary>
		/// <param name="move"></param>
		void InvalidMove(IMove move);

		/// <summary>
		/// Called by moderator indicating the player that the games is over and idicating also if this player won or loose.
		/// </summary>
		/// <param name="result"></param>
        void GameOver(PlayerGameResult result);


		/// <summary>
		/// Called when the game end by any other mistake like network crashes, or any kind of failure
		/// </summary>
		void GameHalted(Exception cause);


		/// <summary>
		/// Called by moderator to inform this player that another player has performed a move sucefully
		/// </summary>
		/// <param name="player"></param>
		/// <param name="move"></param>
		void MovedPerformed(IPlayer player, IMove move);


	    /// <summary>
	    /// Called by the moderator indicating the player that is his turn to play, 
	    /// </summary>
	    /// <param name="state">The game state this player need to know, the player must use this gameState for playing/thinkin his move and then raise the even OnPerformMove</param>
	    /// <param name="gameState"></param>
	    void Play(IGameStateForPlayer gameState);

		/// <summary>
		/// Event raised by the player to perform a move, the moderator must be suscribed to this event
		/// </summary>
		event PerformMove OnPerformMove;


	}

    
}
