namespace Lotus.Syntax;

public sealed record FromNode(Union<StringNode, NameNode> OriginName, Token Token)
: TopLevelNode(Token)
{
    public new static readonly FromNode NULL = new(StringNode.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
