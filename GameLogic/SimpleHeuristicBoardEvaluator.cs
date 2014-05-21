using GameLogic.Abstract;

namespace GameLogic
{
    public class SimpleHeuristicBoardEvaluator : IBoardEvaluator
    {
        public double Eval(CheckersBoard board, int level, PieceColor playerColor)
        {
            {
                double myval = 0;
                double enemyval = 0;
                for (int x = 0; x < board.Width; x++)
                    for (int y = 0; y < board.Height; y++)
                    {
                        CheckersPiece piece = board.GetPieceAt(x, y) as CheckersPiece;
                        if (piece != null)
                        {
                            int factor = (piece.Color == PieceColor.White) ? (7 - y) : (y);
                            if (piece.Color == playerColor)
                            {
                                if (piece is Pawn) myval += 100 + (factor * factor);
                                else
                                {

                                    myval += 200;
                                    if (y == 0)
                                    {
                                        if (x == 0) myval -= 40;
                                        else myval -= 20;
                                    }
                                    else if (y == 7)
                                    {
                                        if (x == 7) myval -= 40;
                                        else myval -= 20;
                                    }
                                }
                            }
                            else
                            {
                                if (piece is Pawn) enemyval += 100 + (factor * factor);
                                else
                                {
                                    enemyval += 200;
                                    if (y == 0)
                                    {
                                        if (x == 0) enemyval -= 40;
                                        else enemyval -= 20;
                                    }
                                    else if (y == 7)
                                    {
                                        if (x == 7) enemyval -= 40;
                                        else enemyval -= 20;
                                    }
                                }
                            }
                        }
                    }

                if (enemyval == 0) return 100000 + level * level;
                else if (myval == 0) return -100000 - level * level;
                return (myval - enemyval);
            }
        }
    }
}
