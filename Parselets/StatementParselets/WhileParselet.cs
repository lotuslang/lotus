public sealed class WhileParselet : IStatementParselet<WhileNode>
{
    public WhileNode Parse(Parser parser, Token whileToken) {
        if (!(whileToken is ComplexToken whileKeyword && whileKeyword == "while")) {
            throw Logger.Fatal(new InvalidCallException(whileToken.Location));
        }

        var isValid = true;

        // Wondering why do we do that ? See note in IfParselet.cs
        if (parser.Tokenizer.Consume() != "(") {
            Logger.Error(new UnexpectedTokenException(
                parser.Tokenizer.Current,
                context: "while parsing a while-loop's condition",
                expected: "an opening parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var condition = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            Logger.Error(new UnexpectedTokenException(
                parser.Tokenizer.Current,
                context: "while parsing a while-loop's condition",
                expected: "a closing parenthesis ')'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var body = parser.ConsumeSimpleBlock();

        return new WhileNode(condition, body, whileKeyword, isValid);
    }
}