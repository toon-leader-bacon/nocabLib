using System;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

public readonly struct Box2D
{
  public readonly float Left_X;
  public readonly float Top_Y;
  public readonly float Right_X;
  public readonly float Bottom_Y;
  public readonly bool PositiveYDown;

  public float Width
  {
    get { return Math.Abs(Right_X - Left_X); }
  }
  public float Height
  {
    get { return Math.Abs(Bottom_Y - Top_Y); }
  }

  public Box2D(float x0, float y0, float x1, float y1, bool positiveYDown = true)
  {
    /**
     * Provide the two opposite corner points. Typically the top left and bottom right, but
     * this also works if you provide the top right and bottom left. The key point is that
     * the two points are opposite of each other.
     *
     * Put another way, this constructor will return the smallest 2d box that contains the provided
     * points.
     */
    float tl_x = Math.Min(x0, x1);
    float tl_y = positiveYDown
      ?
      // If pos y is down, then the Top Left point is the most negative
      Math.Min(y0, y1)
      :
      // Else pos y is up, so the TL is the most positive
      Math.Max(y0, y1);

    // Opposite of the Tl point
    float br_x = Math.Max(x0, x1);
    float br_y = positiveYDown ? Math.Max(y0, y1) : Math.Min(y0, y1);

    this.Left_X = tl_x;
    this.Top_Y = tl_y;
    this.Right_X = br_x;
    this.Bottom_Y = br_y;
    this.PositiveYDown = positiveYDown;
  }

  public static Box2D Box2D_TL_WithHeight(
    float topLeft_x,
    float topLeft_y,
    float width,
    float height,
    bool positiveYDown = true
  )
  {
    width = Math.Abs(width);
    height = Math.Abs(height);

    float right_x = topLeft_x + width;
    float Bottom_Y = topLeft_y + (positiveYDown ? height : -height);
    return new Box2D(topLeft_x, topLeft_y, right_x, Bottom_Y, positiveYDown);
  }

  public static Box2D Box2D_CenterPt(
    float centerX,
    float centerY,
    float width,
    float height,
    bool positiveYDown = true
  )
  {
    width = Math.Abs(width);
    height = Math.Abs(height);

    float left_x = centerX - (width / 2); // Move left half the width

    // Move away from the 'down direction' by half the width
    float top_y = centerY - (positiveYDown ? (height / 2) : -(height / 2));

    return Box2D_TL_WithHeight(left_x, top_y, width, height, positiveYDown);
  }

  #region Side Length

  public float TopSideLength
  {
    get { return Width; }
  }

  public float BottomSideLength
  {
    get { return Width; }
  }

  public float LeftSideLength
  {
    get { return Height; }
  }

  public float RightSideLength
  {
    get { return Height; }
  }

  #endregion

  #region points

  public (float x, float y) TopLeft
  {
    get { return (Left_X, Top_Y); }
  }
  public (float x, float y) TL
  {
    get { return TopLeft; }
  }
  public Vector2 TL_v
  {
    get { return new Vector2(TL.x, TL.y); }
  }

  public (float x, float y) TopRight
  {
    get { return (Right_X, Top_Y); }
  }
  public (float x, float y) TR
  {
    get { return TopRight; }
  }
  public Vector2 TR_v
  {
    get { return new Vector2(TR.x, TR.y); }
  }

  public (float x, float y) BottomLeft
  {
    get { return (Left_X, Bottom_Y); }
  }
  public (float x, float y) BL
  {
    get { return BottomLeft; }
  }
  public Vector2 BL_v
  {
    get { return new Vector2(BL.x, BL.y); }
  }

  public (float x, float y) BottomRight
  {
    get { return (Right_X, Bottom_Y); }
  }
  public (float x, float y) BR
  {
    get { return BottomRight; }
  }
  public Vector2 BR_v
  {
    get { return new Vector2(BR.x, BR.y); }
  }

  public (float x, float y) Center
  {
    get { return ((Left_X + Right_X) / 2, (Top_Y + Bottom_Y) / 2); }
  }
  public Vector2 Center_v
  {
    get { return new Vector2(Center.x, Center.y); }
  }

  #endregion

  bool yInRange(float y)
  {
    // If pos Y is down, then the largest value is at the bottom.
    // Otherwise, the large value is at the top
    (float large_y, float small_y) = PositiveYDown ? (Bottom_Y, Top_Y) : (Top_Y, Bottom_Y);
    return small_y <= y && y <= large_y;
  }

  bool xInRange(float x)
  {
    // Left x will always be smaller than right x
    return Left_X <= x && x <= Right_X;
  }

  public bool Contains(float x, float y)
  {
    return xInRange(x) && yInRange(y);
  }

  public bool IsOnEdge(Vector2Int pt)
  {
    return IsOnLeftEdge(pt) || IsOnRightEdge(pt) || IsOnTopEdge(pt) || IsOnBottomEdge(pt);
  }

  public bool IsOnLeftEdge(Vector2Int pt)
  {
    return pt.x == Left_X && yInRange(pt.y);
  }

  public bool IsOnRightEdge(Vector2Int pt)
  {
    return pt.x == Right_X && yInRange(pt.y);
  }

  public bool IsOnTopEdge(Vector2Int pt)
  {
    return pt.y == Top_Y && xInRange(pt.x);
  }

  public bool IsOnBottomEdge(Vector2Int pt)
  {
    return pt.y == Bottom_Y && xInRange(pt.x);
  }

  public HashSet<Vector2Int> GetAllEdgePoints()
  {
    HashSet<Vector2Int> resultPoints = new();
    resultPoints.UnionWith(NocabPixelLine.getPointsAlongLine(TL_v, TR_v));
    resultPoints.UnionWith(NocabPixelLine.getPointsAlongLine(TR_v, BR_v));
    resultPoints.UnionWith(NocabPixelLine.getPointsAlongLine(BR_v, BL_v));
    resultPoints.UnionWith(NocabPixelLine.getPointsAlongLine(BL_v, TL_v));
    return resultPoints;
  }

  #region Edge Relationship Checks

  /// <summary>
  /// Returns true if this box and other share any edge segment (edges are coincident and overlapping).
  /// This checks if one box's left/right edge aligns with the other's right/left edge (with overlapping y),
  /// or if top/bottom edges align (with overlapping x).
  /// Note: Returns true even if boxes also overlap in their interiors.
  /// </summary>
  public bool SharesEdge(Box2D other)
  {
    // Check vertical edges: this.left == other.right or this.right == other.left
    if (this.Left_X == other.Right_X && YRangesOverlap(other))
      return true;
    if (this.Right_X == other.Left_X && YRangesOverlap(other))
      return true;

    // Check horizontal edges: this.top == other.bottom or this.bottom == other.top
    if (this.Top_Y == other.Bottom_Y && XRangesOverlap(other))
      return true;
    if (this.Bottom_Y == other.Top_Y && XRangesOverlap(other))
      return true;

    return false;
  }

  /// <summary>
  /// Returns true if this box's edge is exactly airGapSize tiles away from other's edge (parallel and adjacent),
  /// but the boxes do NOT overlap or share an edge. This is the problematic "double wall" case.
  /// The airgap size of 0 means the edges are exactly adjacent. An airgap size of 1 means the edges have
  /// one tile of space between them, etc.
  /// </summary>
  public bool IsEdgeTouching(Box2D other, int airGapSize = 0)
  {
    airGapSize = Math.Abs(airGapSize) + 1; // Add 1 because, otherwise, this is just the SharesEdge check above.
    // Check if vertical edges are exactly 1 unit apart with overlapping Y ranges
    // (this.right is 1 left of other.left, or other.right is 1 left of this.left)
    bool verticalTouching =
      (this.Right_X + airGapSize == other.Left_X || other.Right_X + airGapSize == this.Left_X)
      && YRangesOverlap(other);

    if (verticalTouching)
      return true;

    // Check if horizontal edges are exactly 1 unit apart with overlapping X ranges
    // Normalize Y coordinates since Top_Y/Bottom_Y meaning depends on PositiveYDown
    float thisYMax = Math.Max(this.Top_Y, this.Bottom_Y);
    float thisYMin = Math.Min(this.Top_Y, this.Bottom_Y);
    float otherYMax = Math.Max(other.Top_Y, other.Bottom_Y);
    float otherYMin = Math.Min(other.Top_Y, other.Bottom_Y);

    bool horizontalTouching =
      (thisYMax + airGapSize == otherYMin || otherYMax + airGapSize == thisYMin)
      && XRangesOverlap(other);

    return horizontalTouching;
  }

  public bool YRangesOverlap(Box2D other)
  {
    // Normalize y ranges (handles both PositiveYDown true/false)
    float thisYMin = Math.Min(this.Top_Y, this.Bottom_Y);
    float thisYMax = Math.Max(this.Top_Y, this.Bottom_Y);
    float otherYMin = Math.Min(other.Top_Y, other.Bottom_Y);
    float otherYMax = Math.Max(other.Top_Y, other.Bottom_Y);
    return thisYMax >= otherYMin && otherYMax >= thisYMin;
  }

  public bool XRangesOverlap(Box2D other)
  {
    // Left_X <= Right_X is guaranteed by constructor
    return this.Right_X >= other.Left_X && other.Right_X >= this.Left_X;
  }

  #endregion
}
