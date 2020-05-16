using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using C3.XNA;
using System.Threading;

namespace BattleFigures {
  public enum CellPart { Top, Right, Bottom, Left };
  public class Cell {
    public Figure Figure;
    public Texture2D Texture;
    public Rectangle Rectangle;
    public Vector2 TopLeft;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    public Vector2 BottomRight;
    public Point Coords;
    public bool Empty {
      get { return this.Figure == null; }
    }
    protected bool IsFocused;
    public bool Focused {
      get { return this.IsFocused; }
      set {
        if (this.Figure is FigureBase) {
          GameLogic.Field[this.Figure.GetCoords(0)].IsFocused = value;
          GameLogic.Field[this.Figure.GetCoords(1)].IsFocused = value;
          GameLogic.Field[this.Figure.GetCoords(2)].IsFocused = value;
          GameLogic.Field[this.Figure.GetCoords(3)].IsFocused = value;
        } else this.IsFocused = value;
      }
    }
    protected bool IsSelected;
    public bool Selected {
      get { return this.IsSelected; }
      set {
        if (this.Figure is FigureBase) {
          GameLogic.Field[this.Figure.GetCoords(0)].IsSelected = value;
          GameLogic.Field[this.Figure.GetCoords(1)].IsSelected = value;
          GameLogic.Field[this.Figure.GetCoords(2)].IsSelected = value;
          GameLogic.Field[this.Figure.GetCoords(3)].IsSelected = value;
        } else this.IsSelected = value;
      }
    }
    public bool IsMovementRadius;
    public bool IsPlacingRadius;
    public bool IsTempPath;
    public CellPart ActiveCellPart;

    public Cell(Rectangle rect, Point coords) {
      this.Coords = coords;
      this.Rectangle = rect;
      this.TopLeft = new Vector2(this.Rectangle.X + 1, this.Rectangle.Y + 1);
      this.TopRight = new Vector2(this.Rectangle.Right - 1, this.Rectangle.Y + 1);
      this.BottomLeft = new Vector2(this.Rectangle.X + 1, this.Rectangle.Bottom - 1);
      this.BottomRight = new Vector2(this.Rectangle.Right - 1, this.Rectangle.Bottom - 1);
    }

    public void SetCellPart(int x, int y) {
      int xs = x - this.Rectangle.Center.X, ys = y - this.Rectangle.Center.Y, xn = xs - ys, yn = xs + ys;
      if (xn < 0 && yn >= 0) this.ActiveCellPart = CellPart.Bottom;
      else if (xn >= 0 && yn >= 0) this.ActiveCellPart = CellPart.Right;
      else if (xn >= 0 && yn < 0) this.ActiveCellPart = CellPart.Top;
      else this.ActiveCellPart = CellPart.Left;
    }

    public bool CloseToBasePart() {
      if (this.ActiveCellPart == CellPart.Top) return this.Coords.Y - 1 > -1 && GameLogic.Field[this.Coords.Y - 1, this.Coords.X].Figure == GameLogic.ActivePlayer.BaseFigure;
      if (this.ActiveCellPart == CellPart.Right) return this.Coords.X + 1 < GameLogic.Field.Width && GameLogic.Field[this.Coords.Y, this.Coords.X + 1].Figure == GameLogic.ActivePlayer.BaseFigure;
      if (this.ActiveCellPart == CellPart.Bottom) return this.Coords.Y + 1 < GameLogic.Field.Height && GameLogic.Field[this.Coords.Y + 1, this.Coords.X].Figure == GameLogic.ActivePlayer.BaseFigure;
      return this.Coords.X - 1 > -1 && GameLogic.Field[this.Coords.Y, this.Coords.X - 1].Figure == GameLogic.ActivePlayer.BaseFigure;
    }

    public bool CloseToSelectedPart() {
      if (this.ActiveCellPart == CellPart.Top) return GameLogic.Field.SelectedCell.Coords.X == this.Coords.X && GameLogic.Field.SelectedCell.Coords.Y == this.Coords.Y - 1;
      if (this.ActiveCellPart == CellPart.Right) return GameLogic.Field.SelectedCell.Coords.X == this.Coords.X + 1 && GameLogic.Field.SelectedCell.Coords.Y == this.Coords.Y;
      if (this.ActiveCellPart == CellPart.Bottom) return GameLogic.Field.SelectedCell.Coords.X == this.Coords.X && GameLogic.Field.SelectedCell.Coords.Y == this.Coords.Y + 1;
      return GameLogic.Field.SelectedCell.Coords.X == this.Coords.X - 1 && GameLogic.Field.SelectedCell.Coords.Y == this.Coords.Y;
    }

    public Cell Defender {
      get {
        Point coords = this.Coords;
        Point newP = Point.Zero;
        Cell defender = null;
        Point[] arr = GameLogic.figureCellBorder;
        if (this.Figure is FigureBase) {
          coords = this.Figure.GetCoords(0);
          arr = GameLogic.baseCellBorder;
        }
        foreach (Point p in arr) {
          newP = new Point(coords.X + p.X, coords.Y + p.Y);
          if (newP.X > -1 && newP.X < GameLogic.Field.Width && newP.Y > -1 && newP.Y < GameLogic.Field.Height
              && GameLogic.Field[newP].Figure is FigureDefence && GameLogic.Field[newP].Figure.Owner == this.Figure.Owner)
            if (defender == null) defender = GameLogic.Field[newP];
            else if (GameLogic.Field[newP].Figure.HP > defender.Figure.HP) defender = GameLogic.Field[newP];
        }
        return defender;
      }
    }

    public bool InMovementRadius() {
      if (this.ActiveCellPart == CellPart.Top) return this.Coords.Y - 1 > -1 && GameLogic.Field[this.Coords.Y - 1, this.Coords.X].IsMovementRadius;
      if (this.ActiveCellPart == CellPart.Right) return this.Coords.X + 1 < GameLogic.Field.Width && GameLogic.Field[this.Coords.Y, this.Coords.X + 1].IsMovementRadius;
      if (this.ActiveCellPart == CellPart.Bottom) return this.Coords.Y + 1 < GameLogic.Field.Height && GameLogic.Field[this.Coords.Y + 1, this.Coords.X].IsMovementRadius;
      return this.Coords.X - 1 > -1 && GameLogic.Field[this.Coords.Y, this.Coords.X - 1].IsMovementRadius;
    }

    public bool InPlacingRadius() {
      if (this.ActiveCellPart == CellPart.Top) return this.Coords.Y - 1 > -1 && GameLogic.Field[this.Coords.Y - 1, this.Coords.X].IsPlacingRadius;
      if (this.ActiveCellPart == CellPart.Right) return this.Coords.X + 1 < GameLogic.Field.Width && GameLogic.Field[this.Coords.Y, this.Coords.X + 1].IsPlacingRadius;
      if (this.ActiveCellPart == CellPart.Bottom) return this.Coords.Y + 1 < GameLogic.Field.Height && GameLogic.Field[this.Coords.Y + 1, this.Coords.X].IsPlacingRadius;
      return this.Coords.X - 1 > -1 && GameLogic.Field[this.Coords.Y, this.Coords.X - 1].IsPlacingRadius;
    }

    public void Draw(SpriteBatch spriteBatch) {
      if (!this.Empty) {
        if (!(this.Figure is FigureBase && this.Figure.GetCoords(0) != this.Coords)) {
          this.Figure.Draw(spriteBatch);
          if (this.Focused) this.Figure.DrawBorders(spriteBatch);
        }
        if (this.Selected || (this.Figure is FigureBase && this.Figure.Owner == GameLogic.ActivePlayer))
          this.Figure.DrawBorders(spriteBatch);
        if (this.Focused) {
          if (this.Figure.Owner == GameLogic.ActivePlayer) {
            if (GameLogic.stackingFigures) {
              this.DrawTempFigureImproved(spriteBatch);
            }
          } else if (GameLogic.atackingFigure && this == GameLogic.Field.FocusedCell) {
            if (this.ActiveCellPart == CellPart.Top) this.DrawTopBorder(spriteBatch, Color.Red, 3);
            else if (this.ActiveCellPart == CellPart.Right) this.DrawRightBorder(spriteBatch, Color.Red, 3);
            else if (this.ActiveCellPart == CellPart.Bottom) this.DrawBottomBorder(spriteBatch, Color.Red, 3);
            else this.DrawLeftBorder(spriteBatch, Color.Red, 3);
          }
        }
      } else if (this.Focused) {
        Primitives2D.FillRectangle(spriteBatch, this.Rectangle, GameLogic.Field.ColorCellFocus);
        this.DrawAllBorders(spriteBatch, GameLogic.Field.ColorCellBorderFocus, 1);
      }
      if (this.IsMovementRadius) {
        DrawMovementRadius(spriteBatch);
        if (this.IsTempPath) DrawTempPath(spriteBatch);
      }
      if (this.IsPlacingRadius && !(this.Figure is FigureBase)) {
        DrawPlacingRadius(spriteBatch);
        if (this == GameLogic.Field.FocusedCell) {
          GameLogic.tempFigure.Draw(spriteBatch);
        }
        if (this.IsTempPath && !this.Focused) DrawTempPath(spriteBatch);
      }
    }

    public void DrawMovementRadius(SpriteBatch spriteBatch) {
      Primitives2D.FillRectangle(spriteBatch, this.Rectangle, GameLogic.Field.ColorCellMovementRadius);
      if (this.Coords.Y - 1 > -1 && !GameLogic.Field[this.Coords.Y - 1, this.Coords.X].IsMovementRadius &&
          GameLogic.Field[this.Coords.Y - 1, this.Coords.X].Empty)
        this.DrawTopBorder(spriteBatch, GameLogic.Field.ColorCellBorderMVR, 1);
      if (this.Coords.X + 1 < GameLogic.Field.Width && !GameLogic.Field[this.Coords.Y, this.Coords.X + 1].IsMovementRadius &&
          GameLogic.Field[this.Coords.Y, this.Coords.X + 1].Empty)
        this.DrawRightBorder(spriteBatch, GameLogic.Field.ColorCellBorderMVR, 1);
      if (this.Coords.Y + 1 < GameLogic.Field.Height && !GameLogic.Field[this.Coords.Y + 1, this.Coords.X].IsMovementRadius &&
          GameLogic.Field[this.Coords.Y + 1, this.Coords.X].Empty)
        this.DrawBottomBorder(spriteBatch, GameLogic.Field.ColorCellBorderMVR, 1);
      if (this.Coords.X - 1 > -1 && !GameLogic.Field[this.Coords.Y, this.Coords.X - 1].IsMovementRadius &&
          GameLogic.Field[this.Coords.Y, this.Coords.X - 1].Empty)
        this.DrawLeftBorder(spriteBatch, GameLogic.Field.ColorCellBorderMVR, 1);
    }

    public void DrawPlacingRadius(SpriteBatch spriteBatch) {
      Primitives2D.FillRectangle(spriteBatch, this.Rectangle, GameLogic.Field.ColorCellPlacingRadius);
      if (this.Coords.Y - 1 > -1 && !GameLogic.Field[this.Coords.Y - 1, this.Coords.X].IsPlacingRadius &&
          GameLogic.Field[this.Coords.Y - 1, this.Coords.X].Empty)
        this.DrawTopBorder(spriteBatch, GameLogic.Field.ColorCellBorderPR, 1);
      if (this.Coords.X + 1 < GameLogic.Field.Width && !GameLogic.Field[this.Coords.Y, this.Coords.X + 1].IsPlacingRadius &&
          GameLogic.Field[this.Coords.Y, this.Coords.X + 1].Empty)
        this.DrawRightBorder(spriteBatch, GameLogic.Field.ColorCellBorderPR, 1);
      if (this.Coords.Y + 1 < GameLogic.Field.Height && !GameLogic.Field[this.Coords.Y + 1, this.Coords.X].IsPlacingRadius &&
          GameLogic.Field[this.Coords.Y + 1, this.Coords.X].Empty)
        this.DrawBottomBorder(spriteBatch, GameLogic.Field.ColorCellBorderPR, 1);
      if (this.Coords.X - 1 > -1 && !GameLogic.Field[this.Coords.Y, this.Coords.X - 1].IsPlacingRadius &&
          GameLogic.Field[this.Coords.Y, this.Coords.X - 1].Empty)
        this.DrawLeftBorder(spriteBatch, GameLogic.Field.ColorCellBorderPR, 1);
    }

    public void DrawTempFigureImproved(SpriteBatch spriteBatch) {
      Primitives2D.FillRectangle(spriteBatch, this.Rectangle, Color.White);
      GameLogic.tempFigureImproved.Draw(spriteBatch);
      if (this.ActiveCellPart == CellPart.Top) this.DrawTopBorder(spriteBatch, Color.White, 2);
      else if (this.ActiveCellPart == CellPart.Right) this.DrawRightBorder(spriteBatch, Color.White, 2);
      else if (this.ActiveCellPart == CellPart.Bottom) this.DrawBottomBorder(spriteBatch, Color.White, 2);
      else this.DrawLeftBorder(spriteBatch, Color.White, 2);
    }

    public void DrawTempPath(SpriteBatch spriteBatch) {
      Primitives2D.DrawCircle(spriteBatch, this.Rectangle.Center.X, this.Rectangle.Center.Y, this.Rectangle.Width / 6, 16, GameLogic.Field.ColorCellBorderFocus, 4);
    }

    public void DrawTopBorder(SpriteBatch spriteBatch, Color clr, int thikness) {
      Primitives2D.DrawLine(spriteBatch, this.TopLeft, this.TopRight, clr, thikness);
    }
    public void DrawRightBorder(SpriteBatch spriteBatch, Color clr, int thikness) {
      Primitives2D.DrawLine(spriteBatch, this.TopRight, this.BottomRight, clr, thikness);
    }
    public void DrawBottomBorder(SpriteBatch spriteBatch, Color clr, int thikness) {
      Primitives2D.DrawLine(spriteBatch, this.BottomRight, this.BottomLeft, clr, thikness);
    }
    public void DrawLeftBorder(SpriteBatch spriteBatch, Color clr, int thikness) {
      Primitives2D.DrawLine(spriteBatch, this.BottomLeft, this.TopLeft, clr, thikness);
    }
    public void DrawAllBorders(SpriteBatch spriteBatch, Color clr, int thikness) {
      this.DrawTopBorder(spriteBatch, clr, thikness);
      this.DrawRightBorder(spriteBatch, clr, thikness);
      this.DrawBottomBorder(spriteBatch, clr, thikness);
      this.DrawLeftBorder(spriteBatch, clr, thikness);
    }
  }

  public class Field {
    public int DrawCellSize;
    public Rectangle DrawRectangle;
    public int Width;
    public int Height;
    public Cell[][] Cells;
    public List<Figure> FiguresList = new List<Figure>();
    public Color ColorField;
    public Color ColorGrid;
    public Cell FocusedCell;
    public Cell SelectedCell;
    public Color ColorCellFocus;
    public Color ColorCellBorderFocus;
    public Color ColorCellMovementRadius;
    public Color ColorCellBorderMVR;
    public Color ColorCellPlacingRadius;
    public Color ColorCellBorderPR;

    public Field(int drawCellSize, int width, int height, Point drawTopLeft) {
      this.DrawCellSize = drawCellSize;
      this.Width = width;
      this.Height = height;
      this.DrawRectangle = new Rectangle(drawTopLeft.X, drawTopLeft.Y, this.DrawCellSize * this.Width, this.DrawCellSize * this.Height);
      this.ColorField = ColorTheme.ColorFromHex("FAFAFA");
      this.ColorGrid = Color.Black;
      this.ColorCellFocus = this.ColorCellMovementRadius = new Color(0, 128, 0, 10);
      this.ColorCellBorderFocus = this.ColorCellBorderMVR = Color.Green;
      this.ColorCellPlacingRadius = new Color(0, 0, 128, 10);
      this.ColorCellBorderPR = Color.Blue;

      this.Cells = new Cell[Height][];
      for (int i = 0; i < this.Height; i++) {
        this.Cells[i] = new Cell[Width];
        for (int j = 0; j < this.Width; j++) {
          this.Cells[i][j] = new Cell(new Rectangle(this.DrawRectangle.X + j * this.DrawCellSize - 1,
          this.DrawRectangle.Y + i * this.DrawCellSize,
          this.DrawCellSize + 1, this.DrawCellSize + 1), new Point(j, i));
        }
      }
    }

    public void AddPlayersFigures(List<Player> players) {
      for (int i = 0; i < players.Count; i++) {
        this.FiguresList.AddRange(players[i].Figures);
        for (int j = 0; j < players[i].Figures.Count; j++) {
          for (int k = 0; k < players[i].Figures[j].GetCoordsLength(); k++) {
            this.Cells[players[i].Figures[j].GetCoords(k).Y][players[i].Figures[j].GetCoords(k).X].Figure =
                players[i].Figures[j];
          }
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch) {
      this.DrawGrid(spriteBatch);
      this.DrawCells(spriteBatch);
    }

    public void DrawGrid(SpriteBatch spriteBatch) {
      Primitives2D.FillRectangle(spriteBatch, this.DrawRectangle, this.ColorField);
      for (int i = 0; i <= GameLogic.Field.Width; i++)
        Primitives2D.DrawLine(spriteBatch, this.DrawRectangle.X + i * this.DrawCellSize, this.DrawRectangle.Y,
            this.DrawRectangle.X + i * this.DrawCellSize, this.DrawRectangle.Bottom, this.ColorGrid);
      for (int i = 0; i < GameLogic.Field.Height; i++)
        Primitives2D.DrawLine(spriteBatch, this.DrawRectangle.X, this.DrawRectangle.Y + i * this.DrawCellSize,
            this.DrawRectangle.Right, this.DrawRectangle.Y + i * this.DrawCellSize, this.ColorGrid);
      Primitives2D.DrawLine(spriteBatch, this.DrawRectangle.X - 1, this.DrawRectangle.Bottom, //-1 нужно из-за бага
          this.DrawRectangle.Right, this.DrawRectangle.Bottom, this.ColorGrid);
    }

    public void DrawCells(SpriteBatch spriteBatch) {
      for (int i = 0; i < this.Height; i++) {
        for (int j = 0; j < this.Width; j++) {
          this.Cells[i][j].Draw(spriteBatch);
        }
      }
    }

    public Cell this[int index1, int index2] {
      get { return this.Cells[index1][index2]; }
      set { this.Cells[index1][index2] = value; }
    }

    public Cell this[Point coords] {
      get { return this.Cells[coords.Y][coords.X]; }
      set { this.Cells[coords.Y][coords.X] = value; }
    }

    public override string ToString() {
      string res = "";
      for (int i = 0; i < this.Cells.Length; i++) {
        for (int j = 0; j < this.Cells[i].Length; j++) {
          res += this.Cells[i][j].Empty ? "0" : "1";
        }
        res += "\n";
      }
      return res;
    }
  }

  static public class GameLogic {
    public static List<Player> Players;
    public static Player ActivePlayer;
    public static int MovementPoints;
    public static int MaxMovementPoints;
    public static Field Field;
    public static int TurnNumber;
    public static int BaseProduction;
    public enum Price { Attack = 5, Defence = 7, Gold = 10 };
    public static Figure PlacingFigure;
    public static Figure tempFigure;
    public static Figure tempFigureImproved;
    static List<Point> tempRadius = new List<Point>();
    static List<Point> tempPath = new List<Point>();
    public static bool calculatingCoins;
    public static bool calculatingPath;
    public static bool stackingFigures;
    public static bool atackingFigure;
    public static bool stackingEnabled = true;
    public static bool slidingEnabled = true;
    public static bool actionsBlocked = false;
    public static Point[] figureCellBorder = new Point[] { new Point(-1, 0), new Point(0, -1), new Point(1, 0), new Point(0, 1) };
    public static Point[] baseCellBorder = new Point[] { new Point(0, -1), new Point(1, -1), new Point(2, 0), new Point(2, 1), new Point(1, 2), new Point(0, 2), new Point(-1, 1), new Point(-1, 0) };

    public static void Init(int playersCount, int drawCellSize, int fieldWidth, int fieldHeight, int windowWidth, int windowHeight) {
      GameLogic.Field = new Field(drawCellSize, fieldWidth, fieldHeight, new Point(windowWidth / 2 - drawCellSize * fieldWidth / 2,
                                      windowHeight / 2 - drawCellSize * fieldHeight / 2));
      MovementPoints = 3;
      BaseProduction = 1;
      MaxMovementPoints = 15;
      Players = new List<Player>();
      if (playersCount == 3) {
        Players.Add(new Player(Localization.Player + "1", 1, ColorTheme.Red));
        Players.Add(new Player(Localization.Player + "2", 2, ColorTheme.Indigo));
        Players.Add(new Player(Localization.Player + "3", 3, ColorTheme.Green));
      } else if (playersCount == 4) {

        Players.Add(new Player(Localization.Player + "1", 1, ColorTheme.Red));
        Players.Add(new Player(Localization.Player + "2", 2, ColorTheme.Indigo));
        Players.Add(new Player(Localization.Player + "3", 3, ColorTheme.Green));
        Players.Add(new Player(Localization.Player + "4", 4, ColorTheme.Orange));
      } else {
        Players.Add(new Player(Localization.Player + "1", 1, ColorTheme.Red));
        Players.Add(new Player(Localization.Player + "2", 3, ColorTheme.Indigo));
      }
      ActivePlayer = Players[0];
      Field.AddPlayersFigures(Players);
      TurnNumber = 1;
    }

    static int xcellNow = 0, ycellNow = 0, xcellPrev = 0, ycellPrev = 0;
    static CellPart cellPartNow, cellPartPrev;
    public static void FieldMouseMove(object sender, MouseEventArgs args) {
      xcellNow = (args.MouseStNow.X - Field.DrawRectangle.X) / Field.DrawCellSize;
      ycellNow = (args.MouseStNow.Y - Field.DrawRectangle.Y) / Field.DrawCellSize;
      xcellPrev = (args.MouseStPrev.X - Field.DrawRectangle.X) / Field.DrawCellSize;
      ycellPrev = (args.MouseStPrev.Y - Field.DrawRectangle.Y) / Field.DrawCellSize;
      if (ycellPrev < Field.Height && xcellPrev < Field.Width && ycellPrev >= 0 && xcellPrev >= 0) {
        Field[ycellPrev, xcellPrev].Focused = false;
        cellPartPrev = Field[ycellPrev, xcellPrev].ActiveCellPart;
      }
      Field[ycellNow, xcellNow].Focused = true;
      Field.FocusedCell = Field[ycellNow, xcellNow];
      Field.FocusedCell.SetCellPart(args.MouseStNow.X, args.MouseStNow.Y);
      cellPartNow = Field.FocusedCell.ActiveCellPart;
      FieldMouseMove();
    }

    public static void FieldMouseMove() {
      if (actionsBlocked) return;
      if (Field.FocusedCell.IsMovementRadius) {
        if (xcellNow != xcellPrev || ycellNow != ycellPrev) {
          SetTempPathMove(Field.SelectedCell, Field.FocusedCell);
          SetTempMovementPoints();
          ClearStackingAtacking();
        }
      } else if (Field.FocusedCell.IsPlacingRadius) {
        if (xcellNow != xcellPrev || ycellNow != ycellPrev) {
          SetTempPathPlace(Field.FocusedCell);
          MoveTempFigures();
          SetTempMovementPoints();
          ClearStackingAtacking();
        }
      } else if (cellPartNow != cellPartPrev) {
        if (!Field.FocusedCell.Empty) {
          if (Field.FocusedCell.Figure.Owner == ActivePlayer) {
            if (stackingEnabled && Field.SelectedCell != null && Field.SelectedCell.Figure != Field.FocusedCell.Figure &&
                CanStack(Field.SelectedCell.Figure, Field.FocusedCell.Figure) &&
                (Field.FocusedCell.InMovementRadius() || Field.FocusedCell.CloseToSelectedPart())) {
              stackingFigures = true;
              SetTempPathStack();
              SetTempMovementPoints();
              CreateImprovedFigure(Field.SelectedCell.Figure);
              ImproveTempFigure(Field.FocusedCell.Figure);
              MoveTempFigures();
              return;
            } else if (stackingEnabled && PlacingFigure != null && CanStack(PlacingFigure, Field.FocusedCell.Figure)
                  && (Field.FocusedCell.InPlacingRadius() || Field.FocusedCell.CloseToBasePart())) {
              stackingFigures = true;
              SetTempPathStackPlace();
              SetTempMovementPoints();
              CreateImprovedFigure(PlacingFigure);
              ImproveTempFigure(Field.FocusedCell.Figure);
              MoveTempFigures();
              return;
            }
          } else if (GameLogic.Field.SelectedCell != null && GameLogic.Field.SelectedCell.Figure is FigureAttack &&
                    (Field.FocusedCell.InMovementRadius() || Field.FocusedCell.CloseToSelectedPart())) {
            atackingFigure = true;
            SetTempPathPart();
            SetTempMovementPoints();
            return;
          }
        }
        ClearStackingAtacking();
        ClearTempPath();
      }
    }

    public static void FieldMouseEnter(object sender, MouseEventArgs args) {
      xcellNow = (args.MouseStNow.X - Field.DrawRectangle.X) / Field.DrawCellSize;
      ycellNow = (args.MouseStNow.Y - Field.DrawRectangle.Y) / Field.DrawCellSize;
      xcellPrev = -1;
      ycellPrev = -1;
      if (ycellPrev < Field.Height && xcellPrev < Field.Width && ycellPrev >= 0 && xcellPrev >= 0)
        Field[ycellPrev, xcellPrev].Focused = false;
      Field[ycellNow, xcellNow].Focused = true;
      Field.FocusedCell = Field[ycellNow, xcellNow];
      Field.FocusedCell.SetCellPart(args.MouseStNow.X, args.MouseStNow.Y);
      FieldMouseMove();
    }
    public static void FieldMouseLeave(object sender, MouseEventArgs args) {
      xcellPrev = (args.MouseStPrev.X - Field.DrawRectangle.X) / Field.DrawCellSize;
      ycellPrev = (args.MouseStPrev.Y - Field.DrawRectangle.Y) / Field.DrawCellSize;
      Field[ycellPrev, xcellPrev].Focused = false;
      Field.FocusedCell = null;
      ClearTempPath();
    }

    public static void FieldMouseLeftClick(object sender, MouseEventArgs args) {
      if (actionsBlocked) return;
      if ((Field.FocusedCell.Empty || (Field.FocusedCell.Figure.Owner != ActivePlayer && !atackingFigure))
          && !Field.FocusedCell.IsMovementRadius && !Field.FocusedCell.IsPlacingRadius) return;
      if (Field.FocusedCell.IsMovementRadius) {
        MoveFigure(Field.SelectedCell, Field.FocusedCell);
        ClearTempPath();
      } else if (Field.FocusedCell.IsPlacingRadius) {
        PlaceFigure(PlacingFigure, Field.FocusedCell);
        ClearPlacing();
        ToogleFigureButtonsSelected(ActivePlayer, false);
        ClearTempCoins();
      } else if (stackingFigures) {
        if (ActivePlayer.MovementPoints - tempPath.Count > -1) {
          if (Field.SelectedCell != null) {
            StackFiguresMove(Field.SelectedCell, Field.FocusedCell);
            ClearTempPath();
          } else {
            StackFiguresPlace(PlacingFigure, Field.FocusedCell, FigurePrice(PlacingFigure));
            ClearPlacing();
            ClearTempCoins();
          }
          ClearStackingAtacking();
          ToogleFigureButtonsSelected(ActivePlayer, false);
        }
      } else if (atackingFigure) {
        if (ActivePlayer.MovementPoints - tempPath.Count > -1) {
          AttackFigure(Field.SelectedCell, Field.FocusedCell);
          ClearSelection();
          ClearStackingAtacking();
        }
      } else {
        if (PlacingFigure != null) {
          ClearPlacing();
          ClearTempCoins();
        }
        ToogleFigureButtonsSelected(ActivePlayer, false);
        if (Field.FocusedCell.Figure is FigureAttack && ((FigureAttack)Field.FocusedCell.Figure).Attacked) return;
        if (Field.SelectedCell != null && Field.SelectedCell.Figure == Field.FocusedCell.Figure) {
          Field.FocusedCell.Selected = !Field.FocusedCell.Selected;
          if (Field.FocusedCell.Selected && !(Field.FocusedCell.Figure is FigureBase)) {
            tempRadius = Path.FindMovementRadius(new Point(xcellNow, ycellNow), ActivePlayer.MovementPoints);
            foreach (Point p in tempRadius) Field[p].IsMovementRadius = true;
            Field.SelectedCell = Field.FocusedCell;
          } else {
            foreach (Point p in tempRadius) Field[p].IsMovementRadius = false;
            tempRadius = new List<Point>();
            Field.SelectedCell = null;
          }
        } else {
          if (Field.SelectedCell != null) Field.SelectedCell.Selected = false;
          Field.FocusedCell.Selected = true;
          Field.SelectedCell = Field.FocusedCell;
          foreach (Point p in tempRadius) Field[p].IsMovementRadius = false;
          if (!(Field.FocusedCell.Figure is FigureBase)) {
            tempRadius = Path.FindMovementRadius(new Point(xcellNow, ycellNow), ActivePlayer.MovementPoints);
            foreach (Point p in tempRadius) Field[p].IsMovementRadius = true;
          }
        }
      }
    }
    public static void WndMainMouseRightClick(object sender, MouseEventArgs args) {
      if (actionsBlocked) return;
      ClearSelection();
      ClearPlacing();
      ClearTempPath();
      ClearTempCoins();
      ClearStackingAtacking();
      ToogleFigureButtonsSelected(ActivePlayer, false);
    }

    public static void BtnFigureLeftClick(object sender, MouseEventArgs args) {
      if (actionsBlocked) return;
      GUIFigureButton btn = (GUIFigureButton)sender;
      if (btn.Price > ActivePlayer.Coins) return;
      ClearSelection();
      ClearTempCoins();
      SetTempCoins(((GUIFigureButton)sender).Price);
      ToogleFigureButtonsSelected(btn, ActivePlayer);
      if (btn.figure == PlacingFigure) {
        ClearPlacing();
      } else {
        ClearPlacing();
        PlacingFigure = btn.figure;
        tempRadius = Path.FindMovementRadius(ActivePlayer.BaseFigure.GetCoords(), ActivePlayer.MovementPoints);
        foreach (Point p in tempRadius) Field[p].IsPlacingRadius = true;
        CreateTempFigures();
      }
    }

    public static void BtnFigureEnter(object sender, MouseEventArgs args) {
      if (!calculatingCoins) SetTempCoins(((GUIFigureButton)sender).Price);
    }

    public static void BtnFigureLeave(object sender, MouseEventArgs args) {
      if (PlacingFigure == null) ClearTempCoins();
    }

    public static bool CanStack(Figure f1, Figure f2) {
      return f1.Stacks == 1 && f2.Stacks == 1 && !(f1 is FigureGold) && f1.GetType() == f2.GetType();
    }

    public static void SetTempCoins(int price) {
      calculatingCoins = true;
      ActivePlayer.TempCoins = ActivePlayer.Coins - price;
    }

    public static void ClearTempCoins() {
      calculatingCoins = false;
      ActivePlayer.TempCoins = ActivePlayer.Coins;
    }

    public static void ClearStackingAtacking() {
      stackingFigures = false;
      atackingFigure = false;
    }

    public static void SetTempPathStackPlace() {
      if (Field.FocusedCell.ActiveCellPart == CellPart.Top) SetTempPathMove(ActivePlayer.BaseFigure, Field[Field.FocusedCell.Coords.Y - 1, Field.FocusedCell.Coords.X]);
      else if (Field.FocusedCell.ActiveCellPart == CellPart.Right) SetTempPathMove(ActivePlayer.BaseFigure, Field[Field.FocusedCell.Coords.Y, Field.FocusedCell.Coords.X + 1]);
      else if (Field.FocusedCell.ActiveCellPart == CellPart.Bottom) SetTempPathMove(ActivePlayer.BaseFigure, Field[Field.FocusedCell.Coords.Y + 1, Field.FocusedCell.Coords.X]);
      else SetTempPathMove(ActivePlayer.BaseFigure, Field[Field.FocusedCell.Coords.Y, Field.FocusedCell.Coords.X - 1]);
      tempPath.Insert(0, Field.FocusedCell.Coords);
    }

    public static void SetTempPathStack() {
      SetTempPathPart();
      tempPath.Insert(0, Field.FocusedCell.Coords);
    }

    public static void SetTempPathPart() {
      if (Field.FocusedCell.ActiveCellPart == CellPart.Top) SetTempPathMove(Field.SelectedCell, Field[Field.FocusedCell.Coords.Y - 1, Field.FocusedCell.Coords.X]);
      else if (Field.FocusedCell.ActiveCellPart == CellPart.Right) SetTempPathMove(Field.SelectedCell, Field[Field.FocusedCell.Coords.Y, Field.FocusedCell.Coords.X + 1]);
      else if (Field.FocusedCell.ActiveCellPart == CellPart.Bottom) SetTempPathMove(Field.SelectedCell, Field[Field.FocusedCell.Coords.Y + 1, Field.FocusedCell.Coords.X]);
      else SetTempPathMove(Field.SelectedCell, Field[Field.FocusedCell.Coords.Y, Field.FocusedCell.Coords.X - 1]);
    }

    public static void SetTempPathMove(Cell from, Cell to) {
      foreach (Point p in tempPath) Field[p].IsTempPath = false;
      tempRadius = Path.FindMovementRadius(from.Coords, ActivePlayer.MovementPoints);
      if (from.Coords == to.Coords) tempPath = new List<Point>();
      else {
        tempPath = Path.FindPath(from.Coords, to.Coords);
        foreach (Point p in tempPath) Field[p].IsTempPath = true;
      }
    }

    public static void SetTempPathMove(FigureBase fb, Cell to) {
      foreach (Point p in tempPath) Field[p].IsTempPath = false;
      tempRadius = Path.FindMovementRadius(fb.GetCoords(), ActivePlayer.MovementPoints);
      if (fb.GetCoords().Contains(to.Coords)) tempPath = new List<Point>();
      else {
        tempPath = Path.FindPath(fb.GetCoords(), to.Coords);
        foreach (Point p in tempPath) Field[p].IsTempPath = true;
      }
    }

    public static void SetTempPathPlace(Cell to) {
      foreach (Point p in tempPath) Field[p].IsTempPath = false;
      tempRadius = Path.FindMovementRadius(ActivePlayer.BaseFigure.GetCoords(), ActivePlayer.MovementPoints);
      tempPath = Path.FindPath(ActivePlayer.BaseFigure.GetCoords(), to.Coords);
      foreach (Point p in tempPath) Field[p].IsTempPath = true;
    }

    public static void SetTempMovementPoints() {
      calculatingPath = true;
      ActivePlayer.TempMovementPoints = ActivePlayer.MovementPoints - tempPath.Count;
    }

    public static void CreateTempFigures() {
      if (PlacingFigure is FigureAttack) tempFigure = new FigureAttack(ActivePlayer);
      else if (PlacingFigure is FigureDefence) tempFigure = new FigureDefence(ActivePlayer);
      else if (PlacingFigure is FigureGold) tempFigure = new FigureGold(ActivePlayer);
      tempFigure.Color = ActivePlayer.ColorTheme.ClrFigureDefence;
      tempFigure.Color.A = 70;
      CreateImprovedFigure(PlacingFigure);
    }

    public static void CreateImprovedFigure(Figure figure) {
      if (figure is FigureAttack) {
        tempFigureImproved = new FigureAttack(ActivePlayer);
        ((FigureAttack)tempFigureImproved).Damage = ((FigureAttack)figure).Damage;
      } else if (figure is FigureDefence) tempFigureImproved = new FigureDefence(ActivePlayer);
      else if (figure is FigureGold) {
        tempFigureImproved = new FigureGold(ActivePlayer);
        ((FigureGold)tempFigureImproved).Production = ((FigureGold)figure).Production;
        ((FigureGold)tempFigureImproved).MinProduction = ((FigureGold)figure).MinProduction;
        ((FigureGold)tempFigureImproved).MaxProduction = ((FigureGold)figure).MaxProduction;
        ((FigureGold)tempFigureImproved).ProductionCoef = ((FigureGold)figure).ProductionCoef;
      }
      tempFigureImproved.MaxHP = figure.MaxHP; //сначала maxhp
      tempFigureImproved.HP = figure.HP;
      tempFigureImproved.Color = ActivePlayer.ColorTheme.ClrFigureDefence;
      tempFigureImproved.Color.A = 100;
    }

    public static void ResetImprovedFigure() {
      tempFigureImproved.MaxHP = tempFigure.HP;
      tempFigureImproved.HP = tempFigure.HP;
      if (tempFigure is FigureAttack) ((FigureAttack)tempFigureImproved).Damage = ((FigureAttack)tempFigure).Damage;
      else if (tempFigure is FigureGold) {
        ((FigureGold)tempFigureImproved).Production = ((FigureGold)tempFigure).Production;
        ((FigureGold)tempFigureImproved).MinProduction = ((FigureGold)tempFigure).MinProduction;
        ((FigureGold)tempFigureImproved).MaxProduction = ((FigureGold)tempFigure).MaxProduction;
        ((FigureGold)tempFigureImproved).ProductionCoef = ((FigureGold)tempFigure).ProductionCoef;
      };
    }

    public static void ImproveTempFigure(Figure figure) {
      tempFigureImproved.MaxHP += figure.HP;
      tempFigureImproved.HP += figure.HP;
      if (figure is FigureAttack) ((FigureAttack)tempFigureImproved).Damage += ((FigureAttack)figure).Damage;
      else if (figure is FigureGold) {
        ((FigureGold)tempFigureImproved).Production += ((FigureGold)figure).Production;
        ((FigureGold)tempFigureImproved).MinProduction += ((FigureGold)figure).MinProduction;
        ((FigureGold)tempFigureImproved).MaxProduction += ((FigureGold)figure).MaxProduction;
        ((FigureGold)tempFigureImproved).ProductionCoef += ((FigureGold)figure).ProductionCoef;
      };
    }

    public static void MoveTempFigures() {
      tempFigure.SetCoords(Field.FocusedCell.Coords);
      tempFigureImproved.SetCoords(Field.FocusedCell.Coords);
      //if (!Field.FocusedCell.Empty && tempFigureImproved.GetType() == Field.FocusedCell.Figure.GetType())
      //{
      //    ResetImprovedFigure();
      //    ImproveTempFigure(Field.FocusedCell.Figure);
      //}
    }

    public static void ClearSelection() {
      foreach (Point p in tempRadius) Field[p].IsMovementRadius = false;
      if (Field.SelectedCell != null) Field.SelectedCell.Selected = false;
      Field.SelectedCell = null;
      ClearTempPath();
    }

    public static void ClearPlacing() {
      foreach (Point p in tempRadius) Field[p].IsPlacingRadius = false;
      PlacingFigure = null;
      ClearTempPath();
    }

    public static void ClearTempPath() {
      foreach (Point p in tempPath) Field[p].IsTempPath = false;
      calculatingPath = false;
      ActivePlayer.TempMovementPoints = ActivePlayer.MovementPoints;
    }

    public static void MoveFigure(Cell from, Cell to) {
      foreach (Point p in tempRadius) Field[p].IsMovementRadius = false;
      if (Field.SelectedCell != null) Field.SelectedCell.Selected = false;

      to.Figure = from.Figure;
      from.Figure = null;
      if (slidingEnabled) SlideFigure(to.Figure, tempPath);
      else to.Figure.SetCoords(to.Coords);
      ActivePlayer.MovementPoints -= tempPath.Count;
      if (to.Figure is FigureGold) ((FigureGold)to.Figure).Moved = true;

      Field.SelectedCell = null;
    }

    public static float slideSpeedCoef = 0.368f;
    public static float minSlideSpeed = 0.5f;
    public static float maxSlideSpeed = 2.5f;
    public static void SlideFigure(Figure figure, List<Point> path) {
      float speed = path.Count * Field.DrawCellSize * slideSpeedCoef / 100f;
      if (speed < minSlideSpeed) speed = minSlideSpeed;
      else if (speed > maxSlideSpeed) speed = maxSlideSpeed;
      for (int i = path.Count - 1; i > -1; i--) {
        float x = figure.DrawRectangle.X, y = figure.DrawRectangle.Y,
            dx = Field[path[i]].Rectangle.X - x + 1,
            dy = Field[path[i]].Rectangle.Y - y + 1;
        int count = Math.Abs(dy == 0 ? (int)(dx / speed) : (int)(dy / speed));
        int mulX = dx < 0 ? -1 : dx == 0 ? 0 : 1, mulY = dy < 0 ? -1 : dy == 0 ? 0 : 1;
        for (int j = 0; j < count; j++) {
          x += speed * mulX; y += speed * mulY;
          figure.DrawRectangle = new Rectangle((int)x, (int)y, figure.DrawRectangle.Width, figure.DrawRectangle.Height);
          Thread.Sleep(1);
        }
        figure.SetCoords(path[i]);
      }
    }

    public static void PlaceFigure(Figure figure, Cell to) {
      if (figure is FigureAttack) to.Figure = new FigureAttack(ActivePlayer, to.Coords);
      if (figure is FigureDefence) to.Figure = new FigureDefence(ActivePlayer, to.Coords);
      if (figure is FigureGold) to.Figure = new FigureGold(ActivePlayer, to.Coords);
      ActivePlayer.Figures.Add(to.Figure);
      ActivePlayer.MovementPoints -= tempPath.Count;
      ActivePlayer.Coins -= FigurePrice(figure);
    }

    public static void StackFiguresMove(Cell from, Cell to) {
      foreach (Point p in tempRadius) Field[p].IsMovementRadius = false;
      if (Field.SelectedCell != null) Field.SelectedCell.Selected = false;

      if (slidingEnabled) {
        ClearStackingAtacking();
        SlideFigure(from.Figure, tempPath);
      }

      to.Figure.MaxHP += from.Figure.HP;
      to.Figure.HP += from.Figure.HP;
      to.Figure.Stacks++;
      if (from.Figure is FigureAttack) ((FigureAttack)to.Figure).Damage += ((FigureAttack)from.Figure).Damage;
      if (from.Figure is FigureGold) {
        ((FigureGold)from.Figure).Production = ((FigureGold)from.Figure).MinProduction;
        ((FigureGold)to.Figure).Production += ((FigureGold)from.Figure).Production;
        ((FigureGold)to.Figure).MinProduction += ((FigureGold)from.Figure).MinProduction;
        ((FigureGold)to.Figure).MaxProduction += ((FigureGold)from.Figure).MaxProduction;
        ((FigureGold)to.Figure).ProductionCoef += ((FigureGold)from.Figure).ProductionCoef;
      };
      ActivePlayer.MovementPoints -= tempPath.Count;
      ActivePlayer.Figures.Remove(from.Figure);
      from.Figure = null;

      Field.SelectedCell = null;
    }

    public static void StackFiguresPlace(Figure from, Cell to, int price) {
      to.Figure.MaxHP += from.HP;
      to.Figure.HP += from.HP;
      to.Figure.Stacks++;
      if (from is FigureAttack) ((FigureAttack)to.Figure).Damage += ((FigureAttack)from).Damage;
      if (from is FigureGold) {
        ((FigureGold)from).Production = ((FigureGold)from).MinProduction;
        ((FigureGold)to.Figure).Production += ((FigureGold)from).Production;
        ((FigureGold)to.Figure).MinProduction += ((FigureGold)from).MinProduction;
        ((FigureGold)to.Figure).MaxProduction += ((FigureGold)from).MaxProduction;
        ((FigureGold)to.Figure).ProductionCoef += ((FigureGold)from).ProductionCoef;
      };
      ActivePlayer.MovementPoints -= tempPath.Count;
      ActivePlayer.Figures.Remove(from);
      ActivePlayer.Coins -= price;
      from = null;
    }

    public static void AttackFigure(Cell from, Cell to) {
      FigureAttack attacker = ((FigureAttack)from.Figure);
      if (tempPath.Count != 0) MoveFigure(from, Field[tempPath.First()]);

      Cell defender = to.Defender, targetCell = null;
      int damageLast = attacker.Damage;
      do {
        if (defender != null) targetCell = defender;
        else targetCell = to;
        targetCell.Figure.HP -= damageLast;
        if (targetCell.Figure.HP < 0) damageLast = Math.Abs(targetCell.Figure.HP);
        else damageLast = 0;
        attacker.Attacked = true;
        if (targetCell.Figure.HP < 1) {
          Player targetPlayer = targetCell.Figure.Owner;
          targetPlayer.Figures.Remove(targetCell.Figure);
          if (targetCell.Figure is FigureBase) {
            targetCell.Selected = to.Focused = false;
            foreach (Point p in targetPlayer.BaseFigure.GetCoords()) {
              Field[p].Figure = null;
            }
            targetPlayer.BaseFigure = null;
            OwnPlayerFigures(attacker.Owner, targetPlayer);
          } else {
            targetCell.Figure = null;
          }
        }
        defender = to.Defender;
      } while (damageLast > 0 && to.Figure != null);
    }

    public static void OwnPlayerFigures(Player owner, Player loser) {
      foreach (Figure f in loser.Figures) {
        f.Owner = owner;
        owner.Figures.Add(f);
      }
      loser.Figures.Clear();
    }

    public static int FigurePrice(Figure f) {
      if (f is FigureAttack) return (int)Price.Attack;
      if (f is FigureDefence) return (int)Price.Defence;
      else return (int)Price.Gold;
    }

    public static void EndTurnClick(object sender, MouseEventArgs args) {
      if (actionsBlocked) return;
      ToogleFigureButtonsEnabled(ActivePlayer, false);
      ToogleFigureButtonsSelected(ActivePlayer, false);
      ClearSelection();
      ClearPlacing();
      ClearTempCoins();
      EndTurn();
      ToogleFigureButtonsEnabled(ActivePlayer, true);
    }

    public static void ToogleFigureButtonsEnabled(Player p, bool enabled) {
      foreach (GUIFigureButton btn in p.figureButtons)
        btn.Enabled = enabled;
    }

    public static void ToogleFigureButtonsSelected(GUIFigureButton fb, Player p) {
      fb.Selected = !fb.Selected;
      foreach (GUIFigureButton btn in p.figureButtons)
        if (btn != fb) btn.Selected = false;
    }

    public static void ToogleFigureButtonsSelected(Player p, bool selected) {
      foreach (GUIFigureButton btn in p.figureButtons)
        btn.Selected = selected;
    }

    public static void EndTurn() {
      do {
        ActivePlayer = Players[(Players.IndexOf(ActivePlayer) + 1) % Players.Count];
      }
      while (ActivePlayer.Lose);
      if (ActivePlayer.Number == 1) {
        /*if (MovementPoints < MaxMovementPoints)*/
        MovementPoints++;
        TurnNumber++;
        for (int i = 0; i < Players.Count; i++) {
          Players[i].MovementPoints = MovementPoints;
          for (int j = 0; j < Players[i].Figures.Count; j++) {
            if (Players[i].Figures[j] is FigureBase) Players[i].Coins += BaseProduction;
            else if (Players[i].Figures[j] is FigureGold) {
              FigureGold fg = (FigureGold)Players[i].Figures[j];
              if (fg.Moved) {
                fg.Production = fg.MinProduction;
                fg.Moved = false;
              } else {
                fg.Production += fg.ProductionCoef;
                if (fg.Production > fg.MaxProduction) fg.Production = fg.MaxProduction;
              }
              Players[i].Coins += fg.Production;
            } else if (Players[i].Figures[j] is FigureAttack) ((FigureAttack)Players[i].Figures[j]).Attacked = false;
          }
        }
      }
    }
  }
}
