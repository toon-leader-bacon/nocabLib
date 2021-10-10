using LightJson;

public class JsonOrigamist
{
  /**
   * An Origamist is a person who preforms the art of origami.
   *
   * In effect, think of this class like a StringBuilder; Load it
   * up with JsonObjects, then when you're ready have it write
   * everything to disk all at once.
   *
   * This class unfolds JsonObjects into strings and writes them to
   * disk, then later reads the strings and folds them back into
   * their unique JsonObject representations.
   *
   */

  private string relativeDirectory;
  private string fileName;
  private JsonArray toBeFolded;

  public JsonOrigamist(string relativeDirectory, string fileName)
  {
    this.relativeDirectory = relativeDirectory;
    this.fileName = fileName;
    this.toBeFolded = new JsonArray();
  }


  #region Write to Disk
  public void add(JsonObject jo)
  {
    toBeFolded.Add(jo);
  }

  public void writeToDisk()
  {
    string jsonStr = toBeFolded.ToString(true);
    System.Console.WriteLine("WriteToDisk: " + jsonStr);
    NocabDiskUtility.WriteStringToFile(this.relativeDirectory, this.fileName, jsonStr);
  }
  #endregion


  #region Read from Disk
  public static JsonValue readFromDiskStatic(string relativeDirectory, string fileName)
  {
    string jsonStr = NocabDiskUtility.ReadStringFromFile(relativeDirectory, fileName);
    System.Console.WriteLine("ReadFromDisk: " + jsonStr);
    return JsonValue.Parse(jsonStr);
  }

  public JsonArray readFromDisk()
  {
    return readFromDiskStatic(this.relativeDirectory, this.fileName);
  }
  #endregion

}
