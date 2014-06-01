using System;
using System.Drawing;
using System.Windows.Forms;
using GameLogic;

namespace Checkers
{
    public class BoardDrawer
    {
        private Pen SelectionPen;
        protected Color _SelectionColor;
        public Color SelectionColor
        {
            get
            {
                return _SelectionColor;
            }
            set
            {
                _SelectionColor = value;
            }
        }

        protected CheckersBoard _Board;

        public bool[,] Selected = new bool[8, 8];

        public BoardDrawer(CheckersBoard board)
        {
            this._Board = board;
            this.SelectionColor = Color.Red;
            this.SelectionPen = new Pen(SelectionColor);
        }

        public virtual Color GetPieceColor(CheckersPiece piece)
        {
            if (piece is Pawn) return piece.Color == PieceColor.White ? Color.LightBlue : Color.MediumPurple;
            else if (piece is Queen) return piece.Color == PieceColor.White ? Color.Blue : Color.Purple;
            else return Color.Empty;
        }

        public virtual void PaintBoard(Graphics g, int startx, int starty, int width, int height)
        {
            int cellWidth = width / 8;
            int cellHeight = height / 8;

            bool White = false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    using (var brush = new SolidBrush(White ? Color.White : Color.Black))
                    {
                        g.FillRectangle(brush, new Rectangle(cellWidth * i + startx, cellHeight * j + starty, cellWidth, cellHeight));
                        
                        CheckersPiece piece = (CheckersPiece)this._Board.GetPieceAt(i, j);
                        if (piece != null)
                        {
                            try
                            {
                                brush.Color = GetPieceColor(piece);
                                g.FillEllipse(brush, new Rectangle(cellWidth * i + startx, cellHeight * j + starty, cellWidth, cellHeight));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                        if (Selected[i, j])
                        {
                            g.DrawRectangle(SelectionPen, cellWidth * i + startx + 2, cellHeight * j + starty + 2, cellWidth - 4, cellHeight - 4);
                            g.DrawRectangle(SelectionPen, cellWidth * i + startx + 1, cellHeight * j + starty + 1, cellWidth - 2, cellHeight - 2);
                        }

                        White = !White;
                    }
                }
                White = !White;
            }
        }

        public void ClearSelection()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Selected[i, j] = false;
                }
            }
        }

    }
}
