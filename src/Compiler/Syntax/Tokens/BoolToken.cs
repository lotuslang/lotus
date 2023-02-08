namespace Lotus.Syntax;

public sealed record BoolToken : Token
{
    public new static readonly BoolToken NULL = new("", false, LocationRange.NULL) { IsValid = false };

    private readonly bool _val;

    public ref readonly bool Value => ref _val;

    public BoolToken(string rep, bool value, LocationRange location)
        : base(rep, TokenKind.@bool, location)
    {
        _val = value;
    }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITokenVisitor<T> visitor) => visitor.Visit(this);
}