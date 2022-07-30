public sealed record NamespaceNode(NameNode Name, Token Token, bool IsValid = true)
: TopLevelNode(Token, new LocationRange(Token.Location, Name.Location), IsValid), IAccessible
{
    public new static readonly NamespaceNode NULL = new(NameNode.NULL, Token.NULL, false);

    public AccessLevel AccessLevel { get; set; } = AccessLevel.Public;

    AccessLevel IAccessible.DefaultAccessLevel => AccessLevel.Public;
    AccessLevel IAccessible.ValidLevels => AccessLevel.Public | AccessLevel.Internal;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
