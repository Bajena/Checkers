using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Abstract;

namespace GameLogic
{
    internal class AreaBoardEvaluator : IBoardEvaluator
    {
        private readonly double _pawnStrengthArea1;
        private readonly double _queenStrengthArea1;
        private readonly double _pawnStrengthArea2;
        private readonly double _queenStrengthArea2;

        private readonly int _area2Width = 1;

        public AreaBoardEvaluator()
        {
            _queenStrengthArea2 = 12;
            _queenStrengthArea1 = 6;
            _pawnStrengthArea2 = 4;
            _pawnStrengthArea1 = 2;
        }

        public AreaBoardEvaluator(double pawnStrengthArea1, double queenStrengthArea1, double pawnStrengthArea2,
            double queenStrengthArea2)
        {
            _pawnStrengthArea1 = pawnStrengthArea1;
            _queenStrengthArea1 = queenStrengthArea1;
            _pawnStrengthArea2 = pawnStrengthArea2;
            _queenStrengthArea2 = queenStrengthArea2;
        }


        public double Eval(CheckersBoard board, int level, PieceColor playerColor)
        {
            double eval = 0.0;

            for (int x = 0; x < board.Width; x++)
                for (int y = 0; y < board.Height; y++)
                {
                    var piece = board.GetPieceAt(x, y) as CheckersPiece;
                    if (piece != null)
                    {
                        eval += GetPiecePoints(board,piece, playerColor);
                    }
                }

            return eval;
        }


        private double GetPiecePoints(CheckersBoard board,CheckersPiece piece, PieceColor playerColor)
        {
            int area = 1;
            if (piece.X <= _area2Width || piece.X >= board.Width - _area2Width || piece.Y <= _area2Width ||
                piece.Y >= board.Height - _area2Width) area = 2;

            double points = 0;
            if (piece is Pawn)
            {
                points = area == 2 ? _pawnStrengthArea2 : _pawnStrengthArea1;
            }
            else if (piece is Queen)
            {
                points = area == 2 ? _queenStrengthArea2 : _queenStrengthArea1;
            }

            return points * (piece.Color == playerColor ? 1 : -1);
        }
    }
}
