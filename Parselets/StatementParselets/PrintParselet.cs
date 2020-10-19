public sealed class PrintParselet : IStatementParselet<PrintNode>
{
    public PrintNode Parse(Parser parser, Token printToken) {
        if (!(printToken is ComplexToken printKeyword && printKeyword == "print")) {
            throw Logger.Fatal(new InvalidCallException(
                printToken.Location
            ));
        }

        return new PrintNode(printKeyword, parser.ConsumeValue());
    }
}