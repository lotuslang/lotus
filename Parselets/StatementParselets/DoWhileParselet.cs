public sealed class DoWhileParselet : IStatementParselet<WhileNode>
{
    public WhileNode Parse(Parser parser, Token doToken) {
        if (!(doToken is ComplexToken doKeyword && doKeyword == "do")) {
            throw Logger.Fatal(new InvalidCallException(doToken.Location));
        }

        var isValid = true;

        var body = parser.ConsumeSimpleBlock();

        if (!(parser.Tokenizer.Consume() is ComplexToken whileKeyword && whileKeyword == "while")) {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "while parsing a do-while loop",
                expected: "the 'while' keyword"
            ));

            isValid = false;

            if (parser.Tokenizer.Current == "(") {
                parser.Tokenizer.Reconsume();
            }

            whileKeyword = new ComplexToken(
                parser.Tokenizer.Current,
                parser.Tokenizer.Current.Kind,
                parser.Tokenizer.Current.Location,
                false
            );
        }

        var conditionNode = parser.ConsumeValue();

        if (!(conditionNode is ParenthesizedValueNode condition)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as an do-while-loop condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            condition = new ParenthesizedValueNode(Token.NULL, Token.NULL, conditionNode);
        }

        return new WhileNode(condition, body, whileKeyword, doKeyword, isValid);
    }
}