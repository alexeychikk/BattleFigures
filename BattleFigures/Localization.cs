using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleFigures {
  public enum Language { English, Russian };

  public class Localization {
    protected static int index = 1;
    protected static Language language = BattleFigures.Language.Russian;
    public static Language Language {
      get { return language; }
      set {
        language = value;
        index = (int)language;
      }
    }

    protected static string[] endTurn = { "End turn", "Завершить ход" };
    public static string EndTurn { get { return endTurn[index]; } }

    protected static string[] singlePlayer = { "Single player", "Одиночная игра" };
    public static string SinglePlayer { get { return singlePlayer[index]; } }

    protected static string[] oneScreen = { "One screen", "Один экран" };
    public static string OneScreen { get { return oneScreen[index]; } }

    protected static string[] networkGame = { "Network game", "Сетевая игра" };
    public static string NetworkGame { get { return networkGame[index]; } }

    protected static string[] exit = { "Exit", "Выход" };
    public static string Exit { get { return exit[index]; } }

    protected static string[] player = { "Player", "Игрок" };
    public static string Player { get { return player[index]; } }
  }
}
