using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardEngine;

namespace GameLogic
{
    public abstract class CheckersPiece : Piece
    {
        protected PieceColor _Color;
        public PieceColor Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }


        public CheckersPiece(CheckersBoard board, int x, int y, PieceColor color)
            : base(board, x, y)
        {
            this._Color = color;

        }

        /// <summary>
        /// When overriden on a derivated class should return if the give piece can be eaten by this piece or not 
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public abstract bool CanEat(CheckersPiece piece);

        public abstract bool CanMakeMove(CheckersMove move);

        public abstract ArrayList PossibleMoves
        {
            get;
        }

        #region PossiblePaths

        protected abstract System.Drawing.Point[] IncMov { get; }

        public virtual BoardPosition[][] PossiblePaths
        {
            get
            {
                ArrayList listToFill = this.PossibleMovements(false);

                BoardPosition[][] bps = new BoardPosition[listToFill.Count][];
                for (int i = 0; i < listToFill.Count; i++)
                    bps[i] = (BoardPosition[])(((ArrayList)listToFill[i]).ToArray(typeof(BoardPosition)));
                return bps;
            }
        }


        /*
        * Para cada una de las 4 posibles direcciones en que puedo moverme:
        *	-	Me muevo en esa direccion hasta que se salga del tablero, o hasta que se encuentre una ficha.
        *	-	Si pare porque me sali del tablero
        *		-	(1): Si no habia comido antes, entonces encontre el camino maximo en esa direccion [BINGO :-)]
        *				 En caso contrario desecho el camino (si ya habia comido antes, no puedo moverte ahora sin comer)[FAIL :-(]
        *	-	Si pare porque encontre otra ficha
        *		-	Calculo la casilla de la otra ficha (sea F esa casilla)
        *		-	(2): Calculo el maximo camino en esa direccion hasta la casilla anterior a F, pero teniendo en cuenta la restriccion (1) [BINGO ?]
        *		-	Si la ficha de F es de color contrario,
        *			-	Calculo la casilla que viene detras de la casilla F en esa misma direccion (sea C esa casilla)	
        *			-	Si C esta dentro del tablero y esta vacia
        *				-	Simulo comerme la pieza de F
        *				-	Simulo moverme para la casilla C
        *				-	Llamo recursivo a partir de C, teniendo en cuenta que ya comi. 
        *				-	Contruyo las soluciones teniendo en cuenta el resultado del llamado recursivo
        *				-	Restauro mi posicion
        *				-	Restauro la pieza que quite de F
        * Finalmente retorno todos los caminos que pude construir
        * */
        protected ArrayList PossibleMovements(bool eat)
        {
            int countMovement = 0;
            ArrayList listToFill = new ArrayList();

            for (int i = 0; i < IncMov.Length; i++)
            {
                bool outOfBoard = false;
                ArrayList path = MovementInDirection(IncMov[i], ref outOfBoard);

                //Restriccion (1) : Camino limpio de fichas
                if (!eat && path != null && path.Count > 0)
                    this.Restriction1(listToFill, path, new BoardPosition(this.X, this.Y), ref countMovement);
                if (outOfBoard) continue;

                //Pare la busqueda en esa direccion porque encontre otra ficha
                if (!outOfBoard && path != null)
                {
                    BoardPosition f = (path.Count == 0)
                        ? (new BoardPosition(this.X + IncMov[i].X, this.Y + IncMov[i].Y))
                        : (new BoardPosition(((BoardPosition)path[path.Count - 1]).X + IncMov[i].X, ((BoardPosition)path[path.Count - 1]).Y + IncMov[i].Y));

                    if (!this._ParentBoard.IsInsideBoard(f.X, f.Y)) continue;
                    CheckersPiece piece = this._ParentBoard.GetPieceAt(f.X, f.Y) as CheckersPiece;

                    //La pieza es de color contrario, intentare comer
                    if (piece.Color != this.Color)
                    {
                        BoardPosition c = new BoardPosition(f.X + IncMov[i].X, f.Y + IncMov[i].Y);

                        if ((this._ParentBoard.IsInsideBoard(c.X, c.Y) && this._ParentBoard.IsEmptyCell(c.X, c.Y)))
                        {
                            // Me puedo comer a 'piece'
                            this._ParentBoard.RemovePiece(f);
                            BoardPosition myPosition = new BoardPosition(this.X, this.Y);
                            this._ParentBoard.MovePieceTo(this, c.X, c.Y);

                            //Llamo recursivo y construyo mi solucion
                            ArrayList tmp = this.PossibleMovements(true);
                            this.ConfigurePath(tmp, myPosition);
                            foreach (ArrayList al in tmp) listToFill.Add(al);

                            //Restauro mi posicion y la de 'piece' BACKTRACK!!!
                            this._ParentBoard.MovePieceTo(this, myPosition.X, myPosition.Y);
                            this._ParentBoard.PutPieceAt(f.X, f.Y, piece);

                            countMovement = (tmp.Count > 0) ? countMovement + 1 : countMovement;
                            continue;
                        }
                    }//pieza de color contrario
                }//Se encontro otra ficha			
            }//for

            if (countMovement == 0)
                listToFill.Add(new ArrayList(new BoardPosition[] { new BoardPosition(this.X, this.Y) }));

            return listToFill;
        }


        protected abstract System.Collections.ArrayList MovementInDirection(System.Drawing.Point increment, ref bool outOfBoard);

        protected virtual void ConfigurePath(ArrayList listToFill, BoardPosition p)
        {
            if (listToFill != null)
            {
                for (int i = 0; i < listToFill.Count; i++)
                {
                    ArrayList al = listToFill[i] as ArrayList;
                    if (al != null)
                        al.Insert(0, p);
                }
            }
        }


        private void Restriction1(ArrayList listToFill, ArrayList path, BoardPosition p, ref int countMovement)
        {
            ArrayList list = null;
            for (int i = 0; i < path.Count; i++)
            {
                list = new ArrayList();
                list.Add(p);
                list.Add(path[i]);
                listToFill.Add(list);
            }
            //			path.Insert(0,p);
            //			lisToFill.Add(path);
            countMovement++;
        }


        #endregion PossiblePaths

    }
}
