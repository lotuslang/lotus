namespace Lotus.Syntax;

public sealed record BreakNode(Token Token) : StatementNode(Token)
{
    public new static readonly BreakNode NULL = new(Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}