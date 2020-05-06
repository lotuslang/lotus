using System;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class BoolToken : ComplexToken
{
    protected bool val;

    public bool Value {
        get => val;
    }

    public BoolToken(string representation, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.number, location, leading, trailing)
    {
        if (representation.Length != 0 && !Boolean.TryParse(representation, out val)) {
            throw new InvalidInputException(representation, "as a boolean", "because a bool can only take the values 'true' and 'false'", location);
        }
    }

    public BoolToken(bool value, Location location) : base(value.ToString().ToLower(), TokenKind.@bool, location)
    { }
}