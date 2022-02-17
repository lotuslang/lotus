using System.Collections.ObjectModel;

public record OperationNode : ValueNode
{
    public new static readonly OperationNode NULL = new(OperatorToken.NULL, Array.Empty<ValueNode>(), OperationType.Unknown, false);

    public ReadOnlyCollection<Token> AdditionalTokens { get; }

    public new OperatorToken Token { get => (base.Token as OperatorToken)!; init => base.Token = value; }

    private ReadOnlyCollection<ValueNode> operands = new(Array.Empty<ValueNode>());
    public ReadOnlyCollection<ValueNode> Operands {
        get => operands;
        init {
            operands = value;

            Location = OperationKind switch {
                OperationKind.Unary => new LocationRange(Token.Location, operands[0].Location),
                OperationKind.Binary => new LocationRange(operands[0].Location, operands[1].Location),
                OperationKind.Ternary => new LocationRange(operands[0].Location, operands[2].Location),
                _ => Token.Location
            };
        }
    }

    public OperationType OperationType { get; init; }

    public OperationKind OperationKind {
        get => Operands.Count switch {
            1 => OperationKind.Unary,
            2 => OperationKind.Binary,
            3 => OperationKind.Ternary,
            _ => OperationKind.Unknown
        };
    }

    public OperationNode(OperatorToken token, IList<ValueNode> operands, OperationType opType, bool isValid = true, params Token[] additionalTokens)
        : base(token, token.Location, isValid)
    {
        OperationType = opType;
        AdditionalTokens = new ReadOnlyCollection<Token>(additionalTokens);
        Token = token;

        Location = OperationKind switch {
            OperationKind.Unary => new LocationRange(token.Location, operands.First().Location),
            OperationKind.Binary or OperationKind.Ternary => new LocationRange(operands.First().Location, operands.Last().Location),
            _ => Token.Location
        };
        Operands = operands.AsReadOnly();
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
