namespace Lotus.Syntax;

public sealed record ReturnNode(ValueNode Value, Token Token)
: StatementNode(
    Token,
    Value != ValueNode.NULL
        ? new LocationRange(Token.Location, Value.Location)
        : Token.Location
)
{
    public new static readonly ReturnNode NULL = new(ValueNode.NULL, Token.NULL) { IsValid = false };

    public bool IsReturningValue => Value != ValueNode.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}
