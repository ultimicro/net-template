namespace NetTemplate.Compiler;

using System.Collections.Generic;

/** A unique set of strings where we can get a string's index.
 *  We can also get them back out in original order.
 */
public class StringTable
{
    private readonly Dictionary<string, int> table = new Dictionary<string, int>();
    private int i = -1;

    public virtual int Add(string s)
    {
        int I;
        if (table.TryGetValue(s, out I))
            return I;

        i++;
        table[s] = i;
        return i;
    }

    public virtual string[] ToArray()
    {
        string[] a = new string[table.Count];
        int i = 0;
        foreach (string s in table.Keys)
            a[i++] = s;

        return a;
    }
}
