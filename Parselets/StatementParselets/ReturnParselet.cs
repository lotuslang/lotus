public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{

    private static ReturnParslet _instance = new();
    public static ReturnParslet Instance => _instance;

	private ReturnParslet() : base() { }

    public ReturnNode Parse(StatementParser parser, Token returnToken) {

        if (returnToken is not Token returnKeyword || returnKeyword != "return")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, returnToken.Location));

        var nextToken = parser.Tokenizer.Peek();

        var value = ValueNode.NULL;

        if (nextToken != "}" && !nextToken.HasTrivia(";", out _)) {
            value = parser.ExpressionParser.Consume();
        }

        return new ReturnNode(value, returnKeyword);
    }
}
