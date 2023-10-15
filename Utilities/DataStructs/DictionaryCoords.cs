using System.Collections.Generic;
using UnityEngine;

public class DictionaryCoords<TValue> : Dictionary<int, TValue>
{
    #region Calculate Hash Code
    public static int CalculateHashCode(int x, int y)
    {
        return HashablePointInt.CalculateHashCode(x, y);
    }

    public static int CalculateHashCode(Vector2Int coord)
    {
        return CalculateHashCode(coord.x, coord.y);
    }

    public static int CalculateHashCode(HashablePointInt coord)
    {
        return CalculateHashCode(coord.x, coord.y);
    }
    #endregion

    #region Set
    public void Set(int x, int y, TValue value)
    {
        this[CalculateHashCode(x, y)] = value;
    }

    public void Set(Vector2Int coord, TValue value)
    {
        this.Set(coord.x, coord.y, value);
    }

    public void Set(HashablePointInt coord, TValue value)
    {
        this.Set(coord.x, coord.y, value);
    }
    #endregion

    #region Get
    public TValue Get(int x, int y)
    {
        return this[CalculateHashCode(x, y)];
    }

    public TValue Get(Vector2Int coord)
    {
        return this.Get(coord.x, coord.y);
    }

    public TValue Get(HashablePointInt coord)
    {
        return this.Get(coord.x, coord.y);
    }
    #endregion

    #region [ ] array accessors
    public TValue this[int x, int y]
    {
        get { return this.Get(x, y); }
        set { this.Set(x, y, value); }
    }

    public TValue this[Vector2Int key]
    {
        get { return this.Get(key); }
        set { this.Set(key, value); }
    }
    public TValue this[HashablePointInt key]
    {
        get { return this.Get(key); }
        set { this.Set(key, value); }
    }
    #endregion

    #region Contains Key
    public bool ContainsKey(int x, int y)
    {
        return ContainsKey(CalculateHashCode(x, y));
    }

    public bool ContainsKey(Vector2Int coord)
    {
        return ContainsKey(CalculateHashCode(coord));
    }

    public bool ContainsKey(HashablePointInt coord)
    {
        return ContainsKey(CalculateHashCode(coord));
    }
    #endregion
}
