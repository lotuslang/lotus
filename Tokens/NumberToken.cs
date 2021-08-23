using System;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public record NumberToken : Token
{
    public new static readonly NumberToken NULL = new(0d, LocationRange.NULL, false);

    protected double val;

    public double Value {
        get => val;
        init => val = value;
    }

    public NumberToken(string representation, LocationRange location, bool isValid = true)
        : base(representation, TokenKind.number, location, isValid)
    {
        if (isValid && representation.Length != 0 && !Double.TryParse(representation, out val))
            throw Logger.Fatal(new InternalErrorException(
                message: "This NumberToken has been marked valid, but could not parse string '" + representation + "' as a number",
                range: Location
            ));
    }

    public NumberToken(double d, LocationRange location, bool isValid = true)
        : base(d.ToString().ToLower(), TokenKind.number, location, isValid)
    {
        val = d;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}