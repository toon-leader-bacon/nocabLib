using System.Numerics;

public interface INoise : JsonConvertible
{
  /**
   * Generate 1d noise in the range [0,1] inclusive on both sides
   * NOTE: Because this is noise, it is extremely rare/ hard to get close
   * to exactly 0 or exactly 1. Expect only approaching +- 0.05 of min/ max.
   */
  float noise(float x);

  /**
   * Generate 2d noise in the range [0, 1] inclusive on both sides
   * NOTE: Because this is noise, it is extremely rare/ hard to get close
   * to exactly 0 or exactly 1. Expect only approaching +- 0.05 of min/ max.
   */
  float noise(float x, float y);

  /**
   * A required helper function that will simply call noise(float x, float y)
   */
  float noise(Vector2 point);
}
