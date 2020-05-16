using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleFigures {
  static public class Path {
    static public int[,] tempMap;
    static public List<Point> tempObstacles = new List<Point>();

    static public List<Point> FindMovementRadius(Point[] points, int movementPoints, int[,] map) {
      int d = 0;
      List<Point> wave = new List<Point>();
      List<Point> waveNew = new List<Point>();
      List<Point> waveRes = new List<Point>();
      tempObstacles = new List<Point>();
      wave.AddRange(points);
      for (; d < movementPoints && wave.Count != 0; d++) {
        for (int i = 0; i < wave.Count; i++) {
          if (wave[i].Y - 1 >= 0) {
            if (map[wave[i].Y - 1, wave[i].X] == 0) {
              map[wave[i].Y - 1, wave[i].X] = d + 1;
              waveNew.Add(new Point(wave[i].X, wave[i].Y - 1));
            } else tempObstacles.Add(new Point(wave[i].X, wave[i].Y - 1));
          }
          if (wave[i].X + 1 < GameLogic.Field.Width) {
            if (map[wave[i].Y, wave[i].X + 1] == 0) {
              map[wave[i].Y, wave[i].X + 1] = d + 1;
              waveNew.Add(new Point(wave[i].X + 1, wave[i].Y));
            } else tempObstacles.Add(new Point(wave[i].X + 1, wave[i].Y));
          }
          if (wave[i].Y + 1 < GameLogic.Field.Height) {
            if (map[wave[i].Y + 1, wave[i].X] == 0) {
              map[wave[i].Y + 1, wave[i].X] = d + 1;
              waveNew.Add(new Point(wave[i].X, wave[i].Y + 1));
            } else tempObstacles.Add(new Point(wave[i].X, wave[i].Y + 1));
          }
          if (wave[i].X - 1 >= 0) {
            if (map[wave[i].Y, wave[i].X - 1] == 0) {
              map[wave[i].Y, wave[i].X - 1] = d + 1;
              waveNew.Add(new Point(wave[i].X - 1, wave[i].Y));
            } else tempObstacles.Add(new Point(wave[i].X - 1, wave[i].Y));
          }
        }
        wave = new List<Point>(waveNew);
        waveRes.AddRange(waveNew);
        waveNew = new List<Point>();
      }
      tempMap = map;
      return waveRes;
    }

    static public List<Point> FindMovementRadius(Point point, int movementPoints) {
      return FindMovementRadius(new Point[] { point }, movementPoints, FieldToMap());
    }

    static public List<Point> FindMovementRadius(Point[] points, int movementPoints) {
      return FindMovementRadius(points, movementPoints, FieldToMap());
    }

    static public List<Point> FindPath(Point[] from, Point to, int[,] map) {
      List<Point> path = new List<Point>();
      path.Add(to);
      foreach (Point p in from)
        if (Math.Abs(p.Y - to.Y) + Math.Abs(p.X - to.X) == 1) {
          return path;
        }
      while (!from.Contains(to)) {
        if (to.Y - 1 >= 0 && map[to.Y - 1, to.X] == map[to.Y, to.X] - 1) {
          to = new Point(to.X, to.Y - 1);
          path.Add(to);
        } else if (to.X + 1 < GameLogic.Field.Width && map[to.Y, to.X + 1] == map[to.Y, to.X] - 1) {
          to = new Point(to.X + 1, to.Y);
          path.Add(to);
        } else if (to.Y + 1 < GameLogic.Field.Height && map[to.Y + 1, to.X] == map[to.Y, to.X] - 1) {
          to = new Point(to.X, to.Y + 1);
          path.Add(to);
        } else if (to.X - 1 >= 0 && map[to.Y, to.X - 1] == map[to.Y, to.X] - 1) {
          to = new Point(to.X - 1, to.Y);
          path.Add(to);
        } else break;
      }
      return path;
    }

    static public List<Point> FindPath(Point from, Point to) {
      return FindPath(new Point[] { from }, to, tempMap);
    }

    static public List<Point> FindPath(Point[] from, Point to) {
      return FindPath(from, to, tempMap);
    }

    static public int[,] FieldToMap() {
      int[,] map = new int[GameLogic.Field.Height, GameLogic.Field.Width];
      for (int i = 0; i < GameLogic.Field.Height; i++)
        for (int j = 0; j < GameLogic.Field.Width; j++)
          map[i, j] = GameLogic.Field[i, j].Empty ? 0 : -1;
      return map;
    }
  }
}
