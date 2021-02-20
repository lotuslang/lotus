public sealed class PrintParslet : IStatementParslet<PrintNode>
{
    public PrintNode Parse(StatementParser parser, Token printToken) {
        if (!(printToken is ComplexToken printKeyword && printKeyword == "print")) {
            throw Logger.Fatal(new InvalidCallException(
                printToken.Location
            ));
        }

        return new PrintNode(printKeyword, parser.ExpressionParser.ConsumeValue());
    }
}