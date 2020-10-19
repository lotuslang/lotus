using System.Collections.Generic;

public sealed class ForParselet : IStatementParselet<ForNode>
{
    public ForNode Parse(Parser parser, Token forToken) {
        if (!(forToken is ComplexToken forKeyword && forKeyword == "for"))
            throw Logger.Fatal(new InvalidCallException(forToken.Location));

        var header = new List<StatementNode>();

        var isValid = true;

        if (parser.Tokenizer.Consume() != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: parser.Tokenizer.Current,
                context: "in for loop header",
                expected: "an opening parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        var commaCount = 0; // FIXME: I feel dirty

        // no we can't use Parser.ConsumeCommaSeparatedList because here we can have empty "values"
        while (parser.Tokenizer.Peek() != ")") {

            if (parser.Tokenizer.Peek() == ",") {
                parser.Tokenizer.Consume();

                commaCount++;

                // add an empty statement
                // (can't use StatementNode.NULL because of position + this is a valid statement, whereas NULL is not)
                header.Add(new StatementNode("", new Token('\0', TokenKind.EOF, parser.Position)));

                continue;
            }

            header.Add(parser.Consume());

            if (parser.Tokenizer.Consume() != ",") {

                if (parser.Tokenizer.Current == ")") {
                    parser.Tokenizer.Reconsume();
                    break;
                }

                // FIXME: handle EOF

                Logger.Error(new UnexpectedTokenException(
                    token: parser.Tokenizer.Current,
                    context: "in comma-separated list",
                    expected: "a comma ','"
                ));

                isValid = false;

                parser.Tokenizer.Reconsume();
            }

            commaCount++;
        }

        parser.Tokenizer.Consume(); // consume the ')'

        if (header.Count > 3) {// FIXME: Choose an appropriate exception
            Logger.Error(new LotusException(
                message: "Too many statements in for-loop header (expected 3 statements (max), got " + header.Count + " statements).",
                location: header[^1].Token.Location
            ));

            isValid = false;
        }

        // if there's not enough statements in the header (happens when the last statement in not specified),
        // add an empty statement
        // (can't use StatementNode.NULL because of position + this is a valid statement, whereas NULL is not)
        if (header.Count == 2) header.Add(new StatementNode("", new Token('\0', TokenKind.EOF, parser.Position)));

        var body = parser.ConsumeSimpleBlock();

        return new ForNode(forKeyword, header.ToArray(), body, isValid);
    }
}