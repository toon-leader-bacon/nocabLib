using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTable<T>
{
  /**
   * The DropTable<T> class encapsulates a series of possible options. One option can be  selected
   * at random from the list of options. Each option in the list has a variable probablity of being
   * selected.
   */

  private NocabRNG rng;
  private List<KeyValuePair<float, T>> options;
  private float totalSum;

  public DropTable(NocabRNG rng, List<KeyValuePair<float, T>> options)
  {
    /**
     * TODO: Come up with a better variable name than "options"
     * TODO: Build some AddElement options if needed. Consider queueing up changes and normalizing
     *       the options list in a Lazy design pattern.
     */

    this.rng = rng;
    this.options = options;
    this.totalSum = calculateSum(options);
  }


  #region Pull element

  public T PullElement()
  {
    return this.pullElement(this.rng.generateFloat(0f, totalSum, true, false));
  }

  private T pullElement(float f)
  {
    /**
     * Given a value between [0, totalSum) zero inclusinve totalSum exclusive, return the
     * associated element.
     */
    foreach (KeyValuePair<float, T> kvp in options)
    {
      float percentChance = kvp.Key;
      f -= percentChance;
      if (f < 0) { return kvp.Value; }
    }
    int randomIndex = rng.randomIndex(this.options.Count);
    return this.options[randomIndex].Value;
  }

  #endregion

  #region Utilites

  private static float calculateSum(List<KeyValuePair<float, T>> options)
  {
    // Calculate the sum of all the keys in the provided list
    float result = 0.0f;
    foreach (KeyValuePair<float, T> option in options)
    {
      result += Mathf.Abs(option.Key);
    }
    return result;
  }

  #endregion
}
