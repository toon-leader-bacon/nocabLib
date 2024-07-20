using System.Collections.Generic;
using Unity.Mathematics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

public static class NocabPixelLine
{

  public static List<Vector2Int> getPointsAlongLine(Vector2Int start, Vector2Int end)
  {
    // flat lines (IE lines with a 0 or inf slope)
    if ((start.x == end.x) || (start.y == end.y))
    {
      return getPointsAlongLine_Flat(start, end);
    }

    // Lines with a slope
    return getPointsAlongLine_Bresenham(start, end);
  }

  static List<Vector2Int> getPointsAlongLine_Flat(Vector2Int start, Vector2Int end)
  {
    // Straight lines
    if (start.x == end.x)
    {
      // vertical line
      if (start.y > end.y)
      {
        // Line going downwards
        List<Vector2Int> result = verticalLine_up(end.y, start.y, start.x);
        result.Reverse();
        return result;
      }
      else
      { return verticalLine_up(start.y, end.y, start.x); }
    }
    else // start.y == end.y
    {
      // horizontal line
      if (start.x > end.x)
      {
        // Line going leftwards
        List<Vector2Int> result = horizontalLine_right(end.x, start.x, start.y);
        result.Reverse();
        return result;
      }
      else
      { return horizontalLine_right(start.x, end.x, start.x); }
    }
  }

  static List<Vector2Int> getPointsAlongLine_Bresenham(Vector2Int start, Vector2Int end)
  {
    // Implemented using the Bresenham line algorithm
    // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    if (math.abs(end.y - start.y) < math.abs(end.x - start.x))
    {
      if (start.x > end.x)
      {
        List<Vector2Int> result = PlotLineLow(end, start);
        result.Reverse();
        return result;
      }
      else
      { return PlotLineLow(start, end); }
    }
    else
    {
      if (start.y > end.y)
      {
        List<Vector2Int> result = PlotLineHigh(end, start);
        result.Reverse();
        return result;
      }
      else
      { return PlotLineHigh(start, end); }
    }
  }

  static List<Vector2Int> PlotLineLow(Vector2Int start, Vector2Int end)
  {
    List<Vector2Int> result = new List<Vector2Int>();

    int dx = end.x - start.x;
    int dy = end.y - start.y;

    int y_sign = 1;
    if (dy < 0)
    {
      y_sign = -1;
      dy = -dy;
    }

    int error = (2 * dy) - dx;
    int current_y = start.y;

    // for each x position between the start and finish
    // IE: Always move 1 in the x direction
    for (int current_x = start.x; current_x <= end.x; current_x++)
    {
      result.Add(new Vector2Int(current_x, current_y));

      // Check to see if the error has built up enough to move 
      // in the y direction
      if (error > 0)
      {
        current_y += y_sign;
        error += 2 * (dy - dx);
      }
      else
      {
        // Else not enough error build up. Say on the same y 
        // level but increase the error. 
        error += 2 * dy;
      }
    }
    return result;
  }

  static List<Vector2Int> PlotLineHigh(Vector2Int start, Vector2Int end)
  {
    List<Vector2Int> result = new List<Vector2Int>();

    int dx = end.x - start.x;
    int dy = end.y - start.y;

    int x_sign = 1;
    if (dx < 0)
    {
      x_sign = -1;
      dx = -dx;
    }

    int error = (2 * dx) - dy;
    int current_x = start.x;

    // for each y position between the start and finish
    // IE: Always move 1 in the y direction
    for (int current_y = start.y; current_y <= end.y; current_y++)
    {
      result.Add(new Vector2Int(current_x, current_y));

      // Check to see if the error has built up enough to move 
      // in the x direction
      if (error > 0)
      {
        current_x += x_sign;
        error += 2 * (dx - dy);
      }
      else
      {
        // Else not enough error build up. Say on the same x 
        // level but increase the error. 
        error += 2 * dx;
      }
    }
    return result;
  }

  static List<Vector2Int> horizontalLine_right(int x_start, int x_end, int y)
  {
    if (x_start > x_end)
    {
      int swap = x_start;
      x_start = x_end;
      x_end = swap;
    }
    List<Vector2Int> result = new List<Vector2Int>();
    for (int current_x = x_start; current_x <= x_end; current_x++)
    {
      result.Add(new Vector2Int(current_x, y));
    }
    return result;
  }

  static List<Vector2Int> verticalLine_up(int y_start, int y_end, int x)
  {
    // Only draws lines in the positive y direction
    if (y_start > y_end)
    {
      int swap = y_start;
      y_start = y_end;
      y_end = swap;
    }
    List<Vector2Int> result = new List<Vector2Int>();
    for (int current_y = y_start; current_y <= y_end; current_y++)
    {
      result.Add(new Vector2Int(x, current_y));
    }
    return result;
  }

}
