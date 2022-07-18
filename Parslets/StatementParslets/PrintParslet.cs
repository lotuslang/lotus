public sealed class PrintParslet : IStatementParslet<PrintNode>
{
    public static readonly PrintParslet Instance = new();

    public PrintNode Parse(StatementParser parser, Token printToken) {
        if (printToken is not Token printKeyword || printKeyword != "print") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, printToken.Location));
        }

        return new PrintNode(printKeyword, parser.ExpressionParser.Consume());
    }
}