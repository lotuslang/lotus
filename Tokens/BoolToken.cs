[DebuggerDisplay("{Location} {Kind} : {val}")]
public record BoolToken : Token
{
    public new static readonly BoolToken NULL = new("", false, LocationRange.NULL, false);

    // no you can't remove it because properties can't be the out parameter in the "TryParse" call
    protected bool _val;

    public bool Value {
        get => _val;
        init => _val = value;
    }

    public BoolToken(string rep, bool value, LocationRange location, bool isValid = true)
        : base(rep, TokenKind.@bool, location, isValid)
    {
        _val = value;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITokenVisitor<T> visitor) => visitor.Visit(this);
}