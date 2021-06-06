using UnityEngine;

public static class NocabMathUtility {


	public static float extractDecimals(float f) {
		if (f > 0) { return f - Mathf.Floor(f); } 
		else { return f + Mathf.Ceil(f); }
	}

}
