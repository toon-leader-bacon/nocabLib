using System.Collections.Generic;

public class AlphaBetaProgression
{

  private int nextIndex = 0;

  private static readonly List<char> list = new List<char>() {
    'a', 'b', 'c', 'd',
    'e', 'f', 'g', 'h',
    'i', 'j', 'k', 'l',
    'm', 'n', 'o', 'p',
    'q', 'r', 's', 't',
    'u', 'v', 'w', 'x',
    'y', 'z'
  };

  public char nextLetter()
  {
    char result = list[nextIndex];
    nextIndex++;
    nextIndex = nextIndex % list.Count;
    return result;
  }

}
