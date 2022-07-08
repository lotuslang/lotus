public record FunctionDeclarationNode(
    SimpleBlock Body,
    IList<FunctionParameter> Parameters,
    NameNode ReturnType,
    IdentToken Name,
    Token Token,
    Token OpeningParenthesis,
    Token ClosingParenthesis,
    Token ColonToken,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly FunctionDeclarationNode NULL
        = new(
            SimpleBlock.NULL,
            Array.Empty<FunctionParameter>(),
            NameNode.NULL,
            IdentToken.NULL,
            Token.NULL,
            Token.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

    internal bool isInternal = false;

    public bool HasReturnType => ReturnType != NameNode.NULL;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}

/// <summary>You define parameters, you make arguments</summary>
public record FunctionParameter(
    NameNode TypeName,
    IdentNode Name,
    ValueNode DefaultValue,
    Token EqualSign,
    bool IsValid = true
) {
    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;
}