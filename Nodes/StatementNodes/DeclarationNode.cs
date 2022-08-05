/// <summary>
/// Represents a variable declaration statement (var a = b)
/// </summary>
public sealed record DeclarationNode(ValueNode Value, IdentToken Name, Token Token, Token EqualToken)
: StatementNode(Token, new LocationRange(Token.Location, Value.Location))
{
    public new static readonly DeclarationNode NULL = new(ValueNode.NULL, IdentToken.NULL, Token.NULL, Token.NULL) { IsValid = false };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
