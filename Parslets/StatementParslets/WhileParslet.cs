public sealed class WhileParslet : IStatementParslet<WhileNode>
{
    public static readonly WhileParslet Instance = new();

    public WhileNode Parse(StatementParser parser, Token whileToken) {
        Debug.Assert(whileToken == "while");

        var isValid = true;

        var conditionNode = parser.ExpressionParser.Consume();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = conditionNode,
                As = "a while-loop condition",
                Expected = "a condition between parenthesis (e.g. `(a == b)`)"
            });

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = tuple.AsParenthesized();
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL);
            }
        }

        var body = parser.ConsumeSimpleBlock();

        return new WhileNode(condition, body, whileToken, Token.NULL, isValid);
    }
}