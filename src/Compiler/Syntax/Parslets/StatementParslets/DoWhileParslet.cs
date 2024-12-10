namespace Lotus.Syntax;

public sealed class DoWhileParslet : IStatementParslet<WhileNode>
{
    public static readonly DoWhileParslet Instance = new();

    public WhileNode Parse(Parser parser, Token doToken) {
        Debug.Assert(doToken == "do");

        var isValid = true;

        var body = parser.ConsumeStatementBlock();

        if (parser.Tokenizer.Consume() is not Token whileKeyword || whileKeyword != "while") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = parser.Tokenizer.Current,
                In = "while parsing a do-while loop",
                Expected = "the 'while' keyword"
            });

            isValid = false;

            if (parser.Tokenizer.Current == "(") {
                _ = Logger.errorStack.Pop(); // remove the last exception

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Message = "Did you forget the 'while' keyword before the condition ?",
                    Expected = "the 'while' keyword",
                    Value = parser.Tokenizer.Current
                });

                parser.Tokenizer.Reconsume();
            }

            whileKeyword = new Token(
                parser.Tokenizer.Current,
                parser.Tokenizer.Current.Kind,
                parser.Tokenizer.Current.Location
            ) { IsValid = false };
        }

        var conditionNode = parser.ConsumeValue();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = conditionNode,
                As = "a do-while-loop condition",
                Expected = "a condition between parenthesis (e.g. `(a == b)`)"
            });

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = tuple.AsParenthesized() with { IsValid = false };
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL) { IsValid = false };
            }
        }

        return new WhileNode(condition, body, whileKeyword, doToken) { IsValid = isValid };
    }
}