using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NocabHashUtility  {

	public static int generateHash(ICollection<object> definingElements) {
		int hash = 23;
		int prime = 31;
		foreach(object obj in definingElements) { hash = (prime * hash) + obj.GetHashCode(); }
		return hash;
	}

}
