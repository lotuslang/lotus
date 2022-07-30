public sealed record ContinueNode(Token Token, bool IsValid = true) : StatementNode(Token, IsValid)
{
    public new static readonly ContinueNode NULL = new(Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}