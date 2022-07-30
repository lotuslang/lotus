public sealed class StatementBlockParslet {
    private readonly bool areOneLinersAllowed;

    public static readonly StatementBlockParslet Default = new(true);
    public static readonly StatementBlockParslet NoOneLiner = new(false);

    private StatementBlockParslet(bool areOneLinersAllowed) {
        this.areOneLinersAllowed = areOneLinersAllowed;
    }

    public Tuple<StatementNode> Parse(StatementParser parser) {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && parser.Tokenizer.Peek() != "{") {
            if (!parser.Consume(out var statement)) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a statement block",
                    Expected = "a statement",
                    Location = parser.Tokenizer.Current.Location
                });

                isValid = false;
            }

            return new Tuple<StatementNode>(
                new[] { statement },
                Token.NULL,
                Token.NULL,
                isValid
            ) {
                Location = statement.Location
            };
        }

        var openingBracket = parser.Tokenizer.Consume();

        // we don't have to check for EOF because that is (sorta) handled by "areOneLinersAllowed"
        if (openingBracket != "{") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openingBracket,
                In = "at the start of a statement block",
                Expected = "{",
                ExtraNotes = "This probably means there was an internal error, please report this!"
            });

            parser.Tokenizer.Reconsume();
        }

        var location = openingBracket.Location;

        var statements = new List<StatementNode>();

        while (parser.Tokenizer.Peek() != "}") {
            statements.Add(parser.Consume());

            if (parser.Tokenizer.Peek().Kind == TokenKind.EOF) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a simple block",
                    Expected = "a statement",
                    Location = parser.Tokenizer.Current.Location
                });

                isValid = false;

                break;
            }

            //if (Tokenizer.Peek() == ";") Tokenizer.Consume();
        }

        var closingBracket = parser.Tokenizer.Peek();

        if (!(!isValid && closingBracket.Kind == TokenKind.EOF) && closingBracket != "}") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = closingBracket,
                In = "a simple block",
                Expected = "the character '}'"
            });

            if (closingBracket.Kind == TokenKind.EOF) {
                // if this node was already invalid, it probably means that we already encountered an EOF,
                // so no need to tell the user twice
                if (!isValid) Logger.errorStack.Pop();
            } else {
                parser.Tokenizer.Reconsume();
            }

            isValid = false;
        }

        parser.Tokenizer.Consume();

        return new Tuple<StatementNode>(statements, openingBracket, closingBracket, isValid);
    }
}