﻿namespace NetTemplate.Misc;

using System.Diagnostics;
using ArgumentNullException = System.ArgumentNullException;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

/// <summary>
/// Represents a range.
/// </summary>
/// <remarks>
/// This structure represents an immutable integer interval that describes a range of values, from Start to End.
/// It is closed on the left and open on the right: [Start .. End). In the context of a Template it represents a
/// span of text, but the Interval structure itself is independent of any particular text.
/// </remarks>
[DebuggerDisplay("[{Start}..{End})")]
public sealed class Interval
{
    private readonly int _start;
    private readonly int _length;

    /// <summary>
    /// Initializes a new instance of an Interval with the given start point and length.
    /// </summary>
    /// <param name="start">The starting point of the interval.</param>
    /// <param name="length">The length of the interval.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="start"/> or <paramref name="length"/> is less than zero.
    /// </exception>
    public Interval(int start, int length)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException("start");
        if (length < 0)
            throw new ArgumentOutOfRangeException("length");

        this._start = start;
        this._length = length;
    }

    /// <summary>
    /// Gets the starting index of the span.
    /// </summary>
    /// <value>
    /// The starting index of the span.
    /// </value>
    public int Start
    {
        get
        {
            return _start;
        }
    }

    /// <summary>
    /// Gets the end of the interval.
    /// </summary>
    /// <remarks>
    /// The interval is open-ended on the right side, so that Start + Length = End.
    /// </remarks>
    public int End
    {
        get
        {
            return _start + _length;
        }
    }

    /// <summary>
    /// Gets the length of the interval, which is always non-negative.
    /// </summary>
    /// <value>
    /// The length of the interval, which is always non-negative.
    /// </value>
    public int Length
    {
        get
        {
            return _length;
        }
    }

    /// <summary>
    /// Determines whether or not this interval is empty.
    /// </summary>
    /// <value>
    /// true if the length of the interval is zero, otherwise false.
    /// </value>
    public bool IsEmpty
    {
        get
        {
            return _length == 0;
        }
    }

    /// <summary>
    /// Initializes a new instance of an Interval with the given start and end positions.
    /// </summary>
    /// <param name="start">The start position of the new interval.</param>
    /// <param name="end">The end position of the new interval.</param>
    /// <returns>The new interval.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="start"/> is less than 0, or <paramref name="end"/> is less than <paramref name="start"/>.</exception>
    public static Interval FromBounds(int start, int end)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException("start");
        if (end < 0)
            throw new ArgumentOutOfRangeException("end");

        return new Interval(start, end - start);
    }

    /// <summary>
    /// Determines whether the position lies within the interval.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>true if the position is greater than or equal to Start and less than End, otherwise false.</returns>
    public bool Contains(int position)
    {
        return position >= Start && position < End;
    }

    /// <summary>
    /// Determines whether the specified interval falls completely within this interval.
    /// </summary>
    /// <param name="interval">The interval to check.</param>
    /// <returns>true if the specified interval falls completely within this interval, otherwise false.</returns>
    public bool Contains(Interval interval)
    {
        if (interval == null)
            throw new ArgumentNullException("interval");

        return interval.Start >= this.Start && interval.End <= this.End;
    }

    public override string ToString()
    {
        return string.Format("[{0}..{1})", Start, End);
    }
}
