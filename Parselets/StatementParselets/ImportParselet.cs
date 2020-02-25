using System;
using System.Collections.Generic;

public class ImportParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        if (!(token is ComplexToken fromKeyword && token == "from")) {
            throw new Exception();
        }

        var fromOrigin = parser.ConsumeValue();

        FromNode from;

        if (fromOrigin is IdentNode) {
            from = new FromNode(fromOrigin, fromKeyword, true);
        } else if (fromOrigin is StringNode str) {
            from = new FromNode(str, fromKeyword);
        } else {
            throw new Exception();
        }

        // TODO: Would it be better to have parser.ConsumeValue() here ? it would probably do the same thing
        //
        // For now, I think it's better to use parser.Tokenizer because it better represents what is needed here :
        // we don't need a value, we need a token. The context is important, and calling ConsumeValue() would ignore
        // that context, whereas if we consume it here, the context is clear
        //
        // The problem is that I don't like exposing the Tokenizer to the public, because that is supposed to be an "implementation
        // detail", and it's kinda against the encapsulation principle (but I mean at this point this whole thing could probably
        // be written in weird F# since there's so much immutability. We only need OOP for the Tokens, Nodes, Parselets and Toklets.
        // Because OOP is way easier to use to create a SyntaxTree and it's also the model i'm the most comfortable with)
        var importToken = parser.Tokenizer.Consume();

        if (!(importToken is ComplexToken importKeyword && token == "import")) {
            throw new Exception();
        }

        var importList = new List<ValueNode>();

        do {
            var import = parser.ConsumeValue();

            if (!Utilities.IsName(import)) throw new Exception();

            importList.Add(import);
        } while (parser.Tokenizer.Peek() == ",");

        return new ImportNode(importList, from, importKeyword);
    }
}