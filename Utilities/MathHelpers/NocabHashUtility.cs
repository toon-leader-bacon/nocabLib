using System.Collections.Generic;

public static class NocabHashUtility
{

  public static int generateHash(ICollection<object> definingElements) {
    int hash = 23;
    int prime = 31;
    foreach (object obj in definingElements) { hash = (prime * hash) + obj.GetHashCode(); }
    return hash;
  }

  public static int generateHash<T>(ICollection<T> definingElements) where T : struct {
    /**
     * Used for when you need a hash value from a bunch of primitive types.
     * @param definingElements A Collection of primitive (or struct) types.
     */
    int hash = 23;
    int prime = 31;
    foreach (object obj in definingElements) { hash = (prime * hash) + obj.GetHashCode(); }
    return hash;
  }

#region convenience functions

  public static int generateHash(object item1, object item2) {
    int hash = 23;
    int prime = 32;
    hash = (prime * hash) + item1.GetHashCode();
    hash = (prime * hash) + item2.GetHashCode();
    return hash;
  }

  public static int generateHash(object item1, object item2, object item3) {
    int hash = 23;
    int prime = 32;
    hash = (prime * hash) + item1.GetHashCode();
    hash = (prime * hash) + item2.GetHashCode();
    hash = (prime * hash) + item3.GetHashCode();
    return hash;
  }

#endregion convenience functions

}
