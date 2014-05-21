namespace GameLogic.Abstract
{
    public interface IBoardEvaluator
    {
        double Eval(CheckersBoard board, int level, PieceColor playerColor);
    }
}
