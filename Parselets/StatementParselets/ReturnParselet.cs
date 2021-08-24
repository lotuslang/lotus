public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{
    public ReturnNode Parse(StatementParser parser, Token returnToken) {

        if (!(returnToken is Token returnKeyword && returnKeyword == "return"))
            throw Logger.Fatal(new InvalidCallException(returnToken.Location));

        var nextToken = parser.Tokenizer.Peek();

        if (nextToken == "}" || nextToken.HasTrivia(";", out _)) {
            return new ReturnNode(null, returnKeyword);
        }

        var value = parser.ExpressionParser.ConsumeValue();

        return new ReturnNode(value, returnKeyword);
    }
}
