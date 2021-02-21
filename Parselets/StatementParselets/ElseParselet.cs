public sealed class ElseParslet : IStatementParslet<ElseNode>
{
    public ElseNode Parse(StatementParser parser, Token elseToken) {
        if (!(elseToken is ComplexToken elseKeyword && elseKeyword == "else")) {
            throw Logger.Fatal(new InvalidCallException(elseToken.Location));
        }

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = new IfParslet().Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseKeyword);
        }

        return new ElseNode(parser.ConsumeSimpleBlock(), elseKeyword);
    }
}