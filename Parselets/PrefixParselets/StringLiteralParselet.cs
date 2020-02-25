using System;
using System.Collections.Generic;

public class StringLiteralParselet : IPrefixParselet
{
    public StatementNode Parse(Parser _, Token token) {
        if (token.Kind == TokenKind.@string) {
            return new StringNode(token.Representation, token);
        }

        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                node.AddSection(new Parser(new Consumer<Token>(section)).ConsumeValue());
            }

            return node;
        }

        throw new ArgumentException(nameof(token) + " needs to be a string.");
    }
}
