using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PixelUtility
{


  public static bool pointInRect(Vector2Int point, RectInt rect)
  {
    /**
     * Returns true if the point is inside of (including on the edge of)
     * the provided rect.
     */

    int leftEdge = rect.x;
    int rightEdge = rect.x + rect.width - 1;

    // B/c unity (0,0) is bot left, closeEdge is closest to 0
    // farEdge is away from 0.
    int closeEdge = rect.y;
    int fareEdge = rect.y + rect.height - 1;

    return (leftEdge <= point.x) && (point.x <= rightEdge) &&
           (closeEdge <= point.y) && (point.y <= fareEdge);
  }


  public static RectInt? pointInAnyRect(Vector2Int point, IEnumerable<RectInt> rects)
  {
    /**
     * Checks the provided point for collision (including edge of) every rect in the 
     * rects paramater. The first rectangle colision with the point is returned.
     * Otherwise, null is returned if no collision found. 
     */
    foreach (RectInt rect in rects)
    {
      if (pointInRect(point, rect)) { return rect; }
    }
    return null;
  }

}
