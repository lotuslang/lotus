public sealed class WhileParselet : IStatementParselet<WhileNode>
{
    public WhileNode Parse(Parser parser, Token whileToken) {
        if (!(whileToken is ComplexToken whileKeyword && whileKeyword == "while")) {
            throw Logger.Fatal(new InvalidCallException(whileToken.Location));
        }

        var isValid = true;

        var conditionNode = parser.ConsumeValue();

        if (!(conditionNode is ParenthesizedValueNode condition)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as an while-loop condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            condition = new ParenthesizedValueNode(Token.NULL, Token.NULL, conditionNode);
        }

        var body = parser.ConsumeSimpleBlock();

        return new WhileNode(condition, body, whileKeyword, isValid);
    }
}