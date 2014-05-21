using GameLogic.Abstract;

namespace GameLogic
{
    public class PawnStrengthBoardEvaluator : IBoardEvaluator
    {
        private readonly double _ownPawnStrength;
        private readonly double _ownQueenStrength;
        private readonly double _enemyPawnStrength;
        private readonly double _enemyQueenStrength;

        public PawnStrengthBoardEvaluator(double ownPawnStrength, double ownQueenStrength, double enemyPawnStrength, double enemyQueenStrength)
        {
            _ownPawnStrength = ownPawnStrength;
            _ownQueenStrength = ownQueenStrength;
            _enemyPawnStrength = enemyPawnStrength;
            _enemyQueenStrength = enemyQueenStrength;
        }

        public PawnStrengthBoardEvaluator()
        {
            _ownPawnStrength = _enemyPawnStrength = 1;
            _ownQueenStrength = _enemyQueenStrength = 2;
        }

        public double Eval(CheckersBoard board, int level, PieceColor playerColor)
        {
            {
                double eval = 0;

                for (int x = 0; x < board.Width; x++)
                    for (int y = 0; y < board.Height; y++)
                    {
                        var piece = board.GetPieceAt(x, y) as CheckersPiece;
                        if (piece != null)
                        {
                            if (piece.Color == playerColor)
                            {
                                if (piece is Pawn) eval += _ownPawnStrength;
                                else if (piece is Queen) eval += _ownQueenStrength;
                            }
                            else
                            {
                                if (piece is Pawn) eval -= _ownPawnStrength;
                                else if (piece is Queen) eval -= _ownQueenStrength;
                            }
                        }
                    }

                return eval;
            }
        }
    }
}

