public sealed class ElseParslet : IStatementParslet<ElseNode>
{

    private static ElseParslet _instance = new();
    public static ElseParslet Instance => _instance;

	private ElseParslet() : base() { }

    public ElseNode Parse(StatementParser parser, Token elseToken) {
        if (!(elseToken is Token elseKeyword && elseKeyword == "else")) {
            throw Logger.Fatal(new InvalidCallException(elseToken.Location));
        }

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = IfParslet.Instance.Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseKeyword);
        }

        return new ElseNode(parser.ConsumeSimpleBlock(), elseKeyword);
    }
}