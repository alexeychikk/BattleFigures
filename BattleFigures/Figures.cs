using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using C3.XNA;

namespace BattleFigures {
  public struct ColorTheme {
    public Color ClrMain;
    public Color ClrFigureBase;
    public Color ClrFigureAttack;
    public Color ClrFigureDefence;
    public Color ClrFigureGold;
    public Color ClrBorder;
    public Color ClrHpLeft;

    public ColorTheme(Color main, Color figureBase, Color figureAttack, Color figureDefence, Color figureGold) {
      this.ClrMain = main;
      this.ClrFigureBase = figureBase;
      this.ClrFigureAttack = figureAttack;
      this.ClrFigureDefence = figureDefence;
      this.ClrFigureGold = figureGold;
      this.ClrBorder = new Color((this.ClrFigureAttack.R + 85) % 255, (this.ClrFigureAttack.G + 85) % 255,
          (this.ClrFigureAttack.B + 85) % 255);
      this.ClrHpLeft = new Color(this.ClrFigureGold.R + (int)(this.ClrFigureGold.R * 0.3),
          this.ClrFigureGold.G + (int)(this.ClrFigureGold.G * 0.3), this.ClrFigureGold.B + (int)(this.ClrFigureGold.B * 0.3));
    }

    public static ColorTheme Green {
      get {
        return new ColorTheme(ColorTheme.ColorFromHex("4CAF50"), ColorTheme.ColorFromHex("4CAF50"),
            ColorTheme.ColorFromHex("A5D6A7"), ColorTheme.ColorFromHex("2E7D32"), ColorTheme.ColorFromHex("B9F6CA"));
      }
    }

    public static ColorTheme Indigo {
      get {
        return new ColorTheme(ColorTheme.ColorFromHex("5C6BC0"), ColorTheme.ColorFromHex("5C6BC0"),
            ColorTheme.ColorFromHex("9FA8DA"), ColorTheme.ColorFromHex("3F51B5"), ColorTheme.ColorFromHex("8C9EFF"));
      }
    }

    public static ColorTheme Orange {
      get {
        return new ColorTheme(ColorTheme.ColorFromHex("FF9800"), ColorTheme.ColorFromHex("FF9800"),
            ColorTheme.ColorFromHex("FFB74D"), ColorTheme.ColorFromHex("EF6C00"), ColorTheme.ColorFromHex("FFD180"));
      }
    }

    public static ColorTheme Red {
      get {
        return new ColorTheme(ColorTheme.ColorFromHex("F44336"), ColorTheme.ColorFromHex("F44336"),
            ColorTheme.ColorFromHex("EF9A9A"), ColorTheme.ColorFromHex("C62828"), ColorTheme.ColorFromHex("FF8A80"));
      }
    }

    public static ColorTheme Teal {
      get {
        return new ColorTheme(ColorTheme.ColorFromHex("009688"), ColorTheme.ColorFromHex("009688"),
            ColorTheme.ColorFromHex("80CBC4"), ColorTheme.ColorFromHex("00695C"), ColorTheme.ColorFromHex("A7FFEB"));
      }
    }

    public static Color ColorFromHex(string hex) {
      int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
      int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
      int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
      return new Color(r, g, b);
    }

    public static Color ColorFromHex(string hex, byte alpha) {
      Color clr = ColorTheme.ColorFromHex(hex);
      clr.A = alpha;
      return clr;
    }
  }

  abstract public class Figure {
    protected Player owner;
    public Player Owner {
      get { return this.owner; }
      set {
        this.owner = value;
        SetFigureColor(this.owner.ColorTheme);
      }
    }

    protected int maxHP;
    public int MaxHP {
      get { return this.maxHP; }
      set {
        this.maxHP = value;
        this.CalcFillRect();
      }
    }
    protected int hp;
    public int HP {
      get { return this.hp; }
      set {
        this.hp = value;
        this.CalcHPPosition();
        this.CalcFillRect();
      }
    }
    public int Stacks = 1;

    protected Vector2 hpPosition;
    protected Vector2 heartPosition;

    protected Texture2D texture;
    public Color Color;
    public Color TextureColor;

    protected Point[] coords;
    protected Rectangle drawRectangle;
    public virtual Rectangle DrawRectangle {
      get { return this.drawRectangle; }
      set {
        this.drawRectangle = value;
        this.CalcFigureContent();
        this.CalcBordersRect();
        this.CalcFillRect();
      }
    }
    protected Rectangle fillRectangle;
    protected Rectangle bordersRectangle;
    protected Vector2 textureCoords;

    public Figure(Player player, Texture2D texture, Point[] coords, int maxHP, int hp) {
      this.Owner = player;
      this.maxHP = maxHP;
      this.HP = hp;
      this.texture = texture;
      this.SetCoords(coords);
    }

    public void SetCoords(int x, int y) {
      this.coords[0].X = x;
      this.coords[0].Y = y;
      this.CalcDrawParams();
    }

    public void SetCoords(Point[] coords) {
      this.coords = coords;
      this.CalcDrawParams();
    }

    public void SetCoords(Point coords0) {
      this.coords[0] = coords0;
      this.CalcDrawParams();
    }

    protected virtual void CalcDrawParams() {
      this.CalcDrawRect();
      this.CalcFillRect();
      this.CalcFigureContent();
      this.CalcBordersRect();
    }
    protected virtual void CalcFigureContent() {
      this.CalcTextureCoords();
      this.CalcHPPosition();
    }
    protected virtual void CalcDrawRect() {
      this.drawRectangle = new Rectangle(GameLogic.Field.DrawRectangle.X + this.coords[0].X * GameLogic.Field.DrawCellSize,
              GameLogic.Field.DrawRectangle.Y + this.coords[0].Y * GameLogic.Field.DrawCellSize + 1,
              GameLogic.Field.DrawCellSize - 1, GameLogic.Field.DrawCellSize - 1);
    }
    protected virtual void CalcTextureCoords() {
      this.textureCoords = new Vector2(this.drawRectangle.Center.X - (this.texture.Width >> 1),
          this.drawRectangle.Bottom - (int)(this.drawRectangle.Height * 0.1) - this.texture.Height);
    }
    protected void CalcHPPosition() {
      int hpSize = BFContent.NumberWidth(this.hp);
      hpSize += BFContent.heart.Width + 2;
      this.heartPosition = new Vector2(this.drawRectangle.Center.X - (hpSize >> 1),
          this.drawRectangle.Y + (int)(this.drawRectangle.Height * 0.1));
      this.hpPosition = new Vector2(this.heartPosition.X + BFContent.heart.Width + 2, this.heartPosition.Y);
    }
    protected virtual void CalcBordersRect() {
      this.bordersRectangle = new Rectangle(this.drawRectangle.X + 1, this.drawRectangle.Y,
              this.drawRectangle.Width - 2, this.drawRectangle.Height - 2);
    }
    protected virtual void CalcFillRect() {
      this.fillRectangle = this.drawRectangle;
      this.fillRectangle.Height = (int)(this.fillRectangle.Height * ((float)this.hp / this.maxHP));
      this.fillRectangle.Y += this.drawRectangle.Height - this.fillRectangle.Height;
    }

    public Point GetCoords(int index) { return this.coords[index]; }
    public Point[] GetCoords() { return this.coords; }

    public int GetCoordsLength() { return this.coords.Length; }

    public virtual void Draw(SpriteBatch spriteBatch) {
      Primitives2D.FillRectangle(spriteBatch, this.drawRectangle, this.owner.ColorTheme.ClrHpLeft);
      Primitives2D.FillRectangle(spriteBatch, this.fillRectangle, this.Color);
      spriteBatch.Draw(this.texture, this.textureCoords, this.TextureColor);
      this.DrawHP(spriteBatch);
    }

    public virtual void DrawBorders(SpriteBatch spriteBatch) {
      Primitives2D.DrawRectangle(spriteBatch, this.bordersRectangle, this.Owner.ColorTheme.ClrBorder, 2);
    }

    protected virtual void DrawHP(SpriteBatch spriteBatch) {
      spriteBatch.Draw(BFContent.heart, this.heartPosition, Color.Black);
      BFContent.DrawNumber(this.hp, this.hpPosition, spriteBatch);
    }

    protected void SetFigureColor(ColorTheme clrTheme) {
      if (this is FigureBase) this.Color = clrTheme.ClrFigureBase;
      else if (this is FigureAttack) this.Color = clrTheme.ClrFigureAttack;
      else if (this is FigureDefence) this.Color = clrTheme.ClrFigureDefence;
      else if (this is FigureGold) this.Color = clrTheme.ClrFigureGold;
      this.TextureColor = Color.Black;
    }
  }

  public class FigureBase : Figure {
    public FigureBase(Player player)
        : base(player, BFContent.tower, new Point[] { Point.Zero }, 20, 20) {
      if (player.Number == 1) {
        this.SetCoords(new Point[4] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) });
      } else if (player.Number == 2) {
        this.SetCoords(new Point[4] { new Point(GameLogic.Field.Width - 2, 0), new Point(GameLogic.Field.Width - 1, 0),
                    new Point(GameLogic.Field.Width - 2, 1), new Point(GameLogic.Field.Width - 1, 1) });
      } else if (player.Number == 3) {
        this.SetCoords(new Point[4] { new Point(GameLogic.Field.Width - 2, GameLogic.Field.Height - 2),
                    new Point(GameLogic.Field.Width - 1, GameLogic.Field.Height - 2),
                    new Point(GameLogic.Field.Width - 2, GameLogic.Field.Height - 1),
                    new Point(GameLogic.Field.Width - 1, GameLogic.Field.Height - 1) });
      } else if (player.Number == 4) {
        this.SetCoords(new Point[4] { new Point(0, GameLogic.Field.Height - 2), new Point(1, GameLogic.Field.Height - 2),
                    new Point(0, GameLogic.Field.Height - 1), new Point(1, GameLogic.Field.Height - 1) });
      }
    }

    protected override void CalcDrawRect() {
      this.drawRectangle = new Rectangle(GameLogic.Field.DrawRectangle.X + this.coords[0].X * GameLogic.Field.DrawCellSize,
              GameLogic.Field.DrawRectangle.Y + this.coords[0].Y * GameLogic.Field.DrawCellSize + 1,
              GameLogic.Field.DrawCellSize * 2 - 1, GameLogic.Field.DrawCellSize * 2 - 1);
    }
  }

  public class FigureAttack : Figure {
    protected int damage = 2;
    public int Damage {
      get { return this.damage; }
      set {
        this.damage = value;
        CalcDamagePosition();
      }
    }
    protected bool attacked = false;
    public bool Attacked {
      get { return this.attacked; }
      set {
        this.attacked = value;
        if (this.attacked) this.TextureColor = Color.White;
        else this.TextureColor = Color.Black;
      }
    }
    protected Vector2 DamagePosition;

    protected void CalcDamagePosition() {
      int damSize = BFContent.NumberWidth(this.damage);
      damSize += this.texture.Width + 1;
      this.DamagePosition = new Vector2(this.drawRectangle.Center.X - (damSize >> 1),
          this.drawRectangle.Bottom - (int)(this.drawRectangle.Height * 0.15) - this.texture.Height / 2 - BFContent.charHeight / 2);
      this.textureCoords = new Vector2(this.DamagePosition.X + (damSize - this.texture.Width),
          this.drawRectangle.Bottom - (int)(this.drawRectangle.Height * 0.1) - this.texture.Height);
    }

    protected override void CalcTextureCoords() { }

    protected override void CalcFigureContent() {
      base.CalcFigureContent();
      this.CalcDamagePosition();
    }

    public FigureAttack(Player player, Point coords)
        : base(player, BFContent.sword, new Point[1] { coords }, 4, 4) { }

    public FigureAttack(Player player)
        : base(player, BFContent.sword, new Point[1], 4, 4) {
      if (player.Number == 1) {
        this.SetCoords(new Point(2, 2));
      } else if (player.Number == 2) {
        this.SetCoords(new Point(GameLogic.Field.Width - 3, 2));
      } else if (player.Number == 3) {
        this.SetCoords(new Point(GameLogic.Field.Width - 3, GameLogic.Field.Height - 3));
      } else if (player.Number == 4) {
        this.SetCoords(new Point(2, GameLogic.Field.Height - 3));
      }
    }

    public override void Draw(SpriteBatch spriteBatch) {
      base.Draw(spriteBatch);
      BFContent.DrawNumber(this.damage, this.DamagePosition, spriteBatch);
    }

  }

  public class FigureDefence : Figure {
    public FigureDefence(Player player, Point coords)
        : base(player, BFContent.shield, new Point[1] { coords }, 6, 6) { }

    public FigureDefence(Player player)
        : base(player, BFContent.shield, new Point[1], 6, 6) {
      if (player.Number == 1) {
        this.SetCoords(new Point(2, 1));
      } else if (player.Number == 2) {
        this.SetCoords(new Point(GameLogic.Field.Width - 2, 2));
      } else if (player.Number == 3) {
        this.SetCoords(new Point(GameLogic.Field.Width - 3, GameLogic.Field.Height - 2));
      } else if (player.Number == 4) {
        this.SetCoords(new Point(1, GameLogic.Field.Height - 3));
      }
    }
  }

  public class FigureGold : Figure {
    protected int production = 1;
    public int MinProduction = 1;
    public int MaxProduction = 3;
    public int ProductionCoef = 1;
    public int Production {
      get { return this.production; }
      set {
        this.production = value;
        CalcProdPosition();
      }
    }
    public bool Moved = false;
    protected Vector2 ProductionPosition;

    protected void CalcProdPosition() {
      int prodSize = BFContent.NumberWidth(this.production);
      prodSize += this.texture.Width + 2;
      this.ProductionPosition = new Vector2(this.drawRectangle.Center.X - (prodSize >> 1),
          this.drawRectangle.Bottom - (int)(this.drawRectangle.Height * 0.15) - this.texture.Height / 2 - BFContent.charHeight / 2);
      this.textureCoords = new Vector2(this.ProductionPosition.X + (prodSize - this.texture.Width),
          this.drawRectangle.Bottom - (int)(this.drawRectangle.Height * 0.1) - this.texture.Height);
    }

    protected override void CalcTextureCoords() { }

    protected override void CalcFigureContent() {
      base.CalcFigureContent();
      this.CalcProdPosition();
    }

    public FigureGold(Player player, Point coords)
        : base(player, BFContent.coin, new Point[1] { coords }, 2, 2) {
      this.Production = 1;
    }

    public FigureGold(Player player)
        : base(player, BFContent.coin, new Point[1], 2, 2) {
      if (player.Number == 1) {
        this.SetCoords(new Point(1, 2));
      } else if (player.Number == 2) {
        this.SetCoords(new Point(GameLogic.Field.Width - 3, 1));
      } else if (player.Number == 3) {
        this.SetCoords(new Point(GameLogic.Field.Width - 2, GameLogic.Field.Height - 3));
      } else if (player.Number == 4) {
        this.SetCoords(new Point(2, GameLogic.Field.Height - 2));
      }
    }

    public override void Draw(SpriteBatch spriteBatch) {
      base.Draw(spriteBatch);
      BFContent.DrawNumber(this.production, this.ProductionPosition, spriteBatch);
    }
  }
}
