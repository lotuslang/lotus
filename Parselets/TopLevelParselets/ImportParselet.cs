using System.Collections.Generic;

public sealed class ImportParslet : ITopLevelParslet<ImportNode>
{
    public ImportNode Parse(TopLevelParser parser, Token fromToken) {
        if (!(fromToken is Token fromKeyword && fromToken == "from"))
            throw Logger.Fatal(new InvalidCallException(fromToken.Location));

        var fromOrigin = parser.ExpressionParser.ConsumeValue();

        var fromIsValid = true;

        var importIsValid = true;

        if (!(Utilities.IsName(fromOrigin) || fromOrigin is StringNode)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: fromOrigin,
                context: "in from statement",
                expected: "string or name"
            ));

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

        if (!(importToken is Token importKeyword && importKeyword == "import")) {
            Logger.Error(new UnexpectedTokenException(
                token: importToken,
                context: "in import statement",
                expected: "import"
            ));

            importIsValid = false;

            importKeyword = new Token(importToken.Representation, TokenKind.keyword, importToken.Location, false);
        }

        var importList = new List<ValueNode>();

        do {
            var import = parser.ExpressionParser.ConsumeValue(); // consume the import's name

            if (!Utilities.IsName(import)) {

                if (!import.IsValid && import.Token.Representation == "*") {
                    Logger.Exceptions.Pop();

                    Logger.Error(new LotusException(
                        message: "Wildcards ('*') are not allowed in import statements. Use a `using` statement instead. "
                                + "For example, you could write : 'using " + ASTHelper.PrintValue(from.OriginName) + "' "
                                + "at the top of your file.",
                        range: import.Token.Location
                    ));
                } else {
                    Logger.Error(new UnexpectedValueTypeException(
                        node: import,
                        context: "in import statement",
                        expected: "a type name"
                    ));
                }

                importIsValid = false;
            }

            importList.Add(import);
        } while (parser.Tokenizer.Consume() == ",");

        parser.Tokenizer.Reconsume();

        return new ImportNode(importList, from, importKeyword, importIsValid);
    }
}