using LightJson;


public struct HashablePointFloat
{
  /**
	 * A helper struct that represents a point.
	 * Two points with the same (x,y) will generate the same 
	 * hash. Each hash is distinct from other (x,y) combos.
	 */
  public readonly float x;
  public readonly float y;
  public readonly float z;

  public HashablePointFloat(in float x, in float y, in float z)
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }
  public HashablePointFloat(in float x, in float y) : this(x, y, 0) { }

  public override int GetHashCode()
  {
    return NocabHashUtility.generateHash(this.x, this.y, this.z);
  }

  #region Saving/ Loading
  /**
	 * NOTICE: This _Point_ struct dose NOT inherit from JsonConvertable.
	 * because this will convert itself into a JsonArray instead of a 
	 * JsonObject.
	 */
  public JsonArray toJsonArray()
  {
    JsonArray result = new JsonArray { x, y, z };
    return result;
  }

  public HashablePointFloat(JsonArray ja)
  {
    this.x = ja[0];
    this.y = ja[1];
    this.z = ja[2];
  }

  public string toString()
  {
    return $"[{this.x},{this.y},{this.z}]";
  }

  #endregion
}