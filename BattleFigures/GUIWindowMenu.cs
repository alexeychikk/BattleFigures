using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleFigures {
  public class GUIWindowMenu : GUIWindow {
    public GUIButton btnSinglePlayer;
    public GUIButton btnOneScreen;
    public GUIButton btnNet;
    public GUIButton btnExit;

    public GUIWindowMenu(BattleFiguresGame game)
        : base(game.inputManager, game.spriteBatch, BFContent.spriteFont,
            new Rectangle(0, 0, game.graphics.PreferredBackBufferWidth, game.graphics.PreferredBackBufferHeight)) {
      Vector2 singlePlayerSize = BFContent.MeasureString(Localization.SinglePlayer);
      //this.btnSinglePlayer = new GUIButton(
    }
  }
}
