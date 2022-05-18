public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{
    public ReturnNode Parse(StatementParser parser, Token returnToken) {

        if (!(returnToken is Token returnKeyword && returnKeyword == "return"))
            throw Logger.Fatal(new InvalidCallException(returnToken.Location));

        var nextToken = parser.Tokenizer.Peek();

        var value = ValueNode.NULL;

        if (nextToken != "}" && !nextToken.HasTrivia(";", out _)) {
            value = parser.ExpressionParser.Consume();
        }

        return new ReturnNode(value, returnKeyword);
    }
}
