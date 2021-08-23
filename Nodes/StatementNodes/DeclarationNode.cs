using System;

/// <summary>
/// Represents a variable declaration statement (var a = b)
/// </summary>
public class DeclarationNode : StatementNode
{
    public new static readonly DeclarationNode NULL = new(ValueNode.NULL, IdentToken.NULL, Token.NULL, Token.NULL, false);

    /// <summary>
    /// The value the variable is being initialized to
    /// </summary>
    public ValueNode Value { get; protected set; }

    /// <summary>
    /// The name of the variable being declared
    /// </summary>
    /// <value></value>
    public IdentToken Name { get; protected set; }

    public Token EqualToken { get; }

    /// <summary>
    /// Creates a DeclarationNode.
    /// </summary>
    /// <param name="value">The value of the variable</param>
    /// <param name="varName">The name of the variable</param>
    /// <param name="varKeywordToken">The token of the "var" keyword used</param>
    /// <returns></returns>
    public DeclarationNode(ValueNode value, IdentToken varName, Token varKeywordToken, Token equalToken, bool isValid = true)
        : base(varKeywordToken, new LocationRange(varKeywordToken.Location, value.Location), isValid)
    {
        EqualToken = equalToken;
        Name = varName;
        Value = value;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
