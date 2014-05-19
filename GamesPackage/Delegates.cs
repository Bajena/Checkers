using System;
using System.Data;
using System.Collections;

namespace GamesPackage
{
	public delegate void GameOverDelegate(bool winner);
	public delegate void PlayDelegate(IGameStateForPlayer gamestate);
	public delegate void InvalidMoveDelegate(IMove move);	
    public delegate void PerformMove(IPlayer player, IMove move);
}
