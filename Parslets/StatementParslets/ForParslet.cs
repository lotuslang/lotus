
public sealed class ForParslet : IStatementParslet<ForNode>
{
    public static readonly ForParslet Instance = new();

    public ForNode Parse(StatementParser parser, Token forToken) {
        Debug.Assert(forToken == "for");

        var header = new List<StatementNode>();

        var isValid = true;
        var hasEOF = false;

        var openingParen = parser.Tokenizer.Consume();

        if (openingParen != "(") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openingParen,
                In = "a for loop header",
                Expected = "an opening parenthesis '('"
            });

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

            header.Add(parser.Consume(false));

            hasEOF = !parser.Tokenizer.Consume(out token);

            if (hasEOF) {
                Logger.Error(new UnexpectedEOFError() {
                    In = "a for-loop header",
                    Expected = (header.Count == 3 && header.Last() != parser.Default ? "a parenthesis ')'" : "a statement or comma ','"),
                    Location = token.Location
                });

                isValid = false;

                while (header.Count < 3)
                    header.Add(parser.Default);

                break;
            }

            if (token.Representation is not ("," or ")")) {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Current,
                    In = "a comma-separated list",
                    Expected = "a comma ','"
                });

                isValid = false;
            }

            parser.Tokenizer.Reconsume();
        }

        var closingParen = parser.Tokenizer.Current; // consume the ')'

        if (!hasEOF && header.Count > 3) { // FIXME: This is kind of a reoccurring error, should we make a specific class for it ?
            Logger.Error(new UnexpectedError<Node>(ErrorArea.Parser) {
                Message = "Too many statements (expected up to 3 statements, got "
                        + header.Count + " statements).",
                In = "a for-loop header",
                Location = header[^1].Token.Location
            });

            isValid = false;
        } else if (!hasEOF && header.Count <= 2) {
            Logger.Error(new UnexpectedError<Node>(ErrorArea.Parser) {
                Message = "Not enough statements (expected up to 3 statements, got "
                        + header.Count + " statements).",
                In = "a for-loop header",
                Location = header[^1].Token.Location
            });
        }

        // We have to change position cause default filename doesn't match current otherwise
        var body = Tuple<StatementNode>.NULL with { Location = parser.Position };

        if (!hasEOF)
            body = parser.ConsumeSimpleBlock();

        return new ForNode(forToken, new Tuple<StatementNode>(header, openingParen, closingParen), body, isValid);
    }

    private static StatementNode GetDefaultStatement(LocationRange pos)
        => StatementNode.NULL with { Token = Token.NULL with { Location = pos, IsValid = true }, Location = pos, IsValid = true };
}