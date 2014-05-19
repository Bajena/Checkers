using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoardEngine;
using GameLogic;

namespace Checkers
{
    /// <summary>
    /// Summary description for BoardDrawe.
    /// </summary>
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

        /// <summary>
        /// Paints the board over the current graphics
        /// </summary>
        /// <param name="g"></param>
        public virtual void PaintBoard(Graphics g, int startx, int starty, int width, int height)
        {
            int cellWidth = width / 8;
            int cellHeight = height / 8;

            Rectangle originalRect = new Rectangle(0, 0, 32, 32);
            bool White = false;
            Image cellimage = null;
            Image PieceImage = null;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    using (var brush = new SolidBrush(White ? Color.White : Color.Black))
                    {
                        g.FillRectangle(brush, new Rectangle(cellWidth * i + startx, cellHeight * j + starty, cellWidth, cellHeight));
                        
                        //draw the cell
                        //if (White) cellimage = WhiteCellImage;
                        //else cellimage = BlackCellImage;
                        //g.DrawImage(cellimage, new Rectangle(cellWidth * i + startx, cellHeight * j + starty, cellWidth, cellHeight), originalRect, GraphicsUnit.Pixel);

                        //now draw the piece
                        CheckersPiece piece = (CheckersPiece)this._Board.GetPieceAt(i, j);
                        if (piece != null) //if there is a piece there
                        {
                            //PieceImage = GetPieceColor(piece);
                            try
                            {
                                //Color transp = ((Bitmap)PieceImage).GetPixel(0, 0);
                                //ImageAttributes attr = new ImageAttributes();
                                // attr.SetColorKey(transp, transp);
                                //attr.SetColorKey(Color.FromArgb(248,248,240),Color.FromArgb(248,248,240));
                                //g.DrawImage(PieceImage, new Rectangle(cellWidth * i + startx, cellHeight * j + starty, cellWidth, cellHeight), 0, 0, 32, 32, GraphicsUnit.Pixel, attr);

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
