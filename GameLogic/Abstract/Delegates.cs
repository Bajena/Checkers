namespace GameLogic.Abstract
{
	public delegate void GameOverDelegate(bool winner);
	public delegate void PlayDelegate(IGameStateForPlayer gamestate);
	public delegate void InvalidMoveDelegate(IMove move);	
    public delegate void PerformMove(IPlayer player, IMove move);
}
