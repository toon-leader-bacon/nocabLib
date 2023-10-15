using LightJson;

public struct HashablePointInt
{
    /**
     * A helper struct that represents a point.
     * Two points with the same (x,y) will generate the same
     * hash. Each hash is distinct from other (x,y) combos.
     */
    public readonly int x;
    public readonly int y;

    public HashablePointInt(in int x, in int y)
    {
        this.x = x;
        this.y = y;
    }

    public static int CalculateHashCode(int x, int y)
    {
        // Based off the NocabHashUtility.generateHash(...) func
        int hash = 23;
        int prime = 31;
        hash = (prime * hash) + x;
        hash = (prime * hash) + y;
        return hash;
    }

    public override int GetHashCode()
    {
        return CalculateHashCode(this.x, this.y);
    }

    #region Saving/ Loading
    /**
     * NOTICE: This _Point_ struct dose NOT inherit from JsonConvertable.
     * because this will convert itself into a JsonArray instead of a
     * JsonObject.
     */
    public JsonArray toJsonArray()
    {
        JsonArray result = new JsonArray();
        result.Add(x);
        result.Add(y);
        return result;
    }

    public HashablePointInt(JsonArray ja)
    {
        this.x = ja[0];
        this.y = ja[1];
    }

    public string toString()
    {
        return $"[{this.x},{this.y}]";
    }

    #endregion
}
