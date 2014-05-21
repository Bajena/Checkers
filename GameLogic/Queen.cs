using System;
using System.Collections;
using System.Drawing;

namespace GameLogic
{
	public class Queen: CheckersPiece 
	{
		static Point[] Inc =
		{
		    new Point(-1,-1),
		    new Point(-1, 1),
		    new Point( 1,-1),
		    new Point( 1, 1)
		};

		ArrayList aux;
		int orgX, orgY;
        public static bool AmericanRules = false;
	    
		protected void EatMoves(int x, int y, ArrayList moves)
		{
			bool foodFound=false;

			aux.Add(new BoardPosition(x, y));

			for(int i=0; i<4; i++)
			{
			    //Initializing...
			    int newX=x+Inc[i].X;
			    int newY=y+Inc[i].Y;

			    //Looking for piece...
			    while(this.ParentBoard.IsInsideBoard(newX, newY) &&
			          this.ParentBoard.GetPieceAt(newX, newY)==null)
			    {
			        newX+=Inc[i].X;
			        newY+=Inc[i].Y;
			    }
			   

                if (this.ParentBoard.IsInsideBoard(newX, newY) )
                {
                    CheckersPiece piece = this.ParentBoard.GetPieceAt(newX, newY) as CheckersPiece;
                    if (piece.Color != this.Color)
                    {
                        //piece found...
                        int newX2 = newX + Inc[i].X;
                        int newY2 = newY + Inc[i].Y; 
                        int dx = Math.Abs(newX2 - x);
			            int dy = Math.Abs(newY2 - y);
                        while (this.ParentBoard.IsInsideBoard(newX2, newY2) &&
                               this.ParentBoard.GetPieceAt(newX2, newY2) == null && ((AmericanRules && (dx + dy == 4)) || (!AmericanRules)))
                        {
                            foodFound = true;

                            this.ParentBoard.RemovePiece(this);
                            this.ParentBoard.PutPieceAt(newX2, newY2, this);
                            this.ParentBoard.RemovePiece(piece);

                            EatMoves(newX2, newY2, moves);

                            this.ParentBoard.PutPieceAt(newX, newY, piece);
                            this.ParentBoard.RemovePiece(this);
                            this.ParentBoard.PutPieceAt(x, y, this);

                            newX2 += Inc[i].X;
                            newY2 += Inc[i].Y;
                        }
                    }
                }
			}

			if(!foodFound && (orgX!=x || orgY!=y))
			{
				BoardPosition[] arr=new BoardPosition[aux.Count];
				for(int z=0; z<aux.Count; z++)
					arr[z]=aux[z] as BoardPosition;

				moves.Add(new CheckersMove(arr, true));
			}

			aux.RemoveAt(aux.Count-1);
		}

		protected void EatMoves(ArrayList moves)
		{
			orgX=this.X;
			orgY=this.Y;
			aux=new ArrayList();
			EatMoves(this.X, this.Y, moves);
		}

		protected void SingleMoves(ArrayList moves)
		{
			for(int i=0; i<4; i++)
			{
				int newX=this.X+Inc[i].X;
				int newY=this.Y+Inc[i].Y;
				while(this.ParentBoard.IsInsideBoard(newX, newY) &&
					this.ParentBoard.GetPieceAt(newX, newY)==null)
				{
					moves.Add(
						new CheckersMove(
							new BoardPosition[]{
							   new BoardPosition(this.X, this.Y),
							   new BoardPosition(newX, newY)
							},
							false
						)
					);
                    if (AmericanRules) break;
					newX+=Inc[i].X;
					newY+=Inc[i].Y;	
				}
			}
		}

		public override ArrayList PossibleMoves
		{
			get
			{
				ArrayList moves=new ArrayList();
				EatMoves(moves);
				if(moves.Count==0)
					SingleMoves(moves);
				else
				{
					int max=0;
					for(int j=0; j<moves.Count; j++)
					{
						CheckersMove move=moves[j] as CheckersMove;
						if(move.MovePath.Length>max)
							max=move.MovePath.Length;
					}
					for(int k=0; k<moves.Count; k++)
						if(((CheckersMove)moves[k]).MovePath.Length!=max)
							moves.RemoveAt(k--);
				}
				return moves;
			}
		}

		protected override Point[] IncMov 
		{
			get 
			{
				return Queen.Inc;
			}
		}

		public Queen(CheckersBoard board,int x,int y,PieceColor color) : base(board,x,y,color) 
		{
			
		}
		
		public override bool CanEat(CheckersPiece piece) 
		{
			if (piece==null) return false;
			BoardPosition[][] possiblePos = this.PossiblePaths;
			for(int i =0; i<IncMov.Length; i++)
			{
				BoardPosition p = new BoardPosition(piece.X+IncMov[i].X, piece.Y+IncMov[i].Y);
				if (this._ParentBoard.IsInsideBoard(p.X, p.Y) && this._ParentBoard.IsEmptyCell(p.X, p.Y) && CanMoveTo(p.X, p.Y, possiblePos)) return true;
			}
			return false;
		}

		public override bool CanMoveTo(int newx, int newy) 
		{
			return this.CanMoveTo(newx, newy, this.PossiblePaths);
		}
		
		public override bool CanMakeMove(CheckersMove move) 
		{
			BoardPosition[][] posiblePositions = this.PossiblePaths;
						
			foreach (BoardPosition[] path in posiblePositions)
			{
				if (path!=null && IsIncluded(move.MovePath,path)) return true;
			}			
			return false;

			/***************** El siguiente codigo no esta terminado aun ************
			 * *************** Estaria sustituyendo al codigo de arriba *************/
			//Algoritmo: Ciclo por los movimientos simulando moverse para ver si es valido
			//
			//			if ( move==null || move.MovePath==null || move.MovePath.Length==0 || 
			//				!(this.X==move.MovePath[0].X && this.Y==move.MovePath[0].Y) ) return false;
			//			int dx = 0;
			//			int dy = 0;
			//			BoardPosition p = null;
			//			BoardPosition myPos = new BoardPosition(this.X,this.Y);
			//
			//			for(int i =0; i<move.MovePath.Length; i++){
			//				p = move.MovePath[i];
			//				dx = Math.Sign(p.X-this.X);
			//				dy = Math.Sign(p.Y=this.Y);
			//				bool found = false;
			//
			//				BoardPosition f = this.FoundInDirecction(p.X, p.Y,new System.Drawing.Point(dx, dy),found);
			//				if (!found){
			//					//Pare la busqueda en esa direccion porque encontre otra ficha
			//					f = (f!=null) 
			//						? (new BoardPosition(this.X+dx, this.Y+dy))
			//						: (new BoardPosition(f.X+dx, f.Y+dy));
			//					
			//					//?????
			//					if (!this._ParentBoard.isInsideBoard(f.X,f.Y) || ) continue;
			//					
			//					CheckersPiece piece = this._ParentBoard.GetPieceAt(f.X, f.Y) as CheckersPiece;
			//						
			//					//La pieza es de color contrario, intentare comer
			//					if (piece.Color!=this.Color){
			//						BoardPosition c = new BoardPosition(f.X+IncMov[i].X, f.Y+IncMov[i].Y);
			//						
			//						if ((this._ParentBoard.isInsideBoard(c.X, c.Y) && this._ParentBoard.isEmptyCell(c.X, c.Y))){
			//							// Me puedo comer a 'piece'
			//				
			//				}
			//				
			//				BoardPosition c = new BoardPosition(f.X+IncMov[i].X, f.Y+IncMov[i].Y);
			//				
			//			}
		}
	

		protected override System.Collections.ArrayList MovementInDirection(System.Drawing.Point increment ,ref bool outOfBoard)
		{
			System.Collections.ArrayList path = new System.Collections.ArrayList();
			int top = Math.Min((Math.Sign(increment.X)<0)?this.X:(this._ParentBoard.Width-this.X) , (Math.Sign(increment.Y)<0)?this.Y:(this._ParentBoard.Height-this.Y));
			bool foundPiece = false;
			BoardPosition pos = null;
			
			for(int i=1; i<=top; i++)
			{
				pos = new BoardPosition(this.X+i*increment.X,this.Y+i*increment.Y);
				if (this._ParentBoard.IsInsideBoard(pos.X,pos.Y)) 
				{
					if(!this._ParentBoard.IsEmptyCell(pos.X,pos.Y))
					{
						foundPiece = true;
						break;
					}   
					path.Add(pos);
				}
			}
			outOfBoard = !foundPiece;
			return path;
		}

	
		/***************************************/
	
		protected static bool IsIncluded(BoardPosition[] source, BoardPosition[] dest)
		{
			if (source==null || source.Length==0 || dest==null) return false;
			int startPos = 0;
			bool found = false;
			while (!found && startPos<dest.Length)
			{
				if (BoardPosition.Equals(dest[startPos++],source[0]))
					found = true;	
			}
			//No encontre la primera coincidencia o los BoardPosition restantes son menos de los que estoy buscando
			if (!found || ((dest.Length-startPos)<(source.Length-1))) return false;

			if (source.Length==1) return true;

			int i=1; 
			while(i<source.Length && startPos<dest.Length)
				if (BoardPosition.Equals(source[i],dest[startPos++])) i++;
				
			return (i>=source.Length);
		}

		
		protected virtual bool CanMoveTo(int newx, int newy, BoardPosition[][] posiblePositions) 
		{
			if (!this._ParentBoard.IsInsideBoard(newx, newy)) return false;
			
			foreach (BoardPosition[] path in posiblePositions)
			{
				if (path!=null)
				{
					for(int i=0; i<path.Length; i++)
						if (path[i].X==newx && path[i].Y==newy) return true;
				}
			}
			return false;
		}

		
		protected virtual bool CanMoveTo(int newx, int newy, bool eat) 
		{
			for(int i=0; i<IncMov.Length; i++)
			{
				bool found = false;
				BoardPosition f = FoundInDirecction(newx, newy, IncMov[i],ref found);
				if (found) return true;
				
				f = (f!=null) 
					? (new BoardPosition(this.X+IncMov[i].X, this.Y+IncMov[i].Y))
					: (new BoardPosition(f.X+IncMov[i].X, f.Y+IncMov[i].Y));
					
				if (!this._ParentBoard.IsInsideBoard(f.X,f.Y)) continue;
				CheckersPiece piece = this._ParentBoard.GetPieceAt(f.X, f.Y) as CheckersPiece;
						
				if (piece.Color!=this.Color)
				{
					BoardPosition c = new BoardPosition(f.X+IncMov[i].X, f.Y+IncMov[i].Y);
						
					if ((this._ParentBoard.IsInsideBoard(c.X, c.Y) && this._ParentBoard.IsEmptyCell(c.X, c.Y)))
					{
						this._ParentBoard.RemovePiece(f);
						BoardPosition myPosition = new BoardPosition(this.X, this.Y);
						this._ParentBoard.MovePieceTo(this, c.X,c.Y);
								
						if (this.CanMoveTo(newx, newy ,true))return true;
										
						this._ParentBoard.MovePieceTo(this, myPosition.X, myPosition.Y);
						this._ParentBoard.PutPieceAt(f.X, f.Y,piece);
					}
				}
			}
			
			return false;
		}

		
		protected BoardPosition FoundInDirecction(int X, int Y, System.Drawing.Point increment, ref bool found)
		{
			int top = Math.Min((Math.Sign(increment.X)<0)?this.X:(this._ParentBoard.Width-this.X) , (Math.Sign(increment.Y)<0)?this.Y:(this._ParentBoard.Height-this.Y));
			BoardPosition pos = null;
			
			for(int i=1; i<=top; i++)
			{
				pos = new BoardPosition(this.X+i*increment.X,this.Y+i*increment.Y);
				if (this._ParentBoard.IsInsideBoard(pos.X,pos.Y) && this._ParentBoard.IsEmptyCell(pos.X, pos.Y))
				{
					if (pos.X==X && pos.Y==Y) 
					{
						found =true;
						return null;
					}
				}
				else 
				{
					found = false;
					return (this._ParentBoard.IsInsideBoard(pos.X,pos.Y)) ? pos : null;
				}
			}
			return null;
		}

		
		#region USED IN DEBUG MODE
		static string ShowBoardPosition(BoardPosition[][] b)
		{
			string mov = ""+b.Length+"\n";
			foreach(BoardPosition[] p in b)
				mov+= ShowPath(p) +"\n";
			return mov;
		}

		static string ShowPath(BoardPosition[] dest)
		{
			string mov = "";
			foreach (BoardPosition p in dest)
				mov += "["+p.X +","+p.Y+"],";				
			return mov;
		}
		#endregion

		public override object Clone() 
		{
			return new Queen((CheckersBoard)this.ParentBoard, this.X, this.Y, this.Color);
		}

	}
}
