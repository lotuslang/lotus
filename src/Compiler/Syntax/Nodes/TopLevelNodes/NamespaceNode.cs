namespace Lotus.Syntax;

public sealed record NamespaceNode(NameNode Name, Token Token, ImmutableArray<Token> Modifiers)
: TopLevelNode(Token, new LocationRange(Token.Location, Name.Location))
{
    public new static readonly NamespaceNode NULL = new(NameNode.NULL, Token.NULL, default) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
