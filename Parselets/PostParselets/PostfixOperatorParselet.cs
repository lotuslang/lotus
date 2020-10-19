public sealed class PostfixOperatorParselet : IPostfixParselet<OperationNode>
{
    public Precedence Precedence { get; }

    readonly OperationType opType;

    public PostfixOperatorParselet(OperationType operation) {
        Precedence = Precedence.Unary;

        opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token, ValueNode left) {
        if (token is OperatorToken operatorToken) {
            return new OperationNode(
                operatorToken,
                new[] { left },
                opType
            );
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}