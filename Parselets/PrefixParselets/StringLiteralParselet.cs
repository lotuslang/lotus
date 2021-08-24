using System.Collections.Generic;

public sealed class StringLiteralParslet : IPrefixParslet<StringNode>
{
    public StringNode Parse(ExpressionParser parser, Token token) {

        if (token is ComplexStringToken complexString) {
            var node = new ComplexStringNode(complexString, new List<ValueNode>());

            foreach (var section in complexString.CodeSections) {
                // FIXME: See Parser.ConsumeValue comment (tldr Consumer can return null sometimes)
                var sectionConsumer = new Consumer<Token>(section, Token.NULL);
                var sectionParser = new ExpressionParser(sectionConsumer);
                node.AddSection(sectionParser.ConsumeValue());
                if (sectionConsumer.Peek() != sectionConsumer.Default) {
                    Logger.Error(new UnexpectedTokenException(
                        token: sectionConsumer.Consume(),
                        message: "Too many tokens in interpolated string's code section."
                                + " Code sections should only contain a *one value* each."
                                + " This probably means that you forgot a closing `}`,"
                                + " yet a following *valid* code section (the `{...}`"
                                + " parts of an interpolated string) \"closed\" the malformed one."
                                + " If this isn't the case, then you probably wrote a statement instead of a value,"
                                + " which isn't allowed"
                    ));
                }
            }

            return node;
        }

        if (token.Kind == TokenKind.@string) {
            return new StringNode(token.Representation, token);
        }

        throw Logger.Fatal(new InvalidCallException(token.Location));
    }
}
