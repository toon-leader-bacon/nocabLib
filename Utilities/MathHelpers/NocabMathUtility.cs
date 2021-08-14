using UnityEngine;

public static class NocabMathUtility
{

  public static float lerp_clamp(float first, float second, float t) {
    /**
     * @brief linear interpolation between two given points. This function will
     * clamp the value t to the range 0, 1.
     * 
     * @param first the value returned when t = 0
     * @param second the value returned when t = 1
     * @param t SHOULD be in the range [0, 1] inclusive 
     */
    return NocabMathUtility.lerp_exact(first, second, Mathf.Clamp(t, 0f, 1f));
  }

  public static float lerp_fast(float first, float second, float t) {
    /**
     * @brief linear interpolation between two given floats. This method is 
     * slightly faster, but due to floating-point arithmetic inprecisions,
     * may not exactly reach second when t = 1.
     * See here for more details 
     * https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
     * 
     * @param first the value returned when t = 0
     * @param second the value returned when t = 1
     * @param t SHOULD be in the range [0, 1] inclusive 
     */
    return first +  (t * (second - first));
  }

  public static float lerp_exact(float first, float second, float t) {
    /**
     * @brief linear interpolation between two given points. 
     * 
     * @param first the value returned when t = 0
     * @param second the value returned when t = 1
     * @param t SHOULD be in the range [0, 1] inclusive 
     */
    return ((1 - t) * first) + (t * second);
  }

  public static float extractDecimals(float f)
  {
    if (f > 0) { return f - Mathf.Floor(f); }
    else { return f + Mathf.Ceil(f); }
  }

}
