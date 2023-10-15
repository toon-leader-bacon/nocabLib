using LightJson;
using UnityEditor;

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

    public static HashablePointInt operator +(HashablePointInt hp1, HashablePointInt hp2)
    {
        return new HashablePointInt(hp1.x + hp2.x, hp1.y + hp2.y);
    }

    public static HashablePointInt operator -(HashablePointInt hp1, HashablePointInt hp2)
    {
        return new HashablePointInt(hp1.x - hp2.x, hp1.y - hp2.y);
    }

    #region equality and hashes
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

    public bool Equals(HashablePointInt other)
    {
        return (x == other.x) && (y == other.y);
    }

    public override bool Equals(object other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        return Equals((HashablePointInt)other);
    }

    public static bool operator ==(HashablePointInt hp1, HashablePointInt hp2)
    {
        if (ReferenceEquals(hp1, hp2))
        {
            return true;
        }
        if (ReferenceEquals(hp1, null))
        {
            return true;
        }
        if (ReferenceEquals(hp2, null))
        {
            return true;
        }
        return hp1.Equals(hp2);
    }

    public static bool operator !=(HashablePointInt hp1, HashablePointInt hp2)
    {
        return !(hp1 == hp2);
    }
    #endregion

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
