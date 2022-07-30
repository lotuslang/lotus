public sealed record FunctionDeclarationNode(
    Tuple<StatementNode> Body,
    Tuple<FunctionParameter> ParamList,
    NameNode ReturnType,
    IdentToken FuncName,
    Token Token,
    Token ColonToken,
    bool IsValid = true
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location), IsValid)
{
    public new static readonly FunctionDeclarationNode NULL
        = new(
            Tuple<StatementNode>.NULL,
            Tuple<FunctionParameter>.NULL,
            NameNode.NULL,
            IdentToken.NULL,
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
    NameNode Type,
    IdentNode Name,
    ValueNode DefaultValue,
    Token EqualSign,
    bool IsValid = true
) : Parameter(Type, Name, new LocationRange(Type, DefaultValue == ValueNode.NULL ? Name : DefaultValue), IsValid)
{
    public static readonly FunctionParameter NULL = new(
        NameNode.NULL,
        IdentNode.NULL,
        ValueNode.NULL,
        Token.NULL,
        false
    );

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;
}