public sealed class PostfixOperatorParslet : IPostfixParslet<OperationNode>
{
    public Precedence Precedence => Precedence.Unary;

    private readonly OperationType _opType;

    public PostfixOperatorParslet(OperationType operation) {
        _opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token, ValueNode left) {
        if (token is not OperatorToken operatorToken) {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));
        }

        return new OperationNode(
            operatorToken,
            new[] { left },
            _opType
        );
    }
}