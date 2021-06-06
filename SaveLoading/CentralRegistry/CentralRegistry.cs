using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The CentralRegistry is a system to allow for objects to easily lookup other object
 * using a uuid string/ name.
 *
 * Every registered uuid is in-fact, unique. If a duplicate name is trying to be
 * registered, then an error will be logged (but not thrown) and the register operation
 * fails.
 *
 */
public static class CentralRegistry
{

  private static Dictionary<string, object> centralDictionary = new Dictionary<string, object>();

  public static bool containsNocabName(string targetNocabName)
  {
    /**
     * Returns true if the provided targetNocabName is an entry in the central
     * registry. False otherwise.
     */
    return centralDictionary.ContainsKey(targetNocabName);
  }

  public static bool tryRegister(string newObjNocabName, object newObj)
  {
    /**
     * Attempts to register the provided newObject into the provided newObjNocabName slot.
     * If the provided name is not unique (in other words, if the provided name already
     * has an object associated with it) then false will be returned and an error will be
     * logged.
     */
    if (containsNocabName(newObjNocabName))
    {
      string error = "CentralRegistry is trying to register an object but it's " +
        $"NocabName has already been registered!: Name: {newObjNocabName}";
      Debug.LogError(error);
      return false;
    }

    centralDictionary.Add(newObjNocabName, newObj);
    return true;
  }

  public static bool tryDeregister(string targetNocabName)
  {
    /**
     * Attempts to remove an object from the central registry. This function should
     * be called whenever an element in the registry is "killed" or otherwise
     * deleted. The refrence to that object will be removed and the name freed up.
     *
     * Returns true if the remove operation was succesfull and the object removed.
     * False otherwise. False may imply the provided name could not be found.
     */
    if (!containsNocabName(targetNocabName))
    {
      string error = "CentralRegistry is trying to De-Register an object, but it's " +
        $"NocabName is not registered! Name: {targetNocabName}";
      Debug.LogError(error);
      return false;
    }
    return centralDictionary.Remove(targetNocabName);
  }

  public static object getObject(string nocabName)
  {
    if (!containsNocabName(nocabName)) { return null; }
    return centralDictionary[nocabName];
  }

}
