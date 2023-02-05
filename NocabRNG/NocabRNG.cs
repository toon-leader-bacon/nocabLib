using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LightJson;

public class NocabRNG : JsonConvertible
{

  public static NocabRNG newRNG {
    // Warning: generating a new RNG object is expensive and slow.
    // Recommended is to store the returned RNG object and use.
    get {
      return new NocabRNG(DateTime.UtcNow);
    }
  }

  public static NocabRNG defaultRNG {
    // NOTE: The defaultRNG uses the same seed every time. So the order
    // of generated numbers will not change between program runs.
    get {
      if (defaultRNG_ == null) { defaultRNG_ = new NocabRNG(5489); } // Default seed certified random according to Wikipedia
      return defaultRNG_;
    }
  }
  private static NocabRNG defaultRNG_ = null;


  private NocabMT myRNG;

  public string NocabName { get { return this.nocabNameable.getNocabName(); } }
  private NocabNameable nocabNameable;

  public NocabRNG(int seed) {
    this.myRNG = new NocabMT(seed);
    this.nocabNameable = new NocabNameable(this, this.generateUUID());
  }
  public NocabRNG(string seed) : this(seed.GetHashCode()) { }
  public NocabRNG(System.Object seed) : this(seed.GetHashCode()) { }
  public NocabRNG(JsonObject jo) { this.loadJson(jo); }

  #region Value extraction

  public uint generateUInt() { return this.myRNG.extract_number(); }

  public int generateInt(int low, int high, bool lowInclusive = true, bool highInclusive = true) {
    /**
     * Generates a random int within the provided range of [low, high]. The default low and high
     * values are inclusive in the range, meaning they are possible values to be returned.
     *
     * If low == high, then lowInclusive and highInclusive must both be true. Otherwise an error is thrown.
     *
     * If low + 1 == high, then at least one of the of the inclusive parameters must be true. Else error.
     */
    if (low > high) {
      string errMsg = $"Invalid range provided low={low}, high={high}. Swapping now. " +
          $"There may be some unexpected behavior relating to range inclusiveness.";
      System.Console.WriteLine(errMsg);
      int swap = low;
      low = high;
      high = swap;
    }

    // Check for invalid ranges like (4, 5)
    // IE: Pick an int between 4 and 5 that is not 4 nor 5 => invalid
    int delta = high - low;
    int lowDelta = (lowInclusive) ? 0 : 1;
    int highDelta = (highInclusive) ? 0 : 1;
    if (delta < (lowDelta + highDelta)) {
      throw new System.Exception($"Invalid random int range. Low={low}  high={high}  " +
          $"lowInclusive={lowInclusive}  highInclusive={highInclusive}");
    }
    return generateIntInternal(low + lowDelta, (high + 1) - highDelta); // high+1 because generateIntInternal is high-exclusive
  }

  private int generateIntInternal(int low, int high) {
    /**
     * Please use the safe public generateInt(...) function.
     * Generates an in in the range [low, high). That is, low inclusive, but high exclusive.
     *
     * If low == high, then that value is returned.
     *
     * Note: low > high is UNDEFINED BEHAVIOR.
     */
    
    return low + (int)Math.Floor((myRNG.extract_number() / NocabMT.MAX_POSSIBLE_VALUE_PLUS_ONE) * (high - low));
  }

  public float generateFloat(float low, float high, bool lowInclusive = true, bool highInclusive = true) {
    if (low > high) {
      string errMsg = $"Invalid range provided low={low}, high={high}. Swapping now. " +
          $"There may be some unexpected behavior relating to range inclusiveness.";
      System.Console.WriteLine(errMsg);
      float swap = low;
      low = high;
      high = swap;
    }

    float randomNumber = myRNG.extract_number();
    if(!lowInclusive && (randomNumber == 0.0f)) {
      randomNumber = float.Epsilon;
    }
    float denominator = highInclusive ? NocabMT.UNSIGNED_MAX_POSSIBLE_VALUE : // (2^32) - 1
                                        NocabMT.MAX_POSSIBLE_VALUE_PLUS_ONE;  //  2^32
    return low + ((randomNumber / denominator) * (high - low));
  }

  public double generateDouble(double low, double high, bool lowInclusive = true, bool highInclusive = false) {
    if (low > high) {
      string errMsg = $"Invalid range provided low={low}, high={high}. Swapping now. " +
          $"There may be some unexpected behavior relating to range inclusiveness.";
      System.Console.WriteLine(errMsg);
      double swap = low;
      low = high;
      high = swap;
    }
    double randomNumber = lowInclusive ? myRNG.extract_number() :
                                        (myRNG.extract_number() + 0.5d);
    double denominator = highInclusive ? NocabMT.UNSIGNED_MAX_POSSIBLE_VALUE : // (2^32) - 1
                                         NocabMT.MAX_POSSIBLE_VALUE_PLUS_ONE;  //  2^32
    return low + ((randomNumber / denominator) * (high - low));
  }

  public bool generateBool() { return (myRNG.extract_number() % 2) == 1; }

  public List<byte> generateByteList(int numberOfBytes) {
    /**
     * Returns a list of randomly generated bytes. The length of the list
     * is the absolute value of the input parameter numberOfBytes.
     */
    numberOfBytes = Math.Abs(numberOfBytes);
    List<byte> result = new List<byte>(numberOfBytes);
    int totalUints = numberOfBytes / 4; // How many full uint32 to add to result
    int remainderBytes = numberOfBytes % 4; // How many remainder bytes to add to result

    // Generate 'totalUints' number of uints, and add all those bytes to the result.
    for (int i = 0; i < totalUints; i++) {
      byte[] byteArray = BitConverter.GetBytes(this.myRNG.extract_number());
      result.AddRange(byteArray);
    }

    if (remainderBytes == 0) { return result; }

    // Add in the remainder bytes
    byte[] remainderByteArray = BitConverter.GetBytes(this.myRNG.extract_number());
    for (int i = 0; i < remainderBytes; i++) {
      byte b = remainderByteArray[i];
      result.Add(b);
    }

    return result;
  }

  public string generateUUID(int numberOfBytes = 16) {
    /**
     * The method returns a hexadecimal string. Every two characters is one byte
     * in hex code. Every byte is randomly generated. The input parameter
     * numberOfBytes represents how many bytes are used to construct the UUID.
     * Providing a numberOfBytes that is divisible by 4 is recommended.
     *
     * Example output:
     * Default 16 numberOfBytes -> "5cbb91d0f69eae22eefae1e7791fc3d5"
     * (numberOfBytes = 1) -> "2c"
     */
    StringBuilder result = new StringBuilder(numberOfBytes * 2);
    List<byte> bytes = this.generateByteList(numberOfBytes);
    foreach (byte b in bytes) { result.AppendFormat("{0:x2}", b); }
    return result.ToString();
  }

  #endregion

  #region dice
  public int d100 { get { return this.generateIntInternal(1, 101); } }
  public int d20 { get { return this.generateIntInternal(1, 21); } }
  public int d12 { get { return this.generateIntInternal(1, 13); } }
  public int d6 { get { return this.generateIntInternal(1, 7); } }
  public int d4 { get { return this.generateIntInternal(1, 5); } }
  public int rollNSidedDie(int n) { return this.generateIntInternal(1, Math.Max(1, n + 1)); }
  #endregion

  #region Between 0 and 1
  public float unitFloat { get { return this.generateFloat(0f, 1f, true, true); } }
  public double unitDouble { get { return this.generateDouble(0d, 1d, true, true); } }
  #endregion

  #region Collection Based

  public T randomElem<T>(IList<T> list) {
    // Select and return a random element in the provided list.
    // If list.Count == 0 an error is thrown.
    if (list.Count == 0) {
      throw new ArgumentException("Can NOT pull a random elem from a list with 0 elements.");
    }
    return list[randomIndex(list.Count)];
  }

  public T randomElem_Set<T>(HashSet<T> elems) {
    /**
     * @brief Select and returns a random element in the provided set.
     * This is a O(n) function, using an IList is recommended.
     * If a set of size 0 is provided an error is thrown.
     */
    if (elems.Count == 0) {
      throw new ArgumentException("Can NOT pull a random elem from a set with 0 elements.");
    }
    int randIndex = this.randomIndex(elems.Count);

    foreach (T elem in elems) {
      if (randIndex <= 0) {
        return elem;
      }
      randIndex--;
    }
    throw new Exception($"Something has gone wrong!" +
      $"RandIndex = {randIndex}, elems.Count = {elems.Count}");
  }

  public T randomElem_IEnumerator<T>(IEnumerator<T> enumerator) {
    /**
     * @brief Select and returns a random element in the provided enumerable.
     * This is a O(n) function, using an IList is recommended.
     * If a enum of size 0 is provided an error is thrown.
     */

    // Convert the enumerator into a List
    List<T> list = new List<T>();
    while (enumerator.MoveNext()) {
      list.Add(enumerator.Current);
    }

    // Validate the list
    if (list.Count == 0) {
      string errorMsg = "Can NOT pull a random elem for a collection with 0 elements!";
      throw new ArgumentException(errorMsg);
    }

    // Use the list to pull a random element.
    return randomElem<T>(list);
  }

  public int randomIndex(int count) {
    if (count <= 0) {
      throw new System.Exception($"Must provide a positive count to randomIndex(). " +
          $"Count {count}");
    }
    return this.generateIntInternal(0, count);
  }

  // TODO: Consider using % Count instead of generating a number between [0, count)
  //       It might be faster, but not sure if it's worth it.
  public int randomIndex(IList list) { return randomIndex(list.Count); }

  public void shuffleInPlace<T>(List<T> toBeShuffled) {
    /**
     * Fisher-Yates shuffle. Details found here:
     * https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
     *
     * In summary, the list will be partitioned into two parts.
     * Everything to the left of (less than) the separator is the "shuffled"
     * list. A random index will be chosen in the range [separator, count).
     * The value at the random index will be swapped with the value at the
     * separator. Then the separator will increment.
    **/
    for (int separator = 0; separator < toBeShuffled.Count; separator++) {
      int j = this.generateInt(separator, toBeShuffled.Count, true, false);

      T swapSpace = toBeShuffled[j];
      toBeShuffled[j] = toBeShuffled[separator];
      toBeShuffled[separator] = swapSpace;
    }
  }

  public IEnumerable<T> shuffleNewList<T>(List<T> shuffleInput) {
    List<T> result = new List<T>(shuffleInput);
    this.shuffleInPlace(result);
    return result;
  }

  #endregion

  #region saving

  public const string JsonType = "NocabRNG"; // version 1.0

  public string myJsonType() { return JsonType; }

  public JsonObject toJson() {
    JsonObject result = JsonUtilitiesNocab.initJson(JsonType);
    result["rng"] = this.myRNG.toJson();
    result["NocabName"] = this.nocabNameable.getNocabName();
    return result;
  }

  public void loadJson(JsonObject jo) {
    JsonUtilitiesNocab.assertValidJson(jo, JsonType);

    this.myRNG = new NocabMT(jo["rng"].AsJsonObject);
    this.nocabNameable = new NocabNameable(this, jo["NocabName"]);
  }

  #endregion
}
