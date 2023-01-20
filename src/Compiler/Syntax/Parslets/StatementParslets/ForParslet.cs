namespace Lotus.Syntax;

public sealed class ForParslet : IStatementParslet<ForNode>
{
    public static readonly ForParslet Instance = new();

    public ForNode Parse(StatementParser parser, Token forToken) {
        Debug.Assert(forToken == "for");

        var header = ImmutableArray.CreateBuilder<StatementNode>(3);

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
                if (token.Representation is "," or ")") {
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
                    Expected = header.Count == 3 && header.Last() != parser.Default ? "a parenthesis ')'" : "a statement or comma ','",
                    Location = token.Location
                });

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

        if (!hasEOF) {
            if (header.Count > 3) { // fixme(logging): This is kind of a reoccurring error, should we make a specific class for it ?
                Logger.Error(new UnexpectedError<Node>(ErrorArea.Parser) {
                    Value = header.Last(),
                    Message = "Too many statements (expected up to 3 statements, got "
                            + header.Count + " statements).",
                    In = "a for-loop header",
                    Location = header[^1].Token.Location
                });
            } else if (header.Count <= 2) {
                var loc
                    = header.Count == 0
                    ? forToken.Location
                    : header[^1].Location;

                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = closingParen,
                    Message = "Not enough statements (expected 3 statements, got "
                            + header.Count + " statements).",
                    In = "a for-loop header",
                    Location = loc
                });
            }
        }

        isValid |= hasEOF;

        SanitizeHeaderSize(header, parser.Default);

        // We have to change position cause default filename doesn't match current otherwise
        var body = Tuple<StatementNode>.NULL with { Location = parser.Position };

        if (!hasEOF)
            body = parser.ConsumeStatementBlock();

        return new ForNode(forToken, new Tuple<StatementNode>(header.MoveToImmutable(), openingParen, closingParen), body) { IsValid = isValid };
    }

    private static void SanitizeHeaderSize(ImmutableArray<StatementNode>.Builder header, StatementNode defaultNode) {
        if (header.Count == header.Capacity)
            return;

        while (header.Count < header.Capacity) {
            header.Add(defaultNode);
        }
    }

    private static StatementNode GetDefaultStatement(LocationRange pos)
        => StatementNode.NULL with { Token = Token.NULL with { Location = pos, IsValid = true }, Location = pos, IsValid = true };
}