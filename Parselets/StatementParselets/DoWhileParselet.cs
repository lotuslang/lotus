using System;

public class DoWhileParselet : IStatementParselet<WhileNode>
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

        // Wondering why do we do that ? See note in IfParselet.cs
        if (parser.Tokenizer.Consume() != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "while parsing the condition of a do-while loop",
                expected: "an opening parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var condition = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "while parsing the condition of a do-while loop",
                expected: "a closing parenthesis ')'"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        return new WhileNode(condition, body, whileKeyword, doKeyword, isValid);
    }
}