namespace NetTemplate.Misc;

using System.Collections.Generic;

/** A hash table that maps a key to a list of elements not just a single. */
public class MultiMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
{
    public virtual void Add(TKey key, TValue value)
    {
        List<TValue> elementsForKey;
        if (!TryGetValue(key, out elementsForKey))
        {
            elementsForKey = new List<TValue>();
            base.Add(key, elementsForKey);
        }

        elementsForKey.Add(value);
    }
}
