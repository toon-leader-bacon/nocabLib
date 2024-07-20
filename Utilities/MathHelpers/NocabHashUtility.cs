using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class NocabHashUtility
{
  /**
   * This utility is largely inspired by the System.HashCodes.cs
   * main C# library.
   * I've picked (slightly) better variable names, and taken some creative
   * liberties with certain order of operations and other simplifications.
   * 
   * For any serious use of hash codes, please consider using that library directly
   * instead. 
   * https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.Private.CoreLib/src/System/HashCode.cs
   */

  // These primes are used in the C# system library
  private const uint PRIME_1A = 2654435761U;
  private const uint PRIME_2A = 2246822519U;
  private const uint PRIME_2B = 3266489917U;
  private const uint PRIME_1B = 668265263U;
  private const uint INITIAL_PRIME = 374761393U;

  public static int generateHash(ICollection<object> definingElements)
  {
    uint hash = INITIAL_PRIME;
    bool hash_a = true;
    foreach (object obj in definingElements)
    {
      hash = hash_a ?
        internalHash_a(hash, (uint)obj.GetHashCode()) :
        internalHash_b(hash, (uint)obj.GetHashCode());
      hash_a = !hash_a;
    }
    return unchecked((int)finalHash(hash));
  }

  public static int generateHash<T>(ICollection<T> definingElements) where T : struct
  {
    /**
     * Used for when you need a hash value from a bunch of primitive types.
     * @param definingElements A Collection of primitive (or struct) types.
     */
    uint hash = INITIAL_PRIME;
    bool hash_a = true;
    foreach (object obj in definingElements)
    {
      hash = hash_a ?
        internalHash_a(hash, (uint)obj.GetHashCode()) :
        internalHash_b(hash, (uint)obj.GetHashCode());
      hash_a = !hash_a;
    }
    return unchecked((int)finalHash(hash));
  }

  public static int generateHash_Multiplicative<T>(T definingElement)
  {
    // https://en.wikipedia.org/wiki/Hash_function#Multiplicative_hashing
    int machineWordSize = 64;
    int m = 31;
    int a = 23;
    int k = definingElement.GetHashCode();
    k ^= k >> (machineWordSize - m);
    return (a * k) >> (machineWordSize - m);
  }

  #region internal tools
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint internalHash_a(uint hash, uint newValue)
  {
    return rotateLeft(hash + newValue * PRIME_2A, 13) * PRIME_1A;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint internalHash_b(uint hash, uint newValue)
  {
    return rotateLeft(hash + newValue * PRIME_2B, 17) * PRIME_1B;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint finalHash(uint hash)
  {
    hash ^= hash >> 15;
    hash *= PRIME_2A;
    hash ^= hash >> 13;
    hash *= PRIME_2B;
    hash ^= hash >> 16;
    return hash;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint rotateLeft(uint value, int n)
  {
    // NOTE: uint has a size of 32 bits
    n %= 32; // Note: I think this check is un unnecessary

    uint n_first_bits = value >> (32 - n); // Grab the left most n bits
    // Bitshift the initial value left n bits, then 'add' (bitwise or) the 
    // top n bits onto the vacated n bits at the right end.
    return (value << n) | n_first_bits;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint rotateRight(uint value, int n)
  {
    // NOTE: uint has a size of 32 bits
    n %= 32; // Note: I think this check is un unnecessary

    // Take the bottom n bits and make them the top n bits
    uint new_top_n_bits = value << (32 - n);
    // Bitshift the initial value right n bits, then 'add' (bitwise or) the
    // previous bottom n bits into the vacated n bits at the left end of the
    // result.
    return (value >> n) | new_top_n_bits;
  }
  #endregion

  #region convenience functions

  public static int generateHash(object item1, object item2)
  {
    uint hash = INITIAL_PRIME;
    hash = internalHash_a(hash, (uint)item1.GetHashCode());
    hash = internalHash_b(hash, (uint)item2.GetHashCode());
    return unchecked((int)finalHash(hash));
  }

  public static int generateHash(object item1, object item2, object item3)
  {
    uint hash = INITIAL_PRIME;
    hash = internalHash_a(hash, (uint)item1.GetHashCode());
    hash = internalHash_b(hash, (uint)item2.GetHashCode());
    hash = internalHash_a(hash, (uint)item3.GetHashCode());
    return unchecked((int)finalHash(hash));
  }

  #endregion convenience functions

}
