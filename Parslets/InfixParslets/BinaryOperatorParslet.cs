public sealed class BinaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence { get; }

    private readonly OperationType _opType;

    public BinaryOperatorParslet(Precedence precedence, OperationType operation) {
        Precedence = precedence;
        _opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token, ValueNode left) {
        var operatorToken = token as OperatorToken;

        Debug.Assert(operatorToken is not null);

        return new OperationNode(
            operatorToken,
            new[] { // FIXME: why not just use a static readonly buffer we write to each time ? 
                left,
                parser.Consume(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1)) // still is magic to me
            },
            _opType
        );
    }
}
