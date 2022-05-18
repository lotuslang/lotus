public sealed class PrintParslet : IStatementParslet<PrintNode>
{
    public PrintNode Parse(StatementParser parser, Token printToken) {
        if (!(printToken is Token printKeyword && printKeyword == "print")) {
            throw Logger.Fatal(new InvalidCallException(
                printToken.Location
            ));
        }

        return new PrintNode(printKeyword, parser.ExpressionParser.Consume());
    }
}