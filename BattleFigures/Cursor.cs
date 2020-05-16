using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BattleFigures {
  public class Cursor {
    Texture2D Texture;
    MouseState CursorStateNow;
    MouseState CursorStatePrev;
    Vector2 Position;

    public Cursor() {
      this.Texture = BFContent.cursor;
      this.Position = new Vector2(1, 1);
    }

    public void Update() {
      this.CursorStateNow = Mouse.GetState();
      this.Position.X = this.CursorStateNow.X;
      this.Position.Y = this.CursorStateNow.Y;
      this.CursorStatePrev = CursorStateNow;
    }

    public void Draw(SpriteBatch spriteBatch) {
      spriteBatch.Draw(this.Texture, this.Position, Color.White);
    }
  }
}
