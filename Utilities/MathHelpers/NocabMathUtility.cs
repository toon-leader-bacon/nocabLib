using System;
public static class NocabMathUtility
{

  public static float clamp(float value, float min, float max) {
    if      (value < min) { return min; }
    else if (value > max) { return max; }
    else                  { return value; }
  }

  public static double clamp(double value, double min, double max) {
    if      (value < min) { return min; }
    else if (value > max) { return max; }
    else                  { return value; }
  }

  public static float lerp_clamp(float first, float second, float t) {
    /**
     * @brief linear interpolation between two given points. This function will
     * clamp the value t to the range 0, 1.
     * 
     * @param first the value returned when t = 0
     * @param second the value returned when t = 1
     * @param t SHOULD be in the range [0, 1] inclusive 
     */
    return NocabMathUtility.lerp_exact(first, second, NocabMathUtility.clamp(t, 0f, 1f));
  }

  public static float lerp_fast(float first, float second, float t) {
    /**
     * @brief linear interpolation between two given floats. This method is 
     * slightly faster, but due to floating-point arithmetic imprecisions,
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

  public static float extractDecimals(float f) {
    if (f > 0) { return f - MathF.Floor(f); }
    else { return f + MathF.Ceiling(f); }
  }

}
