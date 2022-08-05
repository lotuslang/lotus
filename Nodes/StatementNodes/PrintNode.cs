public sealed record PrintNode(Token Token, ValueNode Value)
: StatementNode(Token, Token.Location)
{
    public new static readonly PrintNode NULL = new(Token.NULL, ValueNode.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}