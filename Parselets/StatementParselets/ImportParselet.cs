using System;
using System.Collections.Generic;

public sealed class ImportParselet : IStatementParselet<ImportNode>
{
    public ImportNode Parse(Parser parser, Token token) {
        if (!(token is ComplexToken fromKeyword && token == "from"))
            throw new UnexpectedTokenException(token, "in import statement", "from");

        var fromOrigin = parser.ConsumeValue();

        if (!(Utilities.IsName(fromOrigin) || fromOrigin is StringNode))
            throw new UnexpectedValueType(fromOrigin, "in from statement", "string or name");

        var from = new FromNode(fromOrigin, fromKeyword);

        // TODO: Would it be better to have parser.ConsumeValue() here ? it would probably do the same thing
        //
        // For now, I think it's better to use parser.Tokenizer because it better represents what is needed here :
        // we don't need a value, we need a token. The context is important, and calling ConsumeValue() would ignore
        // that context, whereas if we consume it here, the context is clear
        //
        // The problem is that I don't really like exposing the Tokenizer to the public, because that is supposed to be an
        // "implementation detail", and it's kinda against the encapsulation principle (but I mean at this point this whole
        // thing could probably be written in weird F# since there's so much immutability. We only need OOP for the Tokens,
        // Nodes, Parselets and Toklets, because OOP is way easier to use to create a SyntaxTree and it's also the model
        // I'm the most comfortable with)
        var importToken = parser.Tokenizer.Consume();

        if (!(importToken is ComplexToken importKeyword && importToken == "import"))
            throw new UnexpectedTokenException(importToken, "in import statement", "import");

        if (parser.Tokenizer.Peek() == "*") {
            return new ImportNode(
                new List<ValueNode>() { new ValueNode(parser.Tokenizer.Consume()) },
                from,
                importKeyword
            );
        }

        var importList = new List<ValueNode>();

        do {
            var import = parser.ConsumeValue(); // consume the import's name

            if (!(Utilities.IsName(import) || import.Representation == "*"))
                throw new UnexpectedValueType(import, "in import statement", "name or the characters '*'");

            importList.Add(import);
        } while (parser.Tokenizer.Consume() == ",");

        parser.Tokenizer.Reconsume();

        return new ImportNode(importList, from, importKeyword);
    }
}