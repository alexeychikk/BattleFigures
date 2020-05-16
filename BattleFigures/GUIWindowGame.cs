using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleFigures {
  public class GUIWindowGame : GUIWindow {
    public GUIPanel pnlField;
    public GUIButton btnEndTurn;

    public GUIWindowGame(BattleFiguresGame game)
        : base(game.inputManager, game.spriteBatch, BFContent.spriteFont,
            new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight)) {
      this.pnlField = new GUIPanel(GameLogic.Field.DrawRectangle);
      this.pnlField.OnMouseMove += GameLogic.FieldMouseMove;
      this.pnlField.OnMouseEnter += GameLogic.FieldMouseEnter;
      this.pnlField.OnMouseLeave += GameLogic.FieldMouseLeave;
      this.pnlField.OnMouseLeftClick += GameLogic.FieldMouseLeftClick;
      this.OnMouseRightClick += GameLogic.WndMainMouseRightClick;

      Vector2 textSize = BFContent.MeasureString(Localization.EndTurn);
      int tWidth = (int)textSize.X + 20, tHeight = (int)textSize.Y + 5;
      this.btnEndTurn = new GUIButton(Localization.EndTurn, BFContent.spriteFont,
          new Rectangle(GameLogic.Field.DrawRectangle.Center.X - tWidth / 2,
          GameLogic.Field.DrawRectangle.Top - tHeight - 10, tWidth, tHeight));
      this.btnEndTurn.OnMouseLeftClick += GameLogic.EndTurnClick;

      this.AddChild(this.pnlField);
      this.AddChild(this.btnEndTurn);

      foreach (Player p in GameLogic.Players) {
        Vector2 lblPos = Vector2.Zero;
        Vector2 nameSize = BFContent.MeasureString(p.Name);
        Rectangle far = new Rectangle(GameLogic.Field.DrawRectangle.X - GameLogic.Field.DrawCellSize - 5,
            GameLogic.Field.DrawRectangle.Y, GameLogic.Field.DrawCellSize, GameLogic.Field.DrawCellSize), fdr = far, fgr = far;
        if (p.Number == 1) {
          lblPos = new Vector2(GameLogic.Field.DrawRectangle.X, GameLogic.Field.DrawRectangle.Y - nameSize.Y);
        } else if (p.Number == 2) {
          lblPos = new Vector2(GameLogic.Field.DrawRectangle.Right - nameSize.X, GameLogic.Field.DrawRectangle.Y - nameSize.Y);
          far.X = GameLogic.Field.DrawRectangle.Right + 4;
        } else if (p.Number == 3) {
          lblPos = new Vector2(GameLogic.Field.DrawRectangle.Right - nameSize.X, GameLogic.Field.DrawRectangle.Bottom + 2);
          far.X = GameLogic.Field.DrawRectangle.Right + 4;
          far.Y = GameLogic.Field.DrawRectangle.Bottom - GameLogic.Field.DrawCellSize * 3 - 10;
        } else if (p.Number == 4) {
          lblPos = new Vector2(GameLogic.Field.DrawRectangle.X, GameLogic.Field.DrawRectangle.Bottom + 2);
          far.Y = GameLogic.Field.DrawRectangle.Bottom - GameLogic.Field.DrawCellSize * 3 - 10;
        }
        fdr.X = far.X;
        fgr.X = far.X;
        fdr.Y = far.Bottom + 5;
        fgr.Y = fdr.Bottom + 5;

        lblPos.X = (int)lblPos.X;
        lblPos.Y = (int)lblPos.Y;

        this.AddChild(new GUILabel(p.Name, p.ColorTheme.ClrFigureDefence, lblPos));
        GUIFigureButton btnA = new GUIFigureButton(new FigureAttack(p), far);
        GUIFigureButton btnD = new GUIFigureButton(new FigureDefence(p), fdr);
        GUIFigureButton btnG = new GUIFigureButton(new FigureGold(p), fgr);
        p.figureButtons.Add(btnA);
        p.figureButtons.Add(btnD);
        p.figureButtons.Add(btnG);
        foreach (GUIFigureButton btn in p.figureButtons) {
          btn.OnMouseLeftClick += GameLogic.BtnFigureLeftClick;
          btn.OnMouseEnter += GameLogic.BtnFigureEnter;
          btn.OnMouseLeave += GameLogic.BtnFigureLeave;
        }

        if (p != GameLogic.ActivePlayer) btnA.Enabled = btnD.Enabled = btnG.Enabled = false;

        this.AddChild(btnA);
        this.AddChild(btnD);
        this.AddChild(btnG);
      }
    }

    public override void Draw() {
      if (!this.Visible) return;
      GameLogic.Field.Draw(this.SpriteBtch);
      foreach (Player p in GameLogic.Players) p.Draw(this.SpriteBtch);
      base.Draw();
    }
  }
}
