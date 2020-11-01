using System.Collections.Generic;

public sealed class StringLiteralParselet : IPrefixParselet<StringNode>
{
    public StringNode Parse(ExpressionParser parser, Token token) {
        if (token.Kind == TokenKind.@string) {
            return new StringNode(token.Representation, token);
        }

        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                // FIXME: See Parser.ConsumeValue comment (tldr Consumer can return null sometimes)
                node.AddSection(new ExpressionParser(new Consumer<Token>(section)).ConsumeValue());
            }

            return node;
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
