using System;
using LightJson;

public class NocabMT : JsonConvertible
{
  /**
   * Mersenne Twister, following the pseudo code from Wikipedia.
   * https://en.wikipedia.org/wiki/Mersenne_Twister
   *
   * Generates an unsigned int (32 bit) in the range between [0, (2^32) - 1] Or [0, 4,294,967,295]
   */

  public const uint UNSIGNED_MAX_POSSIBLE_VALUE = 4294967295; // (2^32) - 1
  public const float MAX_POSSIBLE_VALUE_PLUS_ONE = 4294967296.0f; // (2^32). Float is required because uint overflow

  private const int wordLength = 32; // number of bits
  private const uint stateLength = 624; // degree of recurrence
  private const uint m = 397; // middle word, an offset used in defining the series X.  1 <= m < n
  private const uint r = 31; //separation  point of one word, # bits of lower bitmask.  0 <= r <= w-1
  private const uint a = 2567483615; // Coefficients of the rational normal form twist matrix

  private const int u = 11; // Tempering bit shift

  //private const uint d = 0xFFFFFFFF; // Un-used for 32 bit generation
  private const int s = 7; // Tempering bitshift
  private const uint b = 0x9D2C5680; // Tempering mask
  private const int t = 15; // Tempering bitshift
  private const uint c = 0xEFC60000; // Tempering mask
  private const int l = 18; // Tempering bitshift

  private const uint F32 = 1812433253;
  private const uint UPPER_MASK = 0b_1000_0000_0000_0000_0000_0000_0000_0000; // Most significant w-r bits
  private const uint LOWER_MASK = 0b_0111_1111_1111_1111_1111_1111_1111_1111; // least significant r bits

  // Create a length n array to store the generator state
  UInt32[] mtState;
  uint index;

  public NocabMT(uint seed)
  {
    this.mtState = new uint[stateLength];
    index = stateLength;
    initializeState(seed);
  }

  public NocabMT()
    : this(5489) { }

  public NocabMT(int seed)
    : this((uint)seed) { }

  public NocabMT(object seed)
    : this(seed.GetHashCode()) { }

  public NocabMT(JsonObject jo)
  {
    this.loadJson(jo);
  } // LOADING ONLY.

  // Initialize the generator from a seed
  private void initializeState(uint seed)
  {
    mtState[0] = seed;
    for (int i = 1; i < stateLength - 1; i++)
    {
      mtState[i] = (uint)(F32 * (mtState[i - 1] ^ (mtState[i - 1] >> (wordLength - 2))) + i);
    }
  }

  // Extract a tempered value based on mtState[index]
  // calling twist() every n numbers
  public UInt32 extract_number()
  {
    if (index >= stateLength)
    {
      twist();
    }

    uint y = mtState[index++];
    /* Tempering */
    y ^= (y >> u);
    y ^= ((y << s) & b);
    y ^= ((y << t) & c);
    y ^= (y >> l);

    return y;
  }

  private void twist()
  {
    for (int i = 0; i < stateLength; i++)
    {
      uint x = (mtState[i] & UPPER_MASK) + (mtState[(i + 1) % stateLength] & LOWER_MASK);
      uint xA = x >> 1;
      if ((x % 2) == 1)
      {
        // Lowest bit of x is 1
        xA = (uint)(xA ^ a);
      }
      mtState[i] = mtState[(i + m) % stateLength] ^ xA;
    }
    index = 0;
  }

  #region Saving

  private const string JsonType = "NocabMT"; // Version 1.0

  public string myJsonType()
  {
    return JsonType;
  }

  public JsonObject toJson()
  {
    JsonObject result = JsonUtilitiesNocab.initJson(JsonType);
    result["index"] = index;
    JsonArray ja = new JsonArray();
    for (int i = 0; i < stateLength; i++)
    {
      ja.Add(mtState[i]);
    }
    result["mt"] = ja;
    return result;
  }

  public void loadJson(JsonObject jo)
  {
    JsonUtilitiesNocab.assertValidJson(jo, JsonType);
    this.index = (uint)jo["index"];

    JsonArray ja = jo["mt"];
    mtState = new uint[ja.Count];
    for (int i = 0; i < ja.Count; i++)
    {
      mtState[i] = (uint)ja[i];
    }
  }

  #endregion
}
