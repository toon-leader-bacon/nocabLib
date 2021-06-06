using LightJson;


public struct HashablePointFloat {
	/**
	 * A helper struct that represents a point.
	 * Two points with the same (x,y) will generate the same 
	 * hash. Each hash is distinct from other (x,y) combos.
	 */
	public readonly float x;
	public readonly float y;
	public readonly float z;

	public HashablePointFloat(in float x, in float y, in float z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
	public HashablePointFloat(in float x, in float y) : this(x, y, 0) { }

	public override int GetHashCode() {
		// Based off the NocabHashUtility.generateHash(...) func
		int hash = 23;
		int prime = 31;
		hash = (prime * hash) + x.GetHashCode();
		hash = (prime * hash) + y.GetHashCode();
		hash = (prime * hash) + z.GetHashCode();
		return hash;
	}

	#region Saving/ Loading
	/**
	 * NOTICE: This _Point_ struct dose NOT inherit from JsonConvertable.
	 * because this will convert itself into a JsonArray instead of a 
	 * JsonObject.
	 */
	public JsonArray toJsonArray() {
		JsonArray result = new JsonArray();
		result.Add(x);
		result.Add(y);
		result.Add(z);
		return result;
	}

	public HashablePointFloat(JsonArray ja) {
		this.x = ja[0];
		this.y = ja[1];
		this.z = ja[2];
	}

	public string toString() {
		return $"[{this.x},{this.y},{this.z}]";
	}

	#endregion
}