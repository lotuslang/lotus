public sealed class BinaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence { get; }

    private readonly OperationType _opType;

    public BinaryOperatorParslet(Precedence precedence, OperationType operation) {
        Precedence = precedence;
        _opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token, ValueNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new[] {
                    left,
                    parser.Consume(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1)) // still is magic to me
                },
                _opType
            );
        }

        throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
    }
}
