using System;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class NumberToken : ComplexToken
{
    protected double val;

    public double Value { get => val; }

    public NumberToken(string representation, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.number, location, leading, trailing)
    {
        if (representation.Length != 0 && !Double.TryParse(representation, out val))
            throw new InvalidInputException(representation, "as a number", Location);
    }

    // we should keep them because it's possible for someone to call ComplexToken.Add() on this instance
    // so that it wouldn't be a valid number.
    // However, in the state they are right now, they cannot really be used to create a number, so meh.
    public new void Add(char ch) {
        rep.Append(ch);
        if (!Double.TryParse(Representation, out val))
            throw new InvalidInputException(Representation, "as a number", Location);
    }

    public new void Add(string str) {
        rep.Append(str);
        if (!Double.TryParse(Representation, out val))
            throw new InvalidInputException(Representation, "as a number", Location);
    }
}