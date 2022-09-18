namespace Lotus.Syntax;

public sealed record FunctionDeclarationNode(
    Tuple<StatementNode> Body,
    Tuple<FunctionParameter> ParamList,
    NameNode ReturnType,
    IdentToken FuncName,
    Token Token,
    Token ColonToken
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location))
{
    public new static readonly FunctionDeclarationNode NULL
        = new(
            Tuple<StatementNode>.NULL,
            Tuple<FunctionParameter>.NULL,
            NameNode.NULL,
            IdentToken.NULL,
            Token.NULL,
            Token.NULL
        ) { IsValid = false };

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
    Token EqualSign
) : Parameter(Type, Name, new LocationRange(Type, DefaultValue == ValueNode.NULL ? Name : DefaultValue))
{
    public static readonly FunctionParameter NULL = new(
        NameNode.NULL,
        IdentNode.NULL,
        ValueNode.NULL,
        Token.NULL
    ) { IsValid = false };

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;
}