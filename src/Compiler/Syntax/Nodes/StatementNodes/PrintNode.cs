namespace Lotus.Syntax;

public sealed record PrintNode(Token Token, ValueNode Value)
: StatementNode(Token, new LocationRange(Token, Value))
{
    public new static readonly PrintNode NULL = new(Token.NULL, ValueNode.NULL) { IsValid = false };

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}