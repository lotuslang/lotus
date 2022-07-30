public sealed record OperationNode : ValueNode
{
    public new static readonly OperationNode NULL = new(OperatorToken.NULL, Array.Empty<ValueNode>(), OperationType.Unknown, false);

    public ImmutableArray<Token> AdditionalTokens { get; }

    public new OperatorToken Token { get => (base.Token as OperatorToken)!; init => base.Token = value; }

    private ImmutableArray<ValueNode> operands = ImmutableArray<ValueNode>.Empty;
    public ref readonly ImmutableArray<ValueNode> Operands => ref operands;

    public OperationType OperationType { get; init; }

    public OperationKind OperationKind {
        get => Operands.Length switch {
            1 => OperationKind.Unary,
            2 => OperationKind.Binary,
            3 => OperationKind.Ternary,
            _ => OperationKind.Unknown
        };
    }

    public OperationNode(OperatorToken token, IEnumerable<ValueNode> operands, OperationType opType, bool isValid = true, params Token[] additionalTokens)
        : base(token, token.Location, isValid)
    {
        OperationType = opType;
        AdditionalTokens = additionalTokens.ToImmutableArray();
        Token = token;
        this.operands = operands.ToImmutableArray();

        Location = OperationKind switch {
            OperationKind.Unary => new LocationRange(token.Location, operands.First().Location),
            OperationKind.Binary or OperationKind.Ternary => new LocationRange(operands.First().Location, operands.Last().Location),
            _ => Token.Location
        };

    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
