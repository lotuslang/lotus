namespace Lotus.Syntax;

public sealed record FunctionHeaderNode(
    Token Token,
    IdentToken Name,
    Tuple<FunctionParameter> Parameters,
    Token? ColonToken,
    NameNode? ReturnType
) : TopLevelNode(
    Token,
    new LocationRange(
        Token,
        ReturnType?.Location ?? Parameters.Location
    )
) {
    public new static readonly FunctionHeaderNode NULL = new(
        Token.NULL,
        IdentToken.NULL,
        Tuple<FunctionParameter>.NULL,
        null,
        null
    );

    public ImmutableArray<Token> Modifiers { get; init; }

    public bool HasReturnType => ReturnType is not null;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.ITopLevelVisitor<T> visitor) => visitor.Visit(this);
}