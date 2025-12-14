using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using LightJson;

public class SimplexNoise : INoise
{
  /**
   * A Simplex Noise generator based off the example found here:
   * http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/?C=D;O=A
   * (Related link: http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf)
   *
   * Specifically the code in the file simplexnoise1234.cpp
   * Relevant documentation from the file below:
   * """
   * SimplexNoise1234
   * Copyright © 2003-2011, Stefan Gustavson
   *
   * Contact: stegu@itn.liu.se
   *
   * This library is public domain software, released by the author
   * into the public domain in February 2011. You may do anything
   * you like with it. You may even remove all attributions,
   * but of course I'd appreciate it if you kept my name somewhere.
   *
   * This library is distributed in the hope that it will be useful,
   * but WITHOUT ANY WARRANTY; without even the implied warranty of
   * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   * General Public License for more details.
   *
   * This implementation is "Simplex Noise" as presented by
   * Ken Perlin at a relatively obscure and not often cited course
   * session "Real-Time Shading" at Siggraph 2001 (before real
   * time shading actually took on), under the title "hardware noise".
   * The 3D function is numerically equivalent to his Java reference
   * code available in the PDF course notes, although I re-implemented
   * it from scratch to get more readable code. The 1D, 2D and 4D cases
   * were implemented from scratch by me from Ken Perlin's text.
   *
   * """ </end> Gustavson's documentation
   */

  private const float SkewConstant2d = 0.366025403f; // 0.5*(sqrt(3.0)-1.0)
  private const float UnskewConstant2d = 0.211324865f; // (3.0-Math.sqrt(3.0))/6.0

  /*
   * Note: you SHOULD NOT modify gradientPermutations.
   * Normally gradientPermutations would be a read-only field, but b/c SimplexNoise
   * is a JsonConvertable implementor, the this.fromJson(...) needs to modify
   * the fields after construction.
   */
  private List<int> gradientPermutations;

  public SimplexNoise(NocabRNG rng)
  {
    gradientPermutations = new List<int>();
    for (int i = 0; i < 512; i++)
    {
      gradientPermutations.Add(rng.generateInt(0, 255));
    }
  }

  public float noise(float xin)
  {
    /**
     * A 1D noise generator that produces values in the range [0, 1]
     */

    int floor = (int)Math.Floor(xin);
    int ceil = floor + 1;
    float upFromFloor = xin - floor;
    float downFromCeil = upFromFloor - 1.0f;

    float n0,
      n1; // Noise contributions from each node
    findContribution(upFromFloor, grad(gradientPermutations[floor & 0xff], upFromFloor), out n0);
    findContribution(downFromCeil, grad(gradientPermutations[ceil & 0xff], downFromCeil), out n1);

    // return (n0 + n1) * 0.395f to return in range [-1, 1]
    // return ((0.395f * (n0 + n1)) + 1) / 2.0f; => [0, 1] range as specified by INoise
    return (0.1975f * (n0 + n1)) + 0.5f;
  }

  public float noise(float xin, float yin)
  {
    /**
     * A 2D noise generator that produces values in the range [0, 1]
     */

    // Find target simplex cell via skewing input coords
    float skew = (xin + yin) * SkewConstant2d;
    float xSkewed = xin + skew;
    float ySkewed = yin + skew;
    int targetTriangleI = (int)Math.Floor(xSkewed);
    int targetTriangleJ = (int)Math.Floor(ySkewed);

    // Find (x0, y0) as distance from simplex cell origin
    float unskew = (targetTriangleI + targetTriangleJ) * UnskewConstant2d;
    float x0 = xin - (targetTriangleI - unskew);
    float y0 = yin - (targetTriangleJ - unskew);

    // Find target Simplex triangle
    int i1,
      j1; // offsets for middle corner of simplex triangle cell
    if (x0 > y0)
    {
      // lower triangle, XY order: (0,0) -> (1,0) -> (1,1)
      i1 = 1;
      j1 = 0;
    }
    else
    {
      // upper triangle, YX order: (0,0) -> (0,1) -> (1,1)
      i1 = 0;
      j1 = 1;
    }

    // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
    // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
    // c = (3-sqrt(3))/6

    // Middle corner offsets in (x,y) unskewed coords
    float x1 = x0 - i1 + UnskewConstant2d;
    float y1 = y0 - j1 + UnskewConstant2d;

    // Last corner offsets in (x,y) unskewed coords
    float x2 = x0 - 1.0f + (2.0f * UnskewConstant2d);
    float y2 = y0 - 1.0f + (2.0f * UnskewConstant2d);

    int i = targetTriangleI % 256;
    int j = targetTriangleJ % 256;

    float n0,
      n1,
      n2; // Noise contributions from 3 corners
    findContribution(x0, y0, gradientPermutations[i + gradientPermutations[j]], out n0);
    findContribution(x1, y1, gradientPermutations[i + i1 + gradientPermutations[j + j1]], out n1);
    findContribution(x2, y2, gradientPermutations[i + 1 + gradientPermutations[j + 1]], out n2);

    // return 40.0f * (n0 + n1 + n2) to return in range [-1, 1]
    // return ((40.0f * (n0 + n1 + n2)) + 1) / 2.0f; => [0, 1] range as specificed by INoise
    return (20.0f * (n0 + n1 + n2)) + 0.5f;
  }

  public float noise(Vector2 point)
  {
    return noise(point.X, point.Y);
  }

  #region Utility/ Helper functions

  private float grad(int hash, float x)
  {
    int h = hash & 15;
    float grad = 1.0f + (h & 7); // Gradient value 1.0, 2.0, ..., 8.0
    if ((h & 8) != 0)
    {
      grad = -grad;
    } // Set a random sign for the gradient
    return (grad * x); // Multiply the gradient with the distance
  }

  private float grad(int hash, float x, float y)
  {
    int h = hash & 7; // Convert low 3 bits of hash code
    float u = h < 4 ? x : y; // into 8 simple gradient directions,
    float v = h < 4 ? y : x; // and compute the dot product with (x,y).
    float left = (h & 1) != 0 ? -u : u;
    float right = (h & 2) != 0 ? -2.0f * v : 2.0f * v;
    return left + right;
  }

  private void findContribution(float x, float gradient, out float contribution)
  {
    float temp = 1.0f - (x * x);
    temp *= temp;
    contribution = temp * temp * gradient;
  }

  private void findContribution(float x, float y, int gradient, out float contribution)
  {
    float temp = 0.5f - (x * x) - (y * y);
    if (temp < 0.0f)
      contribution = 0.0f;
    else
    {
      temp *= temp;
      contribution = temp * temp * grad(gradient, x, y);
    }
  }

  #endregion

  #region Saving/ Loading

  public const string JsonType = "SimplexNoise"; // Version 1.0

  public string myJsonType()
  {
    return JsonType;
  }

  public JsonObject toJson()
  {
    JsonObject result = JsonUtilitiesNocab.initJson(JsonType);
    JsonArray gpAsJson = new JsonArray();
    foreach (int val in gradientPermutations)
    {
      gpAsJson.Add(val);
    }
    result["gradientPermutations"] = gpAsJson;
    return result;
  }

  public void loadJson(JsonObject jo)
  {
    JsonUtilitiesNocab.assertValidJson(jo, JsonType);
    this.gradientPermutations = new List<int>();
    foreach (int val in jo["gradientPermutations"].AsJsonArray)
    {
      this.gradientPermutations.Add(val);
    }
  }

  #endregion
}
