using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LightJson;

public class DictionarySet<TKey, TValue> : Dictionary<TKey, HashSet<TValue>> {

    public void Add(TKey key, TValue value) {
        // Safely adds the provided value to the set pointed to by the key.
        // If the key is new, then a new set will be generated. 
        if (key == null) { throw new ArgumentNullException("Key is null"); }
        if( ! this.ContainsKey(key)) { this.Add(key, new HashSet<TValue>()); }
        this[key].Add(value);
    }

    public new void Add(TKey key, HashSet<TValue> values) {
        // Hide the base.Add(key, value) function to instead merge the given values into the 
        // previously existing values (if it exists).
        if (key == null) { throw new ArgumentNullException("Key is null"); }
        if ( ! this.ContainsKey(key)) { base.Add(key, values); }
        else { this[key].UnionWith(values); }
    }

    public void Add(TKey key, ICollection<TValue> values) {
        foreach(TValue elem in values) { this.Add(key, elem); }
    }

    public bool Remove(TKey key, TValue value) {
        if ( ! this.ContainsKey(key)) { return false; }
        if ( ! this[key].Remove(value)) {
            // If the value was not present in the hash set
            return false;
        }
        // Else the value was previously present, but has been sucesfully removed
        if (this[key].Count == 0) {
            // If the entry is now empty
            this.Remove(key);
        }
        return true;
    }

    public HashSet<TValue> safeGet(TKey key) {
        /**
         * If the provided key is contained, the matching pair HashSet is returnd.
         * Otherwise, an empty hashset is returned.
         */

        if (this.ContainsKey(key)) { return this[key]; }
        else                       { return new HashSet<TValue>(); }
    }

}
