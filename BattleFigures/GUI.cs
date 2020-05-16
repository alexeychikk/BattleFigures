using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using C3.XNA;

namespace BattleFigures {
  public delegate void MouseEventHandler(object sender, MouseEventArgs args);

  abstract public class GUIBase {
    public Texture2D BackgroundImage;

    public bool Enabled;

    public Rectangle Rectangle;
    protected string text;
    public virtual string Text { get { return text; } set { text = value; } }
    public Vector2 TextPosition;
    public SpriteFont Font;
    public bool Visible;
    public bool Focused;
    public List<GUIBase> Children;
    public GUIBase Parent;
    public Color DisabledColor;
    public Color FillColor;
    public Color BorderColor;
    public Color TextColor;
    public Color FillColorDr;
    public Color BorderColorDr;
    public Color TextColorDr;
    public Color FillColorMH;
    public Color BorderColorMH;
    public Color TextColorMH;
    public Color FillColorMD;
    public Color BorderColorMD;
    public Color TextColorMD;

    public bool HasChildren {
      get { return this.Children.Count != 0 ? true : false; }
    }

    public event MouseEventHandler OnMouseLeftClick;
    public event MouseEventHandler OnMouseLeftDown;
    public event MouseEventHandler OnMouseLeftUp;
    public event MouseEventHandler OnMouseRightClick;
    public event MouseEventHandler OnMouseRightDown;
    public event MouseEventHandler OnMouseRightUp;
    public event MouseEventHandler OnMouseEnter;
    public event MouseEventHandler OnMouseLeave;
    public event MouseEventHandler OnMouseMove;
    public event MouseEventHandler OnMouseHover;

    protected GUIBase() {
      this.Children = new List<GUIBase>();
      this.text = "";
      this.Visible = true;
      this.Enabled = true;
      this.FillColor = Color.Transparent;
      this.BorderColor = Color.Transparent;
    }

    public void AddChild(GUIBase child) {
      this.Children.Add(child);
      child.Parent = this;
    }

    protected void MouseEvent(MouseState msn, MouseState msp) {
      if (!this.Enabled || !this.Visible) return;
      Point coordsNow = new Point(msn.X, msn.Y), coordsPrev = new Point(msp.X, msp.Y);
      if (this.Rectangle.Contains(coordsNow)) {
        if (this.OnMouseHover != null) this.OnMouseHover(this, new MouseEventArgs(msn, msp));

        if (coordsNow != coordsPrev) {
          if (this.OnMouseMove != null) this.OnMouseMove(this, new MouseEventArgs(msn, msp));
        }
        if (!this.Rectangle.Contains(coordsPrev)) {
          if (this.OnMouseEnter != null) this.OnMouseEnter(this, new MouseEventArgs(msn, msp));
        }

        if (msn.LeftButton == ButtonState.Released) {
          if (this.OnMouseLeftUp != null) this.OnMouseLeftUp(this, new MouseEventArgs(msn, msp));
          if (msp.LeftButton == ButtonState.Pressed) {
            if (this.OnMouseLeftClick != null) this.OnMouseLeftClick(this, new MouseEventArgs(msn, msp));
          }
        } else {
          if (this.OnMouseLeftDown != null) this.OnMouseLeftDown(this, new MouseEventArgs(msn, msp));
        }

        if (msn.RightButton == ButtonState.Released) {
          if (this.OnMouseRightUp != null) this.OnMouseRightUp(this, new MouseEventArgs(msn, msp));
          if (msp.RightButton == ButtonState.Pressed) {
            if (this.OnMouseRightClick != null) this.OnMouseRightClick(this, new MouseEventArgs(msn, msp));
          }
        } else {
          if (this.OnMouseRightDown != null) this.OnMouseRightDown(this, new MouseEventArgs(msn, msp));
        }
      } else if (this.Rectangle.Contains(coordsPrev)) {
        if (this.OnMouseLeave != null) this.OnMouseLeave(this, new MouseEventArgs(msn, msp));
      }
      if (this.HasChildren) foreach (GUIBase gb in this.Children) gb.MouseEvent(msn, msp);

    }

    public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont) {
      if (this.HasChildren) {
        foreach (GUIBase gb in this.Children) {
          gb.Draw(spriteBatch, spriteFont);
        }
      }
    }
  }

  public class GUIWindow : GUIBase {
    protected SpriteBatch SpriteBtch;

    public GUIWindow(InputManager inputManager, SpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle rectangle)
        : base() {
      inputManager.MouseEvent += this.MouseEvent;
      this.Rectangle = rectangle;
      this.SpriteBtch = spriteBatch;
      this.Font = spriteFont;
      this.DisabledColor = new Color(210, 210, 210, 230);
    }

    public virtual void Draw() {
      if (!this.Visible) return;
      Primitives2D.FillRectangle(this.SpriteBtch, this.Rectangle, this.FillColorDr);
      Primitives2D.DrawRectangle(this.SpriteBtch, this.Rectangle, this.BorderColorDr);

      base.Draw(this.SpriteBtch, this.Font);
      if (!this.Enabled) Primitives2D.FillRectangle(this.SpriteBtch, this.Rectangle, this.DisabledColor);
    }
  }

  public class GUIPanel : GUIBase {
    public GUIPanel(Rectangle rectangle)
        : base() {
      this.Rectangle = rectangle;
      this.DisabledColor = new Color(210, 210, 210, 230);
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont) {
      if (!this.Visible) return;
      Primitives2D.FillRectangle(spriteBatch, this.Rectangle, this.FillColorDr);
      Primitives2D.DrawRectangle(spriteBatch, this.Rectangle, this.BorderColorDr);

      base.Draw(spriteBatch, this.Font);
      if (!this.Enabled) Primitives2D.FillRectangle(spriteBatch, this.Rectangle, this.DisabledColor);
    }
  }

  public class GUIButton : GUIBase {
    public GUIButton(String text, SpriteFont font, Rectangle rectangle)
        : base() {
      this.Rectangle = rectangle;

      this.BorderColor = this.BorderColorDr = Color.SaddleBrown;
      this.FillColor = this.FillColorDr = GameLogic.Field.ColorField;
      this.TextColor = this.TextColorDr = Color.Maroon;

      this.BorderColorMH = Color.Peru;
      this.FillColorMH = GameLogic.Field.ColorField;
      this.TextColorMH = Color.SaddleBrown;

      this.BorderColorMD = Color.Gold;
      this.FillColorMD = GameLogic.Field.ColorField;
      this.TextColorMD = Color.Orange;

      this.DisabledColor = new Color(200, 200, 200, 100);

      this.Font = font;
      this.Text = text;
      this.OnMouseHover += GUIButton_OnMouseHover;
      this.OnMouseLeave += GUIButton_OnMouseLeave;
      this.OnMouseLeftDown += GUIButton_OnMouseLeftDown;
    }

    void GUIButton_OnMouseLeftDown(object sender, MouseEventArgs args) {
      this.FillColorDr = this.FillColorMD;
      this.BorderColorDr = this.BorderColorMD;
      this.TextColorDr = this.TextColorMD;
    }

    void GUIButton_OnMouseLeave(object sender, MouseEventArgs args) {
      this.FillColorDr = this.FillColor;
      this.BorderColorDr = this.BorderColor;
      this.TextColorDr = this.TextColor;
    }

    void GUIButton_OnMouseHover(object sender, MouseEventArgs args) {
      this.FillColorDr = this.FillColorMH;
      this.BorderColorDr = this.BorderColorMH;
      this.TextColorDr = this.TextColorMH;
    }

    public override string Text {
      get {
        return this.text;
      }
      set {
        this.text = value;
        Vector2 textSize = BFContent.MeasureString(this.text);
        this.TextPosition = new Vector2(this.Rectangle.Center.X - textSize.X / 2, this.Rectangle.Center.Y - textSize.Y / 2 + 2);
        this.TextPosition.X = (int)this.TextPosition.X;
        this.TextPosition.Y = (int)this.TextPosition.Y;
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont) {
      if (!this.Visible) return;
      Primitives2D.FillRectangle(spriteBatch, this.Rectangle, this.FillColorDr);
      Primitives2D.DrawRectangle(spriteBatch, this.Rectangle, this.BorderColorDr, 2);
      spriteBatch.DrawString(spriteFont, this.text, this.TextPosition, this.TextColorDr, 0f, Vector2.Zero, BFContent.fontScaler, SpriteEffects.None, 0f);
      base.Draw(spriteBatch, spriteFont);
      if (!this.Enabled) Primitives2D.FillRectangle(spriteBatch, this.Rectangle, this.DisabledColor);
    }
  }

  public class GUIFigureButton : GUIButton {
    public Figure figure;
    public int Price;
    public Vector2 PricePosition;
    public bool ShowPrice;
    public bool Selected;

    public GUIFigureButton(Figure f, Rectangle rectangle) : base("", null, rectangle) {
      this.figure = f;
      this.figure.DrawRectangle = rectangle;
      this.DisabledColor = new Color(200, 200, 200, 50);
      this.BorderColor = this.BorderColorDr = Color.Black;
      this.OnMouseEnter += GUIFigureButton_OnMouseEnter;
      this.OnMouseLeave += GUIFigureButton_OnMouseLeave;

      this.Price = GameLogic.FigurePrice(this.figure);
      Vector2 priceSize = new Vector2(BFContent.NumberWidth(this.Price) + BFContent.coin.Width + 2, BFContent.coin.Height);
      if (this.figure.Owner.Number == 1 || this.figure.Owner.Number == 4)
        this.PricePosition = new Vector2(this.Rectangle.X - priceSize.X - 5, this.Rectangle.Center.Y - priceSize.Y / 2 + 2);
      else this.PricePosition = new Vector2(this.Rectangle.Right + 5, this.Rectangle.Center.Y - priceSize.Y / 2 + 2);
    }

    void GUIFigureButton_OnMouseLeave(object sender, MouseEventArgs args) {
      this.ShowPrice = false;
    }

    void GUIFigureButton_OnMouseEnter(object sender, MouseEventArgs args) {
      this.ShowPrice = true;
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont) {
      if (!this.Visible) return;
      this.figure.Draw(spriteBatch);
      Primitives2D.DrawRectangle(spriteBatch, this.Rectangle, this.BorderColorDr, 1);
      if (this.ShowPrice) BFContent.DrawCoins(this.Price, this.PricePosition, spriteBatch, Color.SaddleBrown, Color.Gold);
      if (this.Selected) this.figure.DrawBorders(spriteBatch);
      if (!this.Enabled) Primitives2D.FillRectangle(spriteBatch, this.Rectangle, this.DisabledColor);
    }
  }

  public class GUILabel : GUIBase {
    public GUILabel(string text, Color textColor, Vector2 position)
        : base() {
      this.text = text;
      this.TextColor = this.TextColorDr = textColor;
      this.TextPosition = position;
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont) {
      spriteBatch.DrawString(spriteFont, this.text, this.TextPosition, this.TextColorDr, 0, Vector2.Zero, BFContent.fontScaler,
          SpriteEffects.None, 0);
      base.Draw(spriteBatch, spriteFont);
    }
  }
}
