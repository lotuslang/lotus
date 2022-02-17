
public sealed class ImportParslet : ITopLevelParslet<ImportNode>
{

    private static ImportParslet _instance = new();
    public static ImportParslet Instance => _instance;

	private ImportParslet() : base() { }

    public ImportNode Parse(TopLevelParser parser, Token fromToken) {
        if (fromToken is not Token fromKeyword || fromToken != "from")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, fromToken.Location));

        var fromOrigin = parser.ExpressionParser.Consume();

        var fromIsValid = true;

        var importIsValid = true;

        if (fromOrigin is not StringNode && !Utilities.IsName(fromOrigin)) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = fromOrigin,
                In = "a from statement",
                Expected = "string or name"
            });

            fromIsValid = false;
        }

        var from = new FromNode(fromOrigin, fromKeyword, fromIsValid);

        // TODO: Would it be better to have parser.ConsumeValue() here ? it would probably do the same thing
        //
        // For now, I think it's better to use parser.Tokenizer because it better represents what is needed here :
        // we don't need a value, we need a token. The context is important, and calling ConsumeValue() would ignore
        // that context, whereas if we consume it here, the context is clear
        //
        // The problem is that I don't really like exposing the Tokenizer to the public, because that is supposed to be an
        // "implementation detail", and it's kinda against the encapsulation principle (but I mean at this point this whole
        // thing could probably be written in weird F# since there's so much immutability. We only need OOP for the Tokens,
        // Nodes, Parslets and Toklets, because OOP is way easier to use to create a SyntaxTree and it's also the model
        // I'm the most comfortable with)
        //
        // Also, if this is an invalid statement, which one would wield the least errors/make recovery easier ?
        // After a (very short) test, the tokenizer seems better, but we should probably do more testing
        var importToken = parser.Tokenizer.Consume();

        if (importToken is not Token importKeyword || importKeyword != "import") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = importToken,
                In = "a import statement",
                Expected = "import"
            });

            importIsValid = false;

            importKeyword = new Token(importToken.Representation, TokenKind.keyword, importToken.Location, false);
        }

        var importList = new List<ValueNode>();

        do {
            var import = parser.ExpressionParser.Consume(); // consume the import's name

            if (!Utilities.IsName(import)) {

                if (!import.IsValid && import.Token.Representation == "*") {
                    Logger.errorStack.Pop();

                    Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                        Value = import.Token,
                        ExtraNotes = "Wildcards ('*') are not allowed in import statements. Consider writing a `using` statement instead. "
                                + "For example, you could write : 'using " + ASTHelper.PrintValue(from.OriginName) + "' "
                                + "at the top of your file.",
                    });
                } else {
                    Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                        Value = import,
                        In = "a import statement",
                        Expected = "a type name"
                    });
                }

                importIsValid = false;
            }

            importList.Add(import);
        } while (parser.Tokenizer.Consume() == ",");

        parser.Tokenizer.Reconsume();

        return new ImportNode(importList, from, importKeyword, importIsValid);
    }
}