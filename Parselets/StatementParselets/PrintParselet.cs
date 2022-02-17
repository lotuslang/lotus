public sealed class PrintParslet : IStatementParslet<PrintNode>
{

    private static PrintParslet _instance = new();
    public static PrintParslet Instance => _instance;

	private PrintParslet() : base() { }

    public PrintNode Parse(StatementParser parser, Token printToken) {
        if (printToken is not Token printKeyword || printKeyword != "print") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, printToken.Location));
        }

        return new PrintNode(printKeyword, parser.ExpressionParser.Consume());
    }
}