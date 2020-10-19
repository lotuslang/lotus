public sealed class PrefixOperatorParselet : IPrefixParselet<OperationNode>
{
    readonly OperationType opType;

    public PrefixOperatorParselet(OperationType operation) {
        opType = operation;
    }

    public OperationNode Parse(Parser parser, Token token) {

        if (!(token is OperatorToken opToken))
            throw Logger.Fatal(new InvalidCallException(token.Location));

        return new OperationNode(opToken, new[] { parser.ConsumeValue(Precedence.Unary) }, opType);
    }
}