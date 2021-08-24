using System;

/// <summary>
/// Represents a variable declaration statement (var a = b)
/// </summary>
public record DeclarationNode(ValueNode Value, IdentToken Name, Token Token, Token EqualToken, bool IsValid = true)
: StatementNode(Token, new LocationRange(Token.Location, Value.Location), IsValid)
{
    public new static readonly DeclarationNode NULL = new(ValueNode.NULL, IdentToken.NULL, Token.NULL, Token.NULL, false);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
