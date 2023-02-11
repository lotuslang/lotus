namespace Lotus.Syntax;

public sealed record FunctionDeclarationNode(
    Tuple<StatementNode> Body,
    Tuple<FunctionParameter> ParamList,
    NameNode? ReturnType,
    IdentToken FuncName,
    Token Token,
    Token ColonToken
) : StatementNode(Token, new LocationRange(Token.Location, Body.Location))
{
    public new static readonly FunctionDeclarationNode NULL
        = new(
            Tuple<StatementNode>.NULL,
            Tuple<FunctionParameter>.NULL,
            null,
            IdentToken.NULL,
            Token.NULL,
            Token.NULL
        ) { IsValid = false };

    internal bool isInternal = false;

    [MemberNotNullWhen(true, nameof(ReturnType))]
    public bool HasReturnType => ReturnType is not null;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IStatementVisitor<T> visitor) => visitor.Visit(this);
}

/// <summary>You define parameters, you make arguments</summary>
public record FunctionParameter(
    NameNode Type,
    IdentNode Name,
    ValueNode? DefaultValue,
    Token? EqualSign
) : Parameter(Type, Name, new LocationRange(Type, DefaultValue ?? Name))
{
    public static readonly FunctionParameter NULL = new(
        NameNode.NULL,
        IdentNode.NULL
    ) { IsValid = false };

    public FunctionParameter(NameNode type, IdentNode name) : this(type, name, null, null) { }

    [MemberNotNullWhen(true, nameof(DefaultValue), nameof(EqualSign))]
    public bool HasDefaultValue => DefaultValue is not null;
}