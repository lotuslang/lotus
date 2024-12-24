namespace Lotus.Syntax;

public sealed class TraitParslet : ITopLevelParslet<TraitNode>
{
    public static readonly TraitParslet Instance = new();

    private FunctionHeaderParslet _headerParslet = FunctionHeaderParslet.Instance;

    public TraitNode Parse(Parser parser, Token traitToken, ImmutableArray<Token> modifiers) {
        Debug.Assert(traitToken == "trait");

        bool isValid = true;

        var name = parser.ConsumeValue(IdentNode.NULL, @as: "a trait name");

        var openBracket = parser.Tokenizer.Consume();

        if (openBracket != "{") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = openBracket,
                Expected = "an opening bracket '{'",
                In = "a trait declaration",
            });
            isValid = false;
        }

        var functions = ImmutableArray.CreateBuilder<FunctionHeaderNode>();

        while (parser.Tokenizer.Peek() != "}" && !parser.Tokenizer.EndOfStream) {
            var funcModifiers = parser.ConsumeModifiers();

            var funcToken = parser.Tokenizer.Consume();
            if (funcToken != "func") {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = funcToken,
                    Expected = "the 'func' keyword",
                    In = "a trait definition",
                });
                isValid = false;
                continue;
            }

            var header = _headerParslet.Parse(parser, funcToken, funcModifiers);

            // todo: maybe we should only check if header is valid?
            if (parser.Tokenizer.Consume() != ";") {
                Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                    Value = parser.Tokenizer.Current,
                    Expected = "a semicolon ';'",
                    In = "a trait definition",
                });
                isValid = false;

                if (parser.Tokenizer.Current.Representation is "}" or "func")
                    parser.Tokenizer.Reconsume();
            }

            functions.Add(header);
        }

        var closeBracket = parser.Tokenizer.Consume();
        if (closeBracket.Kind is TokenKind.EOF) {
            Logger.Error(new UnexpectedEOFError(ErrorArea.Parser) {
                Location = parser.Tokenizer.Position,
                Value = parser.Tokenizer.Current,
                Expected = "a closing bracket '{'",
                In = "a trait definition",
            });
        }

        return new TraitNode(
            traitToken,
            name,
            openBracket,
            functions.DrainToImmutable(),
            closeBracket
        ) { IsValid = isValid, Modifiers = modifiers };
    }
}