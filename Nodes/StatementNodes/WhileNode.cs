using System.Diagnostics.CodeAnalysis;

public record WhileNode(
    ParenthesizedValueNode Condition,
    SimpleBlock Body,
    Token Token,
    Token? DoToken,
    bool IsValid = true
) : StatementNode(
    Token,
    DoToken is not null
        ? new LocationRange(DoToken.Location, Token.Location)
        : new LocationRange(Token.Location, Body.Location),
    IsValid
) {
    public new static readonly WhileNode NULL = new(ParenthesizedValueNode.NULL, SimpleBlock.NULL, Token.NULL, Token.NULL, false);

    [MemberNotNullWhen(true, nameof(DoToken))]
    public bool IsDoLoop => DoToken is not null;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}