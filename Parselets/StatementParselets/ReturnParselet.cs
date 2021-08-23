public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{
    public ReturnNode Parse(StatementParser parser, Token returnToken) {

        if (!(returnToken is Token returnKeyword && returnKeyword == "return"))
            throw Logger.Fatal(new InvalidCallException(returnToken.Location));

        if (parser.Tokenizer.Peek() == "}") {
            return new ReturnNode(returnKeyword);
        }

        var value = parser.ExpressionParser.ConsumeValue();

        return new ReturnNode(value, returnKeyword);
    }
}
