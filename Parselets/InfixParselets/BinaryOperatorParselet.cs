public sealed class BinaryOperatorParslet : IInfixParslet<OperationNode>
{
    public Precedence Precedence { get; }

    readonly OperationType opType;

    public BinaryOperatorParslet(Precedence precedence, OperationType operation) {
        Precedence = precedence;
        opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token, ValueNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new[] {
                    left,
                    parser.ConsumeValue(Precedence - (operatorToken.IsLeftAssociative ? 0 : 1)) // still is magic to me
                },
                opType
            );
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
