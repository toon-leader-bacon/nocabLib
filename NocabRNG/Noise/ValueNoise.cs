﻿using System;
using System.Collections.Generic;
using LightJson;
using System.Numerics;

public class ValueNoise : INoise
{
  /**
	 * Value Noise is a type of noise generated by considering random values
	 * at fixed coordinates apart. This is a 2D implimentation (with 1D simplification).
	 * 
	 * To extract the noise value at a given point, use the function
	 * ValueNoise.noise(x, y). When extracting the next value, you SHOULD 
	 * increment or decrement the (x, y) point by less than 1 unit for each
	 * dimention. For example (x + 0.3, y - 0.1). Incrementing by more than 
	 * 1 will loose the effect of noise and start to become disjointed random
	 * elements being generated.
	 * 
	 * 
	 * TODO: currently noise is being lerped, but consider weighted averages?
	 * TODO: the lerp ratio is incremented linearly, but perhapse a n^2 ratio would be better?
	 */


  Dictionary<HashablePointInt, float> nodes; // A store of the psudo random values at each point
                                             // The float value returned is between [0,1]
  private NocabRNG rng; // Used to generate new values for the nodes dictionary.

  private struct Corners
  {
    /**
		 * A container for housing the psudo-random node values
		 * of a given cell.
		 */
    public readonly float topLeft;
    public readonly float topRight;
    public readonly float bottomLeft;
    public readonly float bottomRight;

    public Corners(float topLeft, float topRight,
                 float bottomLeft, float bottomRight)
    {
      this.topLeft = topLeft;
      this.topRight = topRight;
      this.bottomLeft = bottomLeft;
      this.bottomRight = bottomRight;
    }
  }

  public ValueNoise(NocabRNG rng)
  {
    this.rng = rng;
    this.nodes = new Dictionary<HashablePointInt, float>();
  }
  public ValueNoise() : this(NocabRNG.defaultRNG) { }

  public float noise(float x) {
    Corners cell = extractCorners(x, 0.0f);

    // The coord (-0.1, y) needs to be clamped into the Lerp range of [0,1]
    // Additionally, the x = -0.1 is on the far right of the cell => it should actually be 0.9
    float clampedXFloat = (x > 0) ? (x - MathF.Floor(x)) : (x - MathF.Ceiling(x));
    float result = NocabMathUtility.lerp_exact(cell.topLeft, cell.topRight, clampedXFloat);
    return result;
  }

  public float noise(float x, float y)
  {
    /**
		 * Determine the value of the noise at the given x, y position.
		 * Produces numbers in range [0, -1]
		 */
    Corners cell = extractCorners(x, y);

    // The coord (-0.1, y) needs to be clamped into the Lerp range of [0,1]
    // Additionally, the x = -0.1 is on the far right of the cell => it should actually be 0.9
    float clampedXFloat = (x > 0) ? (x - MathF.Floor(x)) : (x - MathF.Ceiling(x));
    float clampedYFloat = (y > 0) ? (y - MathF.Floor(y)) : (y - MathF.Ceiling(y));

    float topRep = NocabMathUtility.lerp_exact(cell.topLeft, cell.topRight, clampedXFloat);
    float botRep = NocabMathUtility.lerp_exact(cell.bottomLeft, cell.bottomRight, clampedXFloat);
    float result = NocabMathUtility.lerp_exact(topRep, botRep, clampedYFloat);
    return result;
  }

  public float noise(Vector2 point) { return this.noise(point.X, point.Y); }


  #region Private Utility

  private Corners extractCorners(float x, float y)
  {
    // Finds the closest 4 nodes to the given point and packages them in the Corners struct
    return new Corners(
      topLeft:     extractNode(new HashablePointInt((int)MathF.Floor(x),   (int)MathF.Floor(y))),
      topRight:    extractNode(new HashablePointInt((int)MathF.Ceiling(x), (int)MathF.Floor(y))),
      bottomLeft:  extractNode(new HashablePointInt((int)MathF.Floor(x),   (int)MathF.Ceiling(y))),
      bottomRight: extractNode(new HashablePointInt((int)MathF.Ceiling(x), (int)MathF.Ceiling(y)))
    );
  }

  private float extractNode(in HashablePointInt point)
  {
    /**
		 * Extract the psudo random unit float at the given position.
		 * If the position has not been initilized yet, then generate the
		 * calue using this.rng.
		 */
    if (!this.nodes.ContainsKey(point))
    {
      float value = rng.unitFloat;
      this.nodes[point] = value;
    }
    return this.nodes[point];
  }

  #endregion

  #region Saving

  public const string JsonType = "ValueNoise"; // version 1.0
  public string myJsonType() { return JsonType; }

  public JsonObject toJson()
  {
    JsonObject result = JsonUtilitiesNocab.initJson(JsonType);
    result["NocabRNG"] = this.rng.toJson();

    JsonArray nodesAsJson = new JsonArray();
    foreach (KeyValuePair<HashablePointInt, float> kvp in this.nodes)
    {
      JsonArray kvpAsJson = new JsonArray();
      kvpAsJson[0] = kvp.Key.toJsonArray();
      kvpAsJson[1] = kvp.Value;
    }
    result["nodes"] = nodesAsJson;

    return result;
  }

  public void loadJson(JsonObject jo)
  {
    JsonUtilitiesNocab.assertValidJson(jo, JsonType);
    this.rng = new NocabRNG(jo["NocabRNG"].AsJsonObject);

    this.nodes = new Dictionary<HashablePointInt, float>();
    JsonArray nodesAsJson = jo["nodes"].AsJsonArray;
    foreach (JsonArray kvpAsJson in nodesAsJson)
    {
      HashablePointInt key = new HashablePointInt(kvpAsJson[0].AsJsonArray);
      float value = kvpAsJson[1];
      this.nodes[key] = value;
    }
  }


  #endregion

}
