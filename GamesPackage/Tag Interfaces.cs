using System;
using System.Data;
using System.Collections;

namespace GamesPackage
{
	
	
    /// <summary>
    /// Tag interface defined for marking the Class that should represent the state of the whole game
    /// </summary>
	public interface IGameState : ICloneable
	{
		
	}


	/// <summary>
	/// Tag interface defined to mark the state of a game that a player should know in order to play
	/// </summary>
	public interface IGameStateForPlayer
	{
	}

	/// <summary>
	/// Tag interface defined for marking a clas that should represent a Move
	/// </summary>
	public interface IMove 
	{
	}
}
