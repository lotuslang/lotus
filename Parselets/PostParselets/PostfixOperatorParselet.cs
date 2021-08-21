public sealed class PostfixOperatorParslet : IPostfixParslet<OperationNode>
{
    public Precedence Precedence { get; }

    readonly OperationType opType;

    public PostfixOperatorParslet(OperationType operation) {
        Precedence = Precedence.Unary;

        opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token, ValueNode left) {
        if (token is not OperatorToken operatorToken) {
            throw Logger.Fatal(new InvalidCallException(token.Location));
        }

        return new OperationNode(
            operatorToken,
            new[] { left },
            opType
        );
    }
}