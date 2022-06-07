public sealed class DoWhileParslet : IStatementParslet<WhileNode>
{

    private static DoWhileParslet _instance = new();
    public static DoWhileParslet Instance => _instance;

	private DoWhileParslet() : base() { }

    public WhileNode Parse(StatementParser parser, Token doToken) {
        if (!(doToken is Token doKeyword && doKeyword == "do")) {
            throw Logger.Fatal(new InvalidCallException(doToken.Location));
        }

        var isValid = true;

        var body = parser.ConsumeSimpleBlock();

        if (!(parser.Tokenizer.Consume() is Token whileKeyword && whileKeyword == "while")) {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "while parsing a do-while loop",
                expected: "the 'while' keyword"
            ));

            isValid = false;

            if (parser.Tokenizer.Current == "(") {
                Logger.Exceptions.Pop(); // remove the last exception

                Logger.Error(new UnexpectedTokenException(
                    message: "Did you forget the 'while' keyword before the condition ?",
                    token: parser.Tokenizer.Current
                ));

                parser.Tokenizer.Reconsume();
            }

            whileKeyword = new Token(
                parser.Tokenizer.Current,
                parser.Tokenizer.Current.Kind,
                parser.Tokenizer.Current.Location,
                false
            );
        }

        var conditionNode = parser.ExpressionParser.Consume();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as an do-while-loop condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = new ParenthesizedValueNode(
                    tuple.Count == 0 ? ValueNode.NULL : tuple.Values[0],
                    tuple.OpeningToken,
                    tuple.ClosingToken
                );
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL);
            }
        }

        return new WhileNode(condition, body, whileKeyword, doKeyword, isValid);
    }
}