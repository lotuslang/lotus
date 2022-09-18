public sealed record OperationNode : ValueNode
{
    public new static readonly OperationNode NULL = new(OperatorToken.NULL, ImmutableArray<ValueNode>.Empty, OperationType.Unknown) { IsValid = false };

    public ImmutableArray<Token> AdditionalTokens { get; }

    public new OperatorToken Token { get => (base.Token as OperatorToken)!; init => base.Token = value; }

    private readonly ImmutableArray<ValueNode> operands = ImmutableArray<ValueNode>.Empty;
    public ref readonly ImmutableArray<ValueNode> Operands => ref operands;

    public OperationType OperationType { get; init; }

    public OperationKind OperationKind
    =>  Operands.Length switch {
            1 => OperationKind.Unary,
            2 => OperationKind.Binary,
            3 => OperationKind.Ternary,
            _ => OperationKind.Unknown
        };

    public OperationNode(OperatorToken token, ImmutableArray<ValueNode> operands, OperationType opType) : this(token, operands, opType, ImmutableArray<Token>.Empty) {}

    public OperationNode(OperatorToken token, ImmutableArray<ValueNode> operands, OperationType opType, ImmutableArray<Token> additionalTokens)
        : base(token, token.Location)
    {
        OperationType = opType;
        AdditionalTokens = additionalTokens;
        Token = token;
        this.operands = operands;

        Location = OperationKind switch {
            OperationKind.Unary => new LocationRange(token.Location, operands[0].Location),
            OperationKind.Binary or OperationKind.Ternary => new LocationRange(operands[0].Location, operands[^1].Location),
            _ => Token.Location
        };
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
