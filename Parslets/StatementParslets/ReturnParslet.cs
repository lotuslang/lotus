public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{
    public static readonly ReturnParslet Instance = new();

    public ReturnNode Parse(StatementParser parser, Token returnToken) {
        Debug.Assert(returnToken == "return");

        var nextToken = parser.Tokenizer.Peek();

        var value = ValueNode.NULL;

        // TODO: add special error message for when we encounter an end of block

        if (nextToken.Kind != TokenKind.semicolon) {
            value = parser.ExpressionParser.Consume();
        }

        return new ReturnNode(value, returnToken);
    }
}
