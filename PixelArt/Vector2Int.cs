#if !UNITY_5_3_OR_NEWER
public struct Vector2Int
{
  public int x;
  public int y;

  public Vector2Int(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public override string ToString()
  {
    return $"({x}, {y})";
  }

  // Implement other methods and operators as needed
}
#endif