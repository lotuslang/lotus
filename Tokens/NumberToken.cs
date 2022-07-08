[DebuggerDisplay("{Location} {Kind} : {val}")]
public record NumberToken : Token
{
    public new static readonly NumberToken NULL = new(double.NaN, LocationRange.NULL, false);

    protected double _val;

    public double Value {
        get => _val;
        init => _val = value;
    }

    public NumberToken(string representation, LocationRange location, bool isValid = true)
        : base(representation, TokenKind.number, location, isValid)
    {
        if (isValid && representation.Length != 0 && !Double.TryParse(representation, out _val))
            throw Logger.Fatal(new InternalError(ErrorArea.Tokenizer) {
                Message = "This NumberToken has been marked valid, but could not parse string '" + representation + "' as a number",
                Location = Location
            });
    }

    public NumberToken(double d, LocationRange location, bool isValid = true)
        : base(d.ToString().ToLower(), TokenKind.number, location, isValid)
    {
        _val = d;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}