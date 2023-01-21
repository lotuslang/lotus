namespace Lotus.Syntax;

public sealed class ImportParslet : ITopLevelParslet<ImportNode>
{
    public static readonly ImportParslet Instance = new();

    public ImportNode Parse(TopLevelParser parser, Token fromToken) {
        Debug.Assert(fromToken == "from");

        var fromIsValid = parser.ExpressionParser.TryConsumeEither<StringNode, NameNode>(
            defaultVal: NameNode.NULL,
            out var fromOrigin,
            out var fromVal
        );

        if (!fromIsValid) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = fromVal,
                In = "a from statement",
                Expected = "string or name"
            });
        }

        var from = new FromNode(fromOrigin, fromToken) { IsValid = fromIsValid };

        // todo(algo): Would it be better to have parser.ConsumeValue() here ? it would probably do the same thing
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

        var importIsValid = true;

        if (importToken is not Token importKeyword || importKeyword != "import") {
            Logger.Error(new UnexpectedError<Token>(ErrorArea.Parser) {
                Value = importToken,
                In = "a import statement",
                Expected = "import"
            });

            importIsValid = false;

            importKeyword = new Token(importToken.Representation, TokenKind.keyword, importToken.Location) { IsValid = false };
        }

        var importList = ImmutableArray.CreateBuilder<NameNode>();

        do {
            // consume the import's name
            var import = parser.ExpressionParser.Consume<NameNode>(
                NameNode.NULL,
                @in: "an import statement",
                @as: "a namespace or type name"
            );

            if (!import.IsValid) {
                if (parser.ExpressionParser.Current.Token == "*") {
                    _ = Logger.errorStack.Pop();

                    Logger.Error(new NotANameError(ErrorArea.Parser) {
                        Value = parser.ExpressionParser.Current,
                        ExtraNotes = "Wildcards ('*') are not allowed in import statements. Consider writing a `using` statement instead. "
                                + "For example, you could write: 'using "
                                + ASTUtils.PrintUnion(from.OriginName)
                                + "' "
                                + "at the top of your file.",
                    });
                }

                import = new IdentNode(
                    new IdentToken(
                        parser.ExpressionParser.Current.Token.Representation,
                        parser.ExpressionParser.Current.Location
                    ) { IsValid = false }
                );

                importIsValid = false;
            }

            importList.Add(import);
        } while (parser.Tokenizer.Consume() == ",");

        parser.Tokenizer.Reconsume();

        return new ImportNode(importList.ToImmutable(), from, importKeyword) { IsValid = importIsValid };
    }
}