using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

public struct Box2D
{
  public readonly float TopLeft_X;
  public readonly float TopLeft_Y;
  public readonly float Width;
  public readonly float Height;
  public readonly int Y_DownSign;

  public Box2D(float topLeft_x, float topLeft_y, float width, float height, bool positiveYDown = true)
  {
    TopLeft_X = topLeft_x;
    TopLeft_Y = topLeft_y;
    Width = Math.Abs(width);
    Height = Math.Abs(height);
    Y_DownSign = positiveYDown ? 1 : -1;
  }

  public static Box2D Box2D_CenterPt(float centerX, float centerY, float width, float height, bool positiveYDown = true)
  {
    width = Math.Abs(width);
    height = Math.Abs(height);

    float topLeft_X = centerX - (width / 2); // Move left half the width

    // Move away from the 'down direction' by half the width
    int y_DownSign = positiveYDown ? 1 : -1;
    float topLeft_Y = centerY - (y_DownSign * (height / 2));

    return new Box2D(topLeft_X, topLeft_Y, width, height, positiveYDown);
  }

  public static Box2D Box2D_CornerPoints(float x0, float y0, float x1, float y1, bool positiveYDown = true)
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
    float tl_y = positiveYDown ? Math.Min(y0, y1) : Math.Max(y0, y1);
    float width = x1 - x0;
    float height = Math.Abs(y1 - y0); // May not be needed
    return new Box2D(tl_x, tl_y, width, height, positiveYDown);
  }

  #region Side Length

  public float TopSideLength
  {
    get { return Width; }
  }

  public float BottomSideLength
  {
    get { return TopSideLength; }
  }

  public float LeftSideLength
  {
    get { return Height; }
  }

  public float RightSideLength
  {
    get { return LeftSideLength; }
  }

  #endregion

  #region points

  public (float x, float y) TopLeft
  {
    get { return (TopLeft_X, TopLeft_Y); }
  }
  public (float x, float y) TL { get { return TopLeft; } }
  public Vector2 TL_v { get { return new Vector2(TL.x, TL.y); } }


  public (float x, float y) TopRight
  {
    get { return (TopLeft_X + TopSideLength, TopLeft_Y); }
  }
  public (float x, float y) TR { get { return TopRight; } }
  public Vector2 TR_v { get { return new Vector2(TR.x, TR.y); } }


  public (float x, float y) BottomLeft
  {
    get
    {
      return (TopLeft_X,
              TopLeft_Y + (Y_DownSign * Height)); // Move down height
    }
  }
  public (float x, float y) BL { get { return BottomLeft; } }
  public Vector2 BL_v { get { return new Vector2(BL.x, BL.y); } }

  public (float x, float y) BottomRight
  {
    get
    {
      return (TopLeft_X + BottomSideLength,
              TopLeft_Y + (Y_DownSign * Height)); // Move down height
    }
  }
  public (float x, float y) BR { get { return BottomRight; } }
  public Vector2 BR_v { get { return new Vector2(BR.x, BR.y); } }

  public (float x, float y) Center
  {
    get
    {
      return (TopLeft_X + Width / 2,
              TopLeft_Y + (Y_DownSign * (Height / 2)) // Move down 1/2 height
      );
    }
  }
  public Vector2 Center_v { get { return new Vector2(Center.x, Center.y); } }

  #endregion

  public bool Contains(float x, float y)
  {
    bool in_x_range = TopLeft_X <= x && x <= TopLeft_X + Width;
    if (!in_x_range)
    {
      return false;
    }

    if (Y_DownSign == 1)
    {
      /*
      TL_Y = -10
      Target_Y = 2
      BR_Y = 4
            -
            |  *  -10
            |
            |
       -----+-----
            |  X    2
            |
            |  *    4
            +

      -10 <= 2 <= 4   In range
      */
      return TopLeft_Y <= y && y <= TopLeft_Y + Height;
    }
    else
    {
      /*
      TL_Y = 10
      Target_Y = -2
      BR_Y = -4
            +
            |  *  10
            |
            |
       -----+-----
            |  X  -2
            |
            |  *  -4
            -

      10 >= -2 >= -4    In range
      */
      return TopLeft_Y >= y && y >= TopLeft_Y + Height;
    }
  }
}
