using System;

[System.Diagnostics.DebuggerDisplay("{Location} {Kind} : {val}")]
public class NumberToken : ComplexToken
{
    protected double val;

    public double Value { get => val; }

    public NumberToken(string representation, LocationRange location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, TokenKind.number, location, isValid, leading, trailing)
    {
        if (isValid && representation.Length != 0 && !Double.TryParse(representation, out val))
            throw Logger.Fatal(new InternalErrorException(
                message: "This NumberToken has been marked valid, but could not parse string '" + representation + "' as a number",
                range: Location
            ));
    }

    // we should keep them because it's possible for someone to call ComplexToken.Add() on this instance
    // so that it wouldn't be a valid number.
    // However, in the state they are right now, they cannot really be used to create a number, so meh.
    public override void Add(char ch) {
        base.Add(ch);
        if (IsValid && !Double.TryParse(Representation, out val))
            throw Logger.Fatal(new InternalErrorException(
                message: "This NumberToken has been marked valid, but could not parse string '" + Representation + "' as a number",
                range: Location
            ));
    }

    public override void Add(string str) {
        base.Add(str);
        if (IsValid && !Double.TryParse(Representation, out val))
            throw Logger.Fatal(new InternalErrorException(
                message: "This NumberToken has been marked valid, but could not parse string '" + Representation + "' as a number",
                range: Location
            ));
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(TokenVisitor<T> visitor) => visitor.Visit(this);
}