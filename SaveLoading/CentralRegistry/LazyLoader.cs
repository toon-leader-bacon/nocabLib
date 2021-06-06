using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyLoader<T> : INocabNameable
{
  /**
   * A LazyLoader is a wrapper class for a NocabNameable object.
   * In short, a LazyLoader holds a NocabName uuid, and uses it
   * to look up a reference in the CentralRegistry at a future time.
   *
   * Some object save a NocabName uuid to disk (via json or otherwise).
   * When an object like this is loaded, it needs to convert the NocabName uuid
   * string back into some kind of object. A load process may be complicated,
   * and in this situation order of events are important. If the NocabName uuid
   * hasn't been registered in the CentralRegistry yet, then the load process
   * will fail for the object trying to find the UUID.
   *
   * This LazyLoader class is one way to solve that issue. Instead of loading the
   * target object as soon as possible, a LazyLoader will ping the CentralRegistry
   * at the last possible second.
  **/

  private readonly string NocabNameUUID;

  // The target. This will be populated after a call to getObjectOrError()
  private T targetObj = default(T); // Use this.setTargetObj(...) to modify!
  private bool targetAcquired = false; // Has this.targetObj been loaded

  public LazyLoader(string NocabNameUUID)
  {
    this.NocabNameUUID = NocabNameUUID;
  }

  public bool isTargetRegistered()
  {
    /**
     * Check if the NocabNameable this LazyLoader targets is loaded in the CentralRegistry.
    **/
    return CentralRegistry.containsNocabName(this.NocabNameUUID);
  }

  public T getObjectOrError()
  {
    /**
     * Ping the CentralRegistry and attempt to get the target object.
     * Errors will be thrown if:
     *  - this.NocabNameUUID is NOT in the CentralRegistry
     *  - The object returned from the CentralRegistry is not of type T
     *
     * "Happy Path" behavior: Ping the CentralRegistry, get an object, sucesfully
     * cast it to type T, save that refrence for the future and return it.
    **/
    if (this.targetAcquired) { return this.targetObj; }

    if (!isTargetRegistered())
    {
      string errorMsg = "LazyLoader attempted to pull an object out of the CentralRegistry. " +
      $"But the target didn't exist! Target NocabNameUUID: \"{NocabNameUUID}\"";
      Debug.LogError(errorMsg);
      throw new System.ArgumentOutOfRangeException(errorMsg);
    }

    object objFromCR = CentralRegistry.getObject(this.NocabNameUUID);
    return this.setTargetObj(objFromCR); // Will error if objFromCR is not type T
  }

  private T setTargetObj(object obj)
  {
    /**
     * Internal helper function to safely cast and save results from the CentralRegistry.
    **/
    // Someone could register a null to CentralRegistry. That's valid, but weird...
    if (obj == null) { this.targetObj = default(T); } // Default of refence types is null
    else
    {
      // Else, process the results from the CentralRegistry.
      // Ensure it's the target type T.
      try { this.targetObj = (T)obj; }
      catch (System.InvalidCastException e)
      {
        string errorMsg = "LazyLoder attempted to cast the result target objcet " +
        "(Received from the CentralRegistry) into the target template type. NocabNameUUID: " +
        $"\"{this.NocabNameUUID}\". InvalidCastException: {e.Message}";
        Debug.LogError(errorMsg);
        throw new System.InvalidCastException(errorMsg, e);
      }
    }

    this.targetAcquired = true;
    return this.targetObj;
  }

  public string getNocabName()
  {
    return this.NocabNameUUID;
  }

  public bool deregister()
  {
    if (!CentralRegistry.tryDeregister(NocabNameUUID))
    {
      string error = "Attempted to de-register a NocabNameable from the " +
        $"Central Registry, but it failed. Attempted to use this name as UUID: {NocabNameUUID}";
      throw new System.ArgumentOutOfRangeException(error);
    }
    return true;
  }

}
