public sealed class ElseParslet : IStatementParslet<ElseNode>
{
    public static readonly ElseParslet Instance = new();

    public ElseNode Parse(StatementParser parser, Token elseToken) {
        Debug.Assert(elseToken == "else");

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = IfParslet.Instance.Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseToken);
        }

        return new ElseNode(parser.ConsumeSimpleBlock(), elseToken);
    }
}