namespace Lotus.Syntax;

public sealed class WhileParslet : IStatementParslet<WhileNode>
{
    public static readonly WhileParslet Instance = new();

    public WhileNode Parse(Parser parser, Token whileToken) {
        Debug.Assert(whileToken == "while");

        var isValid = true;

        var conditionNode = parser.ConsumeValue();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = conditionNode,
                As = "a while-loop condition",
                Expected = "a condition between parenthesis (e.g. `(a == b)`)"
            });

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = tuple.AsParenthesized() with { IsValid = false };
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL);
            }
        }

        var body = parser.ConsumeStatementBlock();

        return new WhileNode(condition, body, whileToken, Token.NULL) { IsValid = isValid };
    }
}