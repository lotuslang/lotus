namespace Lotus.Syntax;

public sealed record EmptyStatementNode(LocationRange Location, bool IsValid)
    : StatementNode(Token.NULL, Location, IsValid)
{
    public new static readonly EmptyStatementNode NULL = new(LocationRange.NULL, false);

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}