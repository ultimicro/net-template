namespace NetTemplate.Misc;

using System.Diagnostics;

/** A line number and char position within a line.  Used by the source
 *  mapping stuff to map address to range within a template.
 */
[DebuggerDisplay("({_line},{_charPosition})")]
public struct Coordinate
{
    private readonly int _line;
    private readonly int _charPosition;

    public Coordinate(int line, int charPosition)
    {
        this._line = line;
        this._charPosition = charPosition;
    }

    public int Line
    {
        get
        {
            return _line;
        }
    }

    public int CharPosition
    {
        get
        {
            return _charPosition;
        }
    }

    public override string ToString()
    {
        return _line + ":" + _charPosition;
    }
}
