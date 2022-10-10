namespace Lotus.Syntax;

public abstract record NameNode(Token Token, ImmutableArray<IdentToken> Parts)
: ValueNode(
    Token,
    Parts.Length == 0
        ? Token.Location
        : new LocationRange(Parts[0].Location, Parts[^1].Location)
)
{
    public new static readonly NameNode NULL = IdentNode.NULL;

    protected NameNode(Token token, ImmutableArray<IdentToken> parts, bool isValid) : this(token, parts) {
        IsValid = isValid;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}
