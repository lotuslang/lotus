using System;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class BoolToken : ComplexToken
{
    public new static readonly BoolToken NULL = new BoolToken(false, LocationRange.NULL, false);

    // no you can't remove it because properties can't be the out parameter in the "TryParse" call
    protected bool val;

    public bool Value {
        get => val;
    }

    public BoolToken(string representation, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.@bool, location, isValid, leading, trailing)
    {
        if (representation.Length != 0 && !Boolean.TryParse(representation, out val)) {
            throw Logger.Fatal(new InternalErrorException(
                message: $"Could not parse string {representation} as a boolean because a bool can only take the values 'true' and 'false'",
                location
            ));
        }
    }

    public BoolToken(bool value, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(value.ToString().ToLower(), TokenKind.@bool, location, isValid, leading, trailing)
    {
        val = value;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}