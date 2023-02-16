namespace Lotus.Syntax;

public sealed class ReturnParslet : IStatementParslet<ReturnNode>
{
    public static readonly ReturnParslet Instance = new();

    public ReturnNode Parse(Parser parser, Token returnToken) {
        Debug.Assert(returnToken == "return");

        var nextToken = parser.Tokenizer.Peek();

        ValueNode? value = null;

        // todo(logging): add special error message for when we encounter an end of block

        if (nextToken.Kind != TokenKind.semicolon) {
            value = parser.ConsumeValue();
        }

        return new ReturnNode(value, returnToken);
    }
}
