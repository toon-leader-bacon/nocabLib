using System;
using System.Collections.Generic;

public struct Wiggler
{
  /**
	 * A utility object that helps to "wiggle" a series of number to make them slightly more
	 * random.
	 * Providing the length of the series will ensure that the sum of the number series doesn't
	 * change. Consier the simple example list of number [5,5,5,5]. The total sum is 20, and
	 * after wiggling it produces the series [3, 6, 7, 4] the total sum is still 20.
	 *
	 * If the provided lengthOfSeries is less than or equal to zero then there is no guarantee
	 * that the sum will remain constant. The sum will stay close to the original sum, but
	 * no guarantee that it will re-converge to the exact sum.
	 */

  public readonly int lengthOfSeries; // How many numbers are in the series?
  public readonly int maxDelta;       // The maximum distance a number can be wiggled

  private int needle;         // The current delta of the series
  private int returnedCount;  // How many numbers have been returned so far

  public Wiggler(int lengthOfSeries, int maxDelta)
  {
    /**
		 * Builds a utility that can statefully wiggle a list of numbers.
		 * Consider using the Wiggler.wiggle(...) static function.
		 *
		 * Look at the class documentation above for examples.
		 *
		 * param lengthOfSeries: How many numbers are expected to be passed into this wiggler.
		 * param maxDelta: How big the change of to a single number can be
		 */
    this.lengthOfSeries = lengthOfSeries;
    this.maxDelta = maxDelta;

    this.needle = 0;
    this.returnedCount = 0;
  }

  public int wiggle(int value, NocabRNG rng)
  {
    /**
		 * A statefull version of the other wiggle function.
		 *
		 * Wiggles a list of numbers. The sum of the number will not change.
		 * The maximum change that any one number can have is maxDelta.
		 *
		 * NOTE: because the sum doesn't change, if the list only have 1 element in it than
		 * there will be no change.
		 *
		 * For future developers poking in the code, the idea is this:
		 * Constantly keep track of how much you're changing the sum so far. Ensure this
		 * change never becomes larger than maxDelta. The last number in the list will
		 * be modified by subtracting this change so far to ensure the total delta is zero.
		 */

    if (this.returnedCount == lengthOfSeries - 1)
    {
      this.returnedCount = 0;
      return value - needle;
    }

    int minRange = Math.Max(-maxDelta, -maxDelta - needle);
    int maxRange = Math.Min(maxDelta, maxDelta - needle);
    int wiggle = rng.generateInt(minRange, maxRange);
    needle += wiggle;
    returnedCount++;
    return value + wiggle;
  }

  public static void wiggle(ref List<int> numbers, int maxDelta, NocabRNG rng)
  {
    /**
		 * A static function version of the other wiggle function.
		 *
		 * Wiggles a list of numbers. The sum of the number will not change.
		 * The maximum change that any one number can have is maxDelta.
		 *
		 * NOTE: because the sum doesn't change, if the list only have 1 element in it than
		 * there will be no change.
		 *
		 * For future developers poking in the code, the idea is this:
		 * Constantly keep track of how much you're changing the sum so far. Ensure this
		 * change never becomes larger than maxDelta. The last number in the list will
		 * be modified by subtracting this change so far to ensure the total delta is zero.
		 */
    int _needle = 0;
    for (int i = 0; i < numbers.Count - 1; i++)
    {
      // iterate through all the numbers except the last one

      int minRange = Math.Max(-maxDelta, -maxDelta - _needle);
      int maxRange = Math.Min(maxDelta, maxDelta - _needle);
      int wiggle = rng.generateInt(minRange, maxRange);
      _needle += wiggle;
      // Debug.Log($"needle {_needle},  range [{minRange},{maxRange}],  wiggle {wiggle}\n");
      numbers[i] = numbers[i] + wiggle;
    }
    numbers[numbers.Count - 1] += -_needle;
  }
}
