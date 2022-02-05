
public sealed class ForParslet : IStatementParslet<ForNode>
{
    public ForNode Parse(StatementParser parser, Token forToken) {
        if (!(forToken is Token forKeyword && forKeyword == "for"))
            throw Logger.Fatal(new InvalidCallException(forToken.Location));

        var header = new List<StatementNode>();

        var isValid = true;
        var hasEOF = false;

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedTokenException(
                token: openingParen,
                context: "in for loop header",
                expected: "an opening parenthesis '('"
            ));

            isValid = false;

            parser.Tokenizer.Reconsume();
        }

        if (parser.Tokenizer.Consume() == ",") {
            // add an empty statement
            header.Add(GetDefaultStatement(parser.Tokenizer.Position));
        }

        parser.Tokenizer.Reconsume();

        // no we can't use Parser.ConsumeCommaSeparatedList because here we can have empty "values"
        while (parser.Tokenizer.Consume(out var token) && token != ")") {

            if (token == ",") {
                token = parser.Tokenizer.Consume();

                // If the next token isn't one of these, then it's a statement, so we shouldn't insert anything
                if (token.Representation is ("," or ")")) {
                    // add an empty statement
                    header.Add(GetDefaultStatement(token.Location));

                    parser.Tokenizer.Reconsume();
                    continue;
                }
            }

            parser.Tokenizer.Reconsume();

            header.Add(parser.Consume());

            hasEOF = !parser.Tokenizer.Consume(out token);

            if (hasEOF) {
                Logger.Error(new UnexpectedEOFException(
                    context: "in a for-loop header",
                    expected: (header.Count == 3 && header.Last() != parser.Default ? "a parenthesis ')'" : "a statement or comma ','"),
                    range: token.Location
                ));

                isValid = false;

                while (header.Count < 3)
                    header.Add(parser.Default);

                break;
            }

            if (token.Representation is not ("," or ")")) {
                Logger.Error(new UnexpectedTokenException(
                    token: parser.Tokenizer.Current,
                    context: "in comma-separated list",
                    expected: "a comma ','"
                ));

                isValid = false;
            }

            parser.Tokenizer.Reconsume();
        }

        var closingParen = parser.Tokenizer.Current; // consume the ')'

        if (!hasEOF && header.Count > 3) {// FIXME: Choose an appropriate exception
            Logger.Error(new LotusException(
                message: "Too many statements in for-loop header (expected 3 statements (max), got " + header.Count + " statements).",
                range: header[^1].Token.Location
            ));

            isValid = false;
        } else if (!hasEOF && header.Count <= 2) {
            Logger.Error(new LotusException(
                message: "Not enough statements in for-loop header (expected 3 statements (max), got " + header.Count + " statements)."
                        +" Did you forget some commas ?",
                range: new LocationRange(openingParen.Location, closingParen.Location)
            ));
        }

        // We have to change position cause default filename doesn't match current otherwise
        var body = SimpleBlock.NULL with { Location = parser.Position };

        if (!hasEOF)
            body = parser.ConsumeSimpleBlock();

        return new ForNode(forKeyword, header.ToArray(), body, openingParen, closingParen, isValid);
    }

    private static StatementNode GetDefaultStatement(LocationRange pos)
        => StatementNode.NULL with { Token = Token.NULL with { Location = pos, IsValid = true }, Location = pos };
}