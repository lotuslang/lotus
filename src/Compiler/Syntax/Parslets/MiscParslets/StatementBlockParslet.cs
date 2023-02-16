namespace Lotus.Syntax;

public sealed class StatementBlockParslet : IParslet<Tuple<StatementNode>> {
    private readonly bool areOneLinersAllowed;

    public static readonly StatementBlockParslet Default = new(true);
    public static readonly StatementBlockParslet NoOneLiner = new(false);

    private StatementBlockParslet(bool areOneLinersAllowed) {
        this.areOneLinersAllowed = areOneLinersAllowed;
    }

    public Tuple<StatementNode> Parse(Parser parser) {
        var isValid = true;

        // to consume a one-liner, you just consume a statement and return
        if (areOneLinersAllowed && parser.Tokenizer.Peek() != "{") {
            if (parser.EndOfStream) {
                Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                    In = "a statement block",
                    Expected = "a statement",
                    Location = parser.Tokenizer.Current.Location
                });

                isValid = false;
            }

            return new Tuple<StatementNode>(parser.ConsumeStatement()) { IsValid = isValid };
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

        var statements = ImmutableArray.CreateBuilder<StatementNode>();

        while (parser.Tokenizer.Peek() is not ({ Kind: TokenKind.EOF } or { Representation: "}" })) {
            statements.Add(parser.ConsumeStatement());
        }

        var closingBracket = parser.Tokenizer.Consume();

        // if we stopped because of an EOF
        if (closingBracket.Kind == TokenKind.EOF) {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                In = "a statement block",
                Expected = "a statement or the '}' character",
                Location = parser.Tokenizer.Current.Location
            });

            isValid = false;
        }

        return new Tuple<StatementNode>(statements.ToImmutable(), openingBracket, closingBracket) { IsValid = isValid };
    }
}