/// <summary>
/// Represents a variable declaration statement (var a = b)
/// </summary>
public sealed record DeclarationNode(ValueNode Value, IdentToken Name, Token Token, Token EqualToken, bool IsValid = true)
: StatementNode(Token, new LocationRange(Token.Location, Value.Location), IsValid)
{
    public new static readonly DeclarationNode NULL = new(ValueNode.NULL, IdentToken.NULL, Token.NULL, Token.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
