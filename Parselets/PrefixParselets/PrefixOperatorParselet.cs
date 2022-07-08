public sealed class PrefixOperatorParslet : IPrefixParslet<OperationNode>
{
    private readonly OperationType _opType;

    public PrefixOperatorParslet(OperationType operation) {
        _opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token) {

        if (token is not OperatorToken opToken)
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));

        return new OperationNode(opToken, new[] { parser.Consume(Precedence.Unary) }, _opType);
    }
}