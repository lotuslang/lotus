public sealed class ElseParslet : IStatementParslet<ElseNode>
{
    public static readonly ElseParslet Instance = new();

    public ElseNode Parse(StatementParser parser, Token elseToken) {
        if (elseToken is not Token elseKeyword || elseKeyword != "else") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, elseToken.Location));
        }

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = IfParslet.Instance.Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseKeyword);
        }

        return new ElseNode(parser.ConsumeSimpleBlock(), elseKeyword);
    }
}