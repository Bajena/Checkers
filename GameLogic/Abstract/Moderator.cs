using System;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.Abstract
{
	/// <summary>
	/// Describes a moderator for turn-based games
	/// </summary>
	public abstract class Moderator 
	{
		#region Private Variables
		/// <summary>
		/// Contains the list of IPlayer instances that this moderator handles for the game
		/// </summary>
		ArrayList PlayersList = new ArrayList();

		ArrayList StateList = new ArrayList();


	    /// <summary>
		/// The index of the current state
		/// </summary>
		int _CurrentStateIndex = 0;

		

		/// <summary>
		/// Contains the index of the next player
		/// </summary>
		int _NextPlayerIndex = 0;

		#endregion

		#region properties

	    /// <summary>
	    /// When Overriden on a derivated class should gets or sets the current GameState 
	    /// </summary>
	    public IGameState CurrentGameState { get; set; }

	    /// <summary>
		/// Gets tha IPlayer instace of the next player
		/// </summary>
		public IPlayer NextPlayer 
		{
			get 
			{
				return (IPlayer)PlayersList[_NextPlayerIndex];
			}
		}

		/// <summary>
		/// Gets tha IPlayer instace of the current player
		/// </summary>
		public IPlayer CurrentPlayer 
		{
			get 
			{
				return (IPlayer)PlayersList[DecreaseIndex(_NextPlayerIndex)];
			}
		}
		#endregion 
	
		#region Constructors
		/// <summary>
		/// Creates an isntance of the moderator class by giving the player that will participate (in order)
		/// </summary>
		/// <param name="players"></param>
		public Moderator(IEnumerable<IPlayer> players) 
		{
			foreach(IPlayer player in players) 
			{
				//suscribe the moderator to the player's OnPerfomMove Event
				player.OnPerformMove+=player_OnPerformMove;
				//Add the player to the list
				PlayersList.Add(player);
			}
		}

		#endregion
	
		#region private and protected methods

		#region abstract methods

	    /// <summary>
	    /// When overriden on a derivated class should determine wheter a player is a winner or not.
	    /// if the game is not yet over, this method should return false
	    /// </summary>
	    /// <param name="player"></param>
	    /// <returns></returns>
	    public abstract PlayerGameResult GetPlayerGameResult(IPlayer player);

		/// <summary>
		/// When Overriden on a derivated class should perform a call the the play method of a player passing the information that 
		/// only that player needs to know for playing, ie: in dominoes the players only need to know pieces on the board
		/// and the pieces on hand, not more.
		/// </summary>
		/// <param name="player"></param>
		protected abstract void InvokePlayMethod(IPlayer player);
		
		/// <summary>
		/// When overriden on a derivated class should perform the Move over the given GameState
		/// </summary>
		/// <param name="state"></param>
		/// <param name="move"></param>
		protected abstract void PerformMove(IGameState state, IMove move);

		#endregion

		#region implemented methods
		/// <summary>
		/// Increase the value of the index in circles, according to the length of the PlayerList, ie: if the index is in the last element (Length-1) and you call this method it will return you 0, otherwise it will return index+1
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected int AdvanceIndex(int index) 
		{
			if (index == PlayersList.Count - 1) return 0;
			else return index + 1;
		}


		/// <summary>
		/// Decrease the value of the index in circles, according to the length of the PlayerList, ie
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected int DecreaseIndex(int index) 
		{
			if (index == 0 ) return PlayersList.Count - 1;
			else return index - 1;
		}

		/// <summary>
		/// called when the game is over because of game reasons (a player won !!)
		/// </summary>
		protected virtual void GameOver() 
		{
			foreach(IPlayer player in PlayersList)
			{
			    player.GameOver(GetPlayerGameResult(player));
			}
		}



		/// <summary>
		/// Halt the game by sending calling hte halt method of all players, with the passed exception
		/// </summary>
		/// <param name="ex"></param>
		public virtual void HaltGame(Exception ex) 
		{
			foreach(IPlayer player in PlayersList) 
			{
					player.GameHalted(ex);
			}
		}


		/// <summary>
		/// Called by the OnPerformMove event on players every time a player is about to perform a move
		/// </summary>
		/// <param name="move"></param>
		private void player_OnPerformMove(IPlayer sender, IMove move)
		{
			if (IsValidMove(sender,move)) 
			{
				//Make the move
				PerformMove(move);

				//Inform all player that this player perfomed a move suscefully
				foreach(IPlayer player in PlayersList)
				{
				    if (player.Equals(sender)) continue;

				    try 
				    {
				        player.MovedPerformed(sender,move);
				    }
				    catch(Exception ex) 
				    {
				        HaltGame(ex);
				    }
				}

                if (isGameFinished())
                {
                    GameOver();
                    return;
                }

				//Increase the values of next player
				_NextPlayerIndex = AdvanceIndex(_NextPlayerIndex);


				//Call the current player Play method
				InvokePlayMethod(NextPlayer);

			}
			else 
			{
				///Comunicate the player that the move was not valid
				sender.InvalidMove(move);
			}

		}

		/// <summary>
		/// Perform the moves over the current GameState
		/// </summary>
		/// <param name="move"></param>
		protected virtual void PerformMove(IMove move) 
		{
			//Saves the state to the list..
			AppendState(this.GetGameStateClone());
						
			//perform the move over the current state so the current game state will change
			this.PerformMove(this.CurrentGameState,move);
		}

		/// <summary>
		/// append an state to the list of states
		/// </summary>
		/// <param name="state"></param>
		private void AppendState(IGameState state) 
		{
			InstantGameState gamephoto = new InstantGameState();
			gamephoto.NextPlayerIndex = _NextPlayerIndex;
			gamephoto.GameState = state;
			if (_CurrentStateIndex<StateList.Count - 1) 
			{
                StateList.RemoveRange(_CurrentStateIndex+1,StateList.Count - _CurrentStateIndex);
				StateList.Add(gamephoto);
			}
			else 
			{
				StateList.Add(gamephoto);
				_CurrentStateIndex++;
			}
		}

		/// <summary>
		/// Restores the game to a saved state
		/// </summary>
		/// <param name="state"></param>
		private void RestoreFromState(InstantGameState state) 
		{
			this.CurrentGameState = state.GameState;
			this._NextPlayerIndex = state.NextPlayerIndex;
		}

		#endregion

		#endregion

		#region Public methods


		#region abstract methods
		/// <summary>
		/// When overriden on a derivated class should determine if a move for a player is valid or not. NOTE: This method does not perform any move
		/// </summary>
		/// <param name="player">the player that want to make the move</param>
		/// <param name="move">the move to be performed</param>
		/// <returns></returns>
		public abstract bool IsValidMove(IPlayer player , IMove move);


		/// <summary>
		/// When overriden on a derivated class should say wheter the games is finished or not (by normal reasons)
		/// </summary>
		/// <returns></returns>
		public abstract bool isGameFinished();


		#endregion

		#region implemented methods
		/// <summary>
		/// Starts the game by calling the Play(Imove) Method of the first player in the list
		/// </summary>
		public void StartGame() 
		{
	
			_NextPlayerIndex = 0;

			//get the first player
			IPlayer player = (IPlayer)PlayersList[_NextPlayerIndex];

			//Save the current state
			AppendState(this.GetGameStateClone());

			//invoke the first player to play
			this.InvokePlayMethod(this.NextPlayer);
		}

		/// <summary>
		/// Returns a copy of the current GameState
		/// </summary>
		/// <returns></returns>
		public IGameState GetGameStateClone() 
		{
			return (IGameState)CurrentGameState.Clone();
		}

		/// <summary>
		/// This method undo the last move and go to the previous state, if any
		/// </summary>
		public void Undo() 
		{
			if (_CurrentStateIndex > 0) 
			{
				_CurrentStateIndex--;
				RestoreFromState( (InstantGameState) StateList[_CurrentStateIndex] );
			}
		}


		/// <summary>
		/// This method goes to the next state if any
		/// </summary>
		public void Redo() 
		{
			if (_CurrentStateIndex < StateList.Count - 1)
			{
				_CurrentStateIndex ++ ;
				RestoreFromState( (InstantGameState) StateList[_CurrentStateIndex] );
			}
		}

		#endregion

		#endregion

		#region InnerClass used

		struct InstantGameState 
		{
			public IGameState GameState;
			public int NextPlayerIndex;

		}

		#endregion
	}
}
