public record FromNode(ValueNode OriginName, Token Token, bool IsValid = true)
: TopLevelNode(Token, IsValid)
{
    public new static readonly FromNode NULL = new(ValueNode.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}
