using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleFigures {
  public class Player {
    protected string name;
    public string Name {
      get { return this.name; }
      set {
        this.name = value;
        this.CalcNamePosition();
        this.CalcMovementPointsPosition(this.movementPoints);
      }
    }

    public int Number;
    public List<Figure> Figures;

    protected int movementPoints;
    public int MovementPoints {
      get { return this.movementPoints; }
      set {
        this.movementPoints = value;
        this.CalcMovementPointsPosition(this.movementPoints);
      }
    }
    protected int tempMovementPoints;
    public int TempMovementPoints {
      get { return this.tempMovementPoints; }
      set {
        this.tempMovementPoints = value;
        this.CalcMovementPointsPosition(this.tempMovementPoints);
      }
    }

    protected int coins;
    public int Coins {
      get { return this.coins; }
      set {
        this.coins = value;
        this.CalcCoinsPosition(coins);
      }
    }
    protected int tempCoins;
    public int TempCoins {
      get { return this.tempCoins; }
      set {
        this.tempCoins = value;
        this.CalcCoinsPosition(tempCoins);
      }
    }
    public ColorTheme ColorTheme;
    public FigureBase BaseFigure;
    public bool Lose { get { return BaseFigure == null; } }
    protected Vector2 CoinsPosition;
    protected Vector2 NamePosition;
    protected Vector2 MovementPointsPosition;
    public List<GUIFigureButton> figureButtons;

    public Player(string name, int number, ColorTheme clrTheme) {
      this.Number = number;
      this.Name = name;
      this.ColorTheme = clrTheme;
      this.MovementPoints = GameLogic.MovementPoints;
      this.Coins = 7;
      this.Figures = new List<Figure>();
      this.BaseFigure = new FigureBase(this);
      this.Figures.Add(this.BaseFigure);
      this.Figures.Add(new FigureAttack(this));
      this.Figures.Add(new FigureDefence(this));
      this.Figures.Add(new FigureGold(this));
      this.figureButtons = new List<GUIFigureButton>();
    }

    protected void CalcNamePosition() {
      Vector2 nameSize = BFContent.MeasureString(this.name);
      if (this.Number == 1) {
        this.NamePosition = new Vector2(GameLogic.Field.DrawRectangle.X, GameLogic.Field.DrawRectangle.Y - nameSize.Y);
      } else if (this.Number == 2) {
        this.NamePosition = new Vector2(GameLogic.Field.DrawRectangle.Right - nameSize.X, GameLogic.Field.DrawRectangle.Y - nameSize.Y);
      } else if (this.Number == 3) {
        this.NamePosition = new Vector2(GameLogic.Field.DrawRectangle.Right - nameSize.X, GameLogic.Field.DrawRectangle.Bottom + 2);
      } else if (this.Number == 4) {
        this.NamePosition = new Vector2(GameLogic.Field.DrawRectangle.X, GameLogic.Field.DrawRectangle.Bottom + 2);
      }
      this.NamePosition.X = (int)this.NamePosition.X;
      this.NamePosition.Y = (int)this.NamePosition.Y;
    }
    protected void CalcMovementPointsPosition(int movementPoints) {
      Vector2 nameSize = BFContent.MeasureString(this.name);
      Vector2 mvsize = BFContent.MovementPointsSize(movementPoints, GameLogic.MovementPoints);
      if (this.Number == 1 || this.Number == 4) {
        this.MovementPointsPosition = new Vector2(this.NamePosition.X + nameSize.X + 10, this.NamePosition.Y + 8);
      } else if (this.Number == 2 || this.Number == 3) {
        this.MovementPointsPosition = new Vector2(this.NamePosition.X - mvsize.X - 10, this.NamePosition.Y + 8);
      }
      this.MovementPointsPosition.X = (int)this.MovementPointsPosition.X;
      this.MovementPointsPosition.Y = (int)this.MovementPointsPosition.Y;
    }

    public void CalcCoinsPosition(int coins) {
      Vector2 csize = BFContent.CoinsSize(coins);
      if (this.Number == 1) {
        this.CoinsPosition = new Vector2(GameLogic.Field.DrawRectangle.X - csize.X - 10,
            GameLogic.Field.DrawRectangle.Y - csize.Y - 4);
      } else if (this.Number == 2) {
        this.CoinsPosition = new Vector2(GameLogic.Field.DrawRectangle.Right + 10,
            GameLogic.Field.DrawRectangle.Y - csize.Y - 4);
      } else if (this.Number == 3) {
        this.CoinsPosition = new Vector2(GameLogic.Field.DrawRectangle.Right + 10,
            GameLogic.Field.DrawRectangle.Bottom + 8);
      } else {
        this.CoinsPosition = new Vector2(GameLogic.Field.DrawRectangle.X - csize.X - 10,
            GameLogic.Field.DrawRectangle.Bottom + 8);
      }
    }

    public void Draw(SpriteBatch spriteBatch) {
      this.DrawCoins(spriteBatch);
      this.DrawMovementPoints(spriteBatch);
    }

    public void DrawCoins(SpriteBatch spriteBatch) {
      if (GameLogic.calculatingCoins && GameLogic.ActivePlayer == this) {
        if (this.tempCoins > -1)
          BFContent.DrawCoins(this.tempCoins, this.CoinsPosition, spriteBatch, Color.Green, Color.Green);
        else
          BFContent.DrawCoins(this.tempCoins, this.CoinsPosition, spriteBatch, Color.Red, Color.Red);
      } else BFContent.DrawCoins(this.coins, this.CoinsPosition, spriteBatch, Color.SaddleBrown, Color.Goldenrod);
    }

    public void DrawMovementPoints(SpriteBatch spriteBatch) {
      if (GameLogic.calculatingPath && GameLogic.ActivePlayer == this) {
        if (this.tempMovementPoints > -1)
          BFContent.DrawMovementPoints(this.tempMovementPoints, GameLogic.MovementPoints, this.MovementPointsPosition,
              spriteBatch, Color.Green, Color.Green, Color.Green);
        else BFContent.DrawMovementPoints(this.tempMovementPoints, GameLogic.MovementPoints, this.MovementPointsPosition,
            spriteBatch, Color.Red, Color.Red, Color.Red);
      } else BFContent.DrawMovementPoints(this.movementPoints, GameLogic.MovementPoints, this.MovementPointsPosition,
            spriteBatch, Color.Black, Color.Black, Color.Black);
    }
  }
}
