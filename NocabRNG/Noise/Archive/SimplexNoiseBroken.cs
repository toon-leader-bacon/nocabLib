// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SimplexNoiseBroken {
// 	/**
// 	 * WARNING: Something's wrong with this implimentation. Namely, there are 
// 	 * major edge artifacts. Please use another implimentation.
// 	 * 
// 	 * Simplex noise generator bassed off the OpenSimplex 
// 	 * project:
// 	 * https://github.com/lmas/opensimplex/blob/master/opensimplex/opensimplex.py
// 	 */

// 	private const float STRETCH_CONSTANT_2D = -0.211324865405187f; // ((1/Math.sqrt(2+1)) - 1) / 2
// 	private const float SQUISH_CONSTANT_2D =   0.366025403784439f; // (Math.sqrt(2+1) - 1) / 2
// 	private const int NORM_CONSTANT_2D = 47;

// 	private readonly int[] GRADIENTS_2D = {
// 		 5,  2,    2,  5,
// 		-5,  2,   -2,  5,
// 		 5, -2,    2, -5,
// 		-5, -2,   -2, -5,
// 	};

// 	private const int DEFAULT_SEED = 861473;

// 	private List<int> perm;

// 	public SimplexNoiseBroken(NocabRNG rng) {
// 		this.perm = new List<int>();
// 		for (int i = 0; i < 256; i++) {
// 			this.perm.Add((int)rng.generateUInt());
// 		}
// 	}

// 	public float noise2d(float x, float y) {
// 		// Place input coords onto grid.
// 		float stretchOffset = (x + y) * STRETCH_CONSTANT_2D;
// 		float xStretch = x + stretchOffset;
// 		float yStretch = y + stretchOffset;

// 		// Floor to get grid coords of rhombus (stretched square) super-cell origin.
// 		int xsb = Mathf.FloorToInt(xStretch);
// 		int ysb = Mathf.FloorToInt(yStretch);

// 		// skew out to get actual coords of rhombus origin. Used later.
// 		float squishOffset = (xsb + ysb) * SQUISH_CONSTANT_2D;
// 		float xb = xsb + squishOffset;
// 		float yb = ysb + squishOffset;

// 		// Compute grid coordinates relative to rhombus origin.
// 		float xins = xsb - xsb;
// 		float yins = ysb - ysb;

// 		// Sum coordinates to determine which region
// 		float inSum = xins + yins;

// 		// Positions relative to origin point.
// 		float dx0 = x - xb;
// 		float dy0 = y - yb;

// 		float value = 0f;

// 		// Contribution (1,0)
// 		float dx1 = dx0 - 1 - SQUISH_CONSTANT_2D;
// 		float dy1 = dy0 - 0 - SQUISH_CONSTANT_2D;
// 		float attn1 = 2 - (dx1 * dx1) - (dy1 * dy1);
// 		if (attn1 > 0) {
// 			attn1 *= attn1;
// 			value += attn1 * attn1 * extrapolate2D(xsb + 1, ysb + 0, dx1, dy1);
// 		}

// 		// Contribution (0, 1)
// 		float dx2 = dx0 - 0 - SQUISH_CONSTANT_2D;
// 		float dy2 = dy0 - 1 - SQUISH_CONSTANT_2D;
// 		float attn2 = 2 - (dx2 * dx2) - (dy2 * dy2);
// 		if (attn2 > 0) {
// 			attn2 *= attn2;
// 			value += attn2 * attn2 * extrapolate2D(xsb + 0, ysb + 1, dx2, dy2);
// 		}

// 		int xsv_ext;
// 		int ysv_ext;
// 		float dx_ext;
// 		float dy_ext;

// 		if (inSum <= 1) {
// 			// If inside the triangle (2-simplex) at (0,0)
// 			float zins = 1 - inSum;
// 			if ((zins > xins) || (zins > yins)) {
// 				// (0,0) is one of the closest two triangular vertices
// 				if (xins > yins) {
// 					xsv_ext = xsb + 1;
// 					ysv_ext = ysb - 1;
// 					dx_ext  = dx0 - 1;
// 					dy_ext  = dy0 + 1;
// 				} else {
// 					xsv_ext = xsb - 1;
// 					ysv_ext = ysb + 1;
// 					dx_ext  = dx0 + 1;
// 					dy_ext  = dy0 - 1;
// 				}
// 			} else {
// 				// (1,0) and (0,1) are the closest two vertices
// 				xsv_ext = xsb + 1;
// 				ysv_ext = ysb + 1;
// 				dx_ext  = dx0 - 1 - 2 * (SQUISH_CONSTANT_2D);
// 				dy_ext  = dy0 - 1 - 2 * (SQUISH_CONSTANT_2D);
// 			}
// 		} else {
// 			// Inside the triangle (2-simplex) at (1,1)
// 			float zins = 2 - inSum;
// 			if ((zins < xins) || (zins < yins)) {
// 				// (0,0) is one of the closest tow trianglar vertices
// 				if (xins > yins) {
// 					xsv_ext = xsb + 2;
// 					ysv_ext = ysb + 0;
// 					dx_ext  = dx0 - 2 - 2 * SQUISH_CONSTANT_2D;
// 					dy_ext  = dy0 + 0 - 2 * SQUISH_CONSTANT_2D;
// 				} else {
// 					xsv_ext = xsb + 0;
// 					ysv_ext = ysb + 2;
// 					dx_ext  = dx0 + 0 - 2 * SQUISH_CONSTANT_2D;
// 					dy_ext  = dy0 - 2 - 2 * SQUISH_CONSTANT_2D;
// 				}
// 			} else {
// 				// (1,0) and (0,1) are the closest two vertices.
// 				xsv_ext = xsb;
// 				ysv_ext = ysb;
// 				dx_ext  = dx0;
// 				dy_ext  = dy0;
// 			}

// 			xsb += 1;
// 			ysb += 1;
// 			dx0 = dx0 - 1 - 2 * SQUISH_CONSTANT_2D;
// 			dy0 = dy0 - 1 - 2 * SQUISH_CONSTANT_2D;
// 		}

// 		// Contributino (0,0) or (1,1)
// 		float attn0 = 2 - (dx0 * dx0) - (dy0 * dy0);
// 		if (attn0 > 0) {
// 			attn0 *= attn0;
// 			value += attn0 * attn0 * extrapolate2D(xsb, ysb, dx0, dy0);
// 		}

// 		// Extra Vertex
// 		float attn_ext = 2 - dx_ext * dx_ext - dy_ext * dy_ext;
// 		if (attn_ext > 0) {
// 			attn_ext *= attn_ext;
// 			value += attn_ext * attn_ext * extrapolate2D(xsv_ext, ysv_ext, dx_ext, dy_ext);
// 		}

// 		return value / NORM_CONSTANT_2D;
// 	}



// 	private float extrapolate2D(int xsb, int ysb, float dx, float dy) {
// 		int index = perm[(perm[xsb & 0xFF] + ysb) & 0xFF] & 0x0E;
// 		int g1 = GRADIENTS_2D[(index)     % GRADIENTS_2D.Length];
// 		int g2 = GRADIENTS_2D[(index + 1) % GRADIENTS_2D.Length];
// 		return (g1 * dx) + (g2 * dy);
// 	}

// }
