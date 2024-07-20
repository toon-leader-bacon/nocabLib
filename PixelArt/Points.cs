public interface IPoint<T>
{
  T getX();
  T getY();
}


public class PointInt : IPoint<int>
{
  public PointInt(int x, int y)
  {
    this.X = x;
    this.Y = y;
  }
  public PointInt(PointInt other)
  {
    this.X = other.X;
    this.Y = other.Y;
  }
  public int X { get; set; }
  public int Y { get; set; }
  public int getX() { return X; }
  public int getY() { return Y; }
}

public class PointDouble : IPoint<double>
{
  public double X { get; set; }
  public double Y { get; set; }

  public PointDouble(double x, double y)
  {
    this.X = x;
    this.Y = y;
  }

  public PointDouble(PointDouble other)
  {
    this.X = other.X;
    this.Y = other.Y;
  }
  public double getX() { return X; }
  public double getY() { return Y; }
}

public class PointLong : IPoint<long>
{
  public long X { get; set; }
  public long Y { get; set; }

  public PointLong(long x, long y)
  {
    this.X = x;
    this.Y = y;
  }

  public PointLong(PointLong other)
  {
    this.X = other.X;
    this.Y = other.Y;
  }
  public long getX() { return X; }
  public long getY() { return Y; }
}

