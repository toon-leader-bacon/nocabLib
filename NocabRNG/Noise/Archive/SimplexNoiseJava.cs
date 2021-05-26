using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexNoiseJava {
	/**
	 * A C# implimentation of the 2d noise described here:
	 * https://weber.itn.liu.se/~stegu/simplexnoise/
	 * Origionally authored by Stefan Gustavson and Peter Eastman.
	 * It origionally (and this file also) is in the public domain.
	 * 
	 * C# implimentation by Arthur Bacon of Nocab Software.
	 */

	private const float SkewFactor2d =   0.366025403784438646f; //0.5f * (Mathf.Sqrt(3.0f) - 1.0f)
	private const float UnskewFactor2d = 0.211324865405187117f; // (3 - Mathf.sqrt(3.0f)) / 6.0;

	private List<Vector3Int> gradients;

	private List<int> perm;
	private List<int> permMod12;

	public SimplexNoiseJava(NocabRNG rng) {
		perm = new List<int>();
		permMod12 = new List<int>();
		for (int i = 0; i < 512; i++) {
			int permValue = rng.generateInt(0, 255);
			perm.Add(permValue);
			permMod12.Add(permValue % 12);
		}

		gradients = new List<Vector3Int>();
		for (int i = 0; i < 12; i++) {
			int x = rng.generateInt(-1, 1, true, true);
			int y = rng.generateInt(-1, 1, true, true);
			int z = rng.generateInt(-1, 1, true, true);
			gradients.Add(new Vector3Int(x, y, z));
		}
	}

	private static float dot(Vector3Int grad, float x, float y) 
			{ return (grad.x * x) + (grad.y * y); }

	public float noise2d(float xin, float yin) {
		/**
		 * Produces floats in the range [-1, 1]
		 */

		float n0, n1, n2;

		// Determine which simplex cell point is in
		float skew = (xin + yin) * SkewFactor2d;
		int cellX = Mathf.FloorToInt(xin + skew); // i
		int cellY = Mathf.FloorToInt(yin + skew); // j

		float unskew = (cellX + cellY) * UnskewFactor2d;
		float cellXUnskew = cellX - unskew;
		float cellYUnskew = cellY - unskew;
		float distFromCellOriginX = xin - cellXUnskew; // x0
		float distFromCellOriginY = yin - cellYUnskew; // y0

		int middleOffsetI, middleOffsetJ;
		if (distFromCellOriginX > distFromCellOriginY) {
			middleOffsetI = 1;
			middleOffsetJ = 0;
		}
		else {
			middleOffsetI = 0;
			middleOffsetJ = 1;
		}

		float x1 = distFromCellOriginX - middleOffsetI + UnskewFactor2d;
		float y1 = distFromCellOriginY - middleOffsetJ + UnskewFactor2d;
		float x2 = distFromCellOriginX - 1.0f + (2.0f * UnskewFactor2d);
		float y2 = distFromCellOriginY - 1.0f + (2.0f * UnskewFactor2d);

		int i = cellX & 255;
		int j = cellY & 255;
		int gradientIndex0 = permMod12[i + perm[j]];
		int gradientIndex1 = permMod12[i + middleOffsetI + perm[j + middleOffsetJ]];
		int gradientIndex2 = permMod12[i + 1 + perm[j + 1]];

		// Calculate contributions for each corner
		float t0 = 0.5f -
				  (distFromCellOriginX * distFromCellOriginX) -
				  (distFromCellOriginY * distFromCellOriginY);
		if (t0 < 0) { n0 = 0.0f; }
		else {
			t0 *= t0;
			n0 = t0 * t0 * dot(gradients[gradientIndex0], distFromCellOriginX, distFromCellOriginY);
		}

		float t1 = 0.5f - (x1 * x1) - (y1 * y1);
		if (t1 < 0) { n1 = 0.0f; }
		else {
			t1 *= t1;
			n1 = t1 * t1 * dot(gradients[gradientIndex1], x1, y1);
		}

		float t2 = 0.5f - (x2 * x2) - (y2 * y2);
		if (t2 < 0) { n2 = 0.0f; }
		else {
			t2 *= t2;
			n2 = t2 * t2 * dot(gradients[gradientIndex2], x2, y2);
		}

		return 70f * (n0 + n1 + n2);
	}






}
