using System;
using System.Collections.Generic;

public sealed class StringLiteralParselet : IPrefixParselet<StringNode>
{
    public StringNode Parse(Parser parser, Token token) {
        if (token.Kind == TokenKind.@string) {
            return new StringNode(token.Representation, token);
        }

        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                // FIXME: See Parser.ConsumeValue comment
                node.AddSection(new Parser(new Consumer<Token>(section), parser.Grammar).ConsumeValue());
            }

            return node;
        }

        throw new ArgumentException("Token needs to be a string.");
    }
}
