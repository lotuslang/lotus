namespace Lotus.Syntax;

public sealed record ReturnNode(ValueNode? Value, Token Token)
: StatementNode(
    Token,
    Value is not null
        ? new LocationRange(Token.Location, Value.Location)
        : Token.Location
)
{
    public new static readonly ReturnNode NULL = new(null, Token.NULL) { IsValid = false };

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsReturningValue => Value is not null;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}
