using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using C3.XNA;
using System.Diagnostics;
using MonoGame.Extended;

namespace BattleFigures {
  public class BattleFiguresGame : Game {
    public GraphicsDeviceManager graphics;
    public SpriteBatch spriteBatch;
    public InputManager inputManager;
    public Cursor cursor;
    public GUIWindowGame windowGame;
    private FramesPerSecondCounter fpsCounter;

    public BattleFiguresGame() {
      this.graphics = new GraphicsDeviceManager(this);
      this.Content.RootDirectory = "Content";
      this.fpsCounter = new FramesPerSecondCounter();
      this.IsFixedTimeStep = false;
      this.graphics.SynchronizeWithVerticalRetrace = true;
    }

    protected override void Initialize() {
      base.Initialize();
    }

    protected override void LoadContent() {
      this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
      this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
      this.graphics.ApplyChanges();

      this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      BFContent.Load(this.Content);

      this.inputManager = new InputManager();
      this.inputManager.StartListenInput();
      cursor = new Cursor();

      Localization.Language = Language.English;

      InitGameLogic();
      InitGui();
    }

    protected void InitGameLogic() {
      int width = 20, height = 20, cellSize; //ширина должна быть не меньше высоты
      if (this.graphics.PreferredBackBufferWidth / width > this.graphics.PreferredBackBufferHeight / height)
        cellSize = (this.graphics.PreferredBackBufferHeight - 80) / height;
      else cellSize = (this.graphics.PreferredBackBufferWidth - 200) / width;
      GameLogic.Init(2, cellSize, width, height, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
    }

    protected void InitGui() {
      windowGame = new GUIWindowGame(this);
    }

    protected override void UnloadContent() {

    }

    protected override void Update(GameTime gameTime) {
      fpsCounter.Update(gameTime);
      this.cursor.Update();
      base.Update(gameTime);
    }

    Vector2 fpsPos = new Vector2(10, 10);
    Vector2 cellPos = new Vector2(10, 25);
    protected override void Draw(GameTime gameTime) {
      this.fpsCounter.Draw(gameTime);
      this.GraphicsDevice.Clear(Color.White);

      this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

      this.DrawDebug();

      windowGame.Draw();

      this.cursor.Draw(this.spriteBatch);

      this.spriteBatch.End();

      base.Draw(gameTime);
    }

    protected void DrawDebug() {
      this.spriteBatch.DrawString(
        BFContent.spriteFont,
        $"FPS: {this.fpsCounter.FramesPerSecond}",
        fpsPos, Color.Black, 0, Vector2.Zero, BFContent.fontScaler,
        SpriteEffects.None, 0
      );
      if (GameLogic.Field.FocusedCell != null) {
        this.spriteBatch.DrawString(
          BFContent.spriteFont,
          "cellPart: " + GameLogic.Field.FocusedCell.ActiveCellPart,
          cellPos, Color.Black, 0, Vector2.Zero, BFContent.fontScaler,
          SpriteEffects.None, 0
        );
      } else {
        this.spriteBatch.DrawString(
          BFContent.spriteFont,
          "cellPart: null",
          cellPos, Color.Black, 0, Vector2.Zero, BFContent.fontScaler,
          SpriteEffects.None, 0
        );
      }
    }
  }
}
