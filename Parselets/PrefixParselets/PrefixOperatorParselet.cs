public class PrefixOperatorParselet : IPrefixParselet
{
    readonly string opType;

    public PrefixOperatorParselet(string operation) {
        opType = operation;
    }

    public StatementNode Parse(Parser parser, Token token) {
        return new OperationNode(token as OperatorToken, new ValueNode[] { parser.ConsumeValue(Precedence.Unary) }, "prefix" + opType);
    }
}