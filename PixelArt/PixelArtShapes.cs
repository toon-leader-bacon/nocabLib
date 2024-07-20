#if UNITY_5_3_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endif

public static class PixelArtShapes
{
  public static List<Vector2Int> ConnectTheDots(List<Vector2Int> points)
  {
    if (points.Count == 0) { return new List<Vector2Int>(); }
    if (points.Count == 1) { return points; }

    List<Vector2Int> result = new List<Vector2Int>();
    Vector2Int pointA = points[0];
    for (int i = 1; i < points.Count; i++)
    {
      Vector2Int pointB = points[i];
      List<Vector2Int> connecting_line = NocabPixelLine.getPointsAlongLine(pointA, pointB);
      // Avoid the duplicate point issue by removing the last point.
      // The next iteration will start at the end point so that one will be included
      connecting_line.RemoveAt(connecting_line.Count - 1);

      result.AddRange(connecting_line);
      pointA = pointB;
    }
    // Add back the last point
    result.Add(pointA);

    return result;
  }

}
