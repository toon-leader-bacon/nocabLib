using System;
using System.IO;


public static class NocabDiskUtility
{
  /**
   * A static class for reading and writing strings to the disk
   */

  static readonly string DEFAULT_SAVE_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/NocabDiskUtility";
  // IF in unity: 
  // static readonly string DEFAULT_SAVE_LOCATION = Application.persistentDataPath;



  public static void WriteStringToFile(string relativeDirectory, string fileName, string textToWrite)
  {
    string fullDirectory = cleanFileName(relativeDirectory);
    Directory.CreateDirectory(fullDirectory); // No op if it already exists
    string fileLocation = Path.Combine(fullDirectory, fileName);
    System.Console.WriteLine("Writing file to: " + fileLocation);
    File.WriteAllText(fileLocation, textToWrite);
  }

  public static string ReadStringFromFile(string relativeDirectory, string filename)
  {
    string fullDirectory = cleanFileName(relativeDirectory);
    string fileLocation = Path.Combine(fullDirectory, filename);
    return File.ReadAllText(fileLocation);
  }

  private static string cleanFileName(string relativeDirectory)
  {
    // The output of this function is the true directory location on the file system
    // Do a little cleaning and add in the Unity special persistent Data Path

    if (relativeDirectory.Length > 0)
    {
      if (relativeDirectory[0] == '\\' ||
          relativeDirectory[0] == '/')
      {
        // Strip the leading special character
        string errMsg = "Warning: Provided relativePathName contains a leading problem character. " +
          "Please remove the first character to increase efficency. Problem string: " +
          relativeDirectory; 
        System.Console.WriteLine(errMsg);
        relativeDirectory.Remove(0, 1); // Start at element 0, remove 1 character
      }
    }
    System.Console.WriteLine($"DEFAULT_SAVE_LOCATION: '{DEFAULT_SAVE_LOCATION}'");
    // Debug.Log(Application.persistentDataPath);
    return Path.Combine(DEFAULT_SAVE_LOCATION, relativeDirectory);
  }

}
