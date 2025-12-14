public interface INocabNameable
{
  /*
   * Once a NocabNameable has been named, it can never change.
   * The NocabName is (read "should be") constant even through
   * save and load events.
   *
   * If a NocabNameable entity/ object "dies" or needs to be
   * destroyed the deregister() function must be called (if not,
   * then memory leaks will happen because at least the CentralRegistry
   * will hold one reference to this INocabNameable)
   *
   * Nocab Namable objects are automaticall registered in the
   * CentralRegistry at creation time.
   */

  string getNocabName();

  /**
   * Remove the reference to this NocabNamable from the central Registry.
   * Should only be called when the NocabNamable entity/ obj "dies" or
   * needs to be destroyed.
   *
   * Generally, an error may be thrown if the CentralRegistry doesn't have
   * this NocabNameable registered internally. In otherwords, this method
   * MAY not be a no-op. In general, it will return true or throw an error.
   * But other implimentations of this interface may have different behavior.
   *
   * See NocabNameable.deregister() and LazyLoader.deregister() for example.
   */
  bool deregister();
}

public class NocabNameable : INocabNameable
{
  private readonly string NocabName;

  public NocabNameable(object objectToRegister)
  {
    this.NocabName = NocabRNG.defaultRNG.generateUUID();
    this.register(objectToRegister);
  }

  public NocabNameable(object objectToRegister, string name)
  {
    this.NocabName = name;
    this.register(objectToRegister);
  }

  public bool register(object objToRegister)
  {
    if (!CentralRegistry.tryRegister(NocabName, objToRegister))
    {
      string error =
        "Attempted to register a NocabNameable to the "
        + $"CentralRegistry, but it failed. Attempted to use this name as UUID: {NocabName}";
      throw new System.ArgumentOutOfRangeException(error);
    }
    return true;
  }

  public bool deregister()
  {
    if (!CentralRegistry.tryDeregister(NocabName))
    {
      string error =
        "Attempted to de-register a NocabNameable from the "
        + $"Central Registry, but it failed. Attempted to use this name as UUID: {NocabName}";
      throw new System.ArgumentOutOfRangeException(error);
    }
    return true;
  }

  public string getNocabName()
  {
    return this.NocabName;
  }
}
