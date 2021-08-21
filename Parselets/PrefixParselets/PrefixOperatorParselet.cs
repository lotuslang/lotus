public sealed class PrefixOperatorParslet : IPrefixParslet<OperationNode>
{
    readonly OperationType opType;

    public PrefixOperatorParslet(OperationType operation) {
        opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token) {
        if (token is not OperatorToken opToken)
            throw Logger.Fatal(new InvalidCallException(token.Location));

        return new OperationNode(opToken, new[] { parser.ConsumeValue(Precedence.Unary) }, opType);
    }
}