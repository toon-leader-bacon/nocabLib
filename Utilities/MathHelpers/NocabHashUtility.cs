using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
