public sealed class PrefixOperatorParslet : IPrefixParslet<OperationNode>
{
    readonly OperationType opType;

    public PrefixOperatorParslet(OperationType operation) {
        opType = operation;
    }

    public OperationNode Parse(ExpressionParser parser, Token token) {

        if (token is not OperatorToken opToken)
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, token.Location));

        return new OperationNode(opToken, new[] { parser.Consume(Precedence.Unary) }, opType);
    }
}