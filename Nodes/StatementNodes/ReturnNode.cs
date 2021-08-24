using System.Diagnostics.CodeAnalysis;

public record ReturnNode(ValueNode? Value, Token Token, bool IsValid = true)
: StatementNode(
    Token,
    Value is not null
        ? new LocationRange(Token.Location, Value.Location)
        : Token.Location,
    IsValid
)
{
    public new static readonly ReturnNode NULL = new(ValueNode.NULL, Token.NULL, false);

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsReturningValue => Value != null;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
