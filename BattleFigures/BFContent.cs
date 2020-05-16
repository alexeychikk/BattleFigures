using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace BattleFigures {
  static public class BFContent {
    static public Texture2D sword;
    static public Texture2D tower;
    static public Texture2D shield;
    static public Texture2D coin;
    static public Texture2D heart;
    static public Texture2D cursor;
    static public Texture2D movement;
    static public SpriteFont spriteFont;
    static public float fontScaler = 0.5f;

    static public Texture2D digits;
    static private Rectangle[] digitsRecs;
    static public Texture2D chars;
    static public int charOffset = 6;
    static public int charWidth = 5;
    static public int charHeight = 7;

    static public Dictionary<char, Rectangle> charsRecs = new Dictionary<char, Rectangle>();

    static public void Load(ContentManager Content) {
      sword = Content.Load<Texture2D>(@"Textures\sword");
      tower = Content.Load<Texture2D>(@"Textures\tower");
      shield = Content.Load<Texture2D>(@"Textures\shield");
      coin = Content.Load<Texture2D>(@"Textures\coin");
      heart = Content.Load<Texture2D>(@"Textures\heart");
      cursor = Content.Load<Texture2D>(@"Textures\cursor");
      chars = Content.Load<Texture2D>(@"Textures\chars");
      movement = Content.Load<Texture2D>(@"Textures\movement");
      spriteFont = Content.Load<SpriteFont>(@"Fonts\Arial");

      digits = Content.Load<Texture2D>(@"Textures\digits");
      CreateDigitsRecs();
      CreateCharsRecs();
    }

    static private void CreateDigitsRecs() {
      digitsRecs = new Rectangle[10];
      for (int i = 0; i < digitsRecs.Length; i++) {
        digitsRecs[i] = new Rectangle(i * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight);
      }
    }

    static private void CreateCharsRecs() {
      charsRecs.Add('-', new Rectangle(0 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('+', new Rectangle(1 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('*', new Rectangle(2 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('/', new Rectangle(3 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('=', new Rectangle(4 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('(', new Rectangle(5 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add(')', new Rectangle(6 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add('.', new Rectangle(7 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add(',', new Rectangle(8 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add(':', new Rectangle(9 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
      charsRecs.Add(';', new Rectangle(10 * BFContent.charOffset, 0, BFContent.charWidth, BFContent.charHeight));
    }

    static public void DrawChar(char ch, Vector2 position, SpriteBatch spriteBatch, Color clr) {
      spriteBatch.Draw(chars, position, charsRecs[ch], clr);
    }

    static public void DrawChar(char ch, Vector2 position, SpriteBatch spriteBatch) {
      spriteBatch.Draw(chars, position, charsRecs[ch], Color.Black);
    }

    static public void DrawNumber(int num, Vector2 position, SpriteBatch spriteBatch) {
      DrawNumber(num, position, spriteBatch, Color.Black);
    }

    static public void DrawNumber(int num, Vector2 position, SpriteBatch spriteBatch, Color clr) {
      if (num == 0) {
        spriteBatch.Draw(BFContent.digits, position, digitsRecs[0], clr);
        return;
      }
      if (num < 0) {
        DrawChar('-', position, spriteBatch, clr);
        position.X += charOffset;
        num = Math.Abs(num);
      }
      int index = 0;
      BFContent.DrawNumberRecursion(num, position, spriteBatch, clr, ref index);
    }

    static private void DrawNumberRecursion(int num, Vector2 position, SpriteBatch spriteBatch, Color clr, ref int index) {
      if (num != 0) {
        BFContent.DrawNumberRecursion(num / 10, position, spriteBatch, clr, ref index);
        position.X += charOffset * index;
        spriteBatch.Draw(BFContent.digits, position, digitsRecs[num % 10], clr);
        index++;
      }
    }

    public static int NumberWidth(int num) {
      if (Math.Abs(num) < 10) return (num < 0 ? charOffset * 2 - 1 : charWidth);
      return (int)Math.Floor(Math.Log10(num) + 1) * charOffset - 1 + (num < 0 ? charOffset : 0);
    }

    public static Vector2 CoinsSize(int coins) {
      return new Vector2(NumberWidth(coins) + 2 + coin.Width, coin.Height);
    }

    public static Vector2 MovementPointsSize(int points, int maxPoints) {
      return new Vector2(NumberWidth(points) + charWidth + NumberWidth(maxPoints) + 11 + movement.Width, movement.Height);
    }

    static public Vector2 MeasureString(string str) {
      Vector2 res = spriteFont.MeasureString(str);
      res.X *= fontScaler;
      res.Y *= fontScaler;
      return res;
    }

    static public void DrawCoins(int num, Vector2 position, SpriteBatch spriteBatch, Color clrNum, Color clrCoin) {
      DrawNumber(num, position, spriteBatch, clrNum);
      spriteBatch.Draw(coin, new Vector2(position.X + NumberWidth(num) + 2, position.Y - 2), clrCoin);
    }

    static public void DrawMovementPoints(int mv, int maxmv, Vector2 position, SpriteBatch spriteBatch, Color clrMvLeft, Color clrMvMax, Color clrTexture) {
      DrawNumber(mv, position, spriteBatch, clrMvLeft);
      int w = NumberWidth(mv) + 3;
      DrawChar('/', new Vector2(position.X + w, position.Y), spriteBatch, clrMvMax);
      w += charWidth + 3;
      DrawNumber(maxmv, new Vector2(position.X + w, position.Y), spriteBatch, clrMvMax);
      w += NumberWidth(maxmv) + 5;
      spriteBatch.Draw(movement, new Vector2(position.X + w, position.Y - 3), clrTexture);
    }
  }
}
