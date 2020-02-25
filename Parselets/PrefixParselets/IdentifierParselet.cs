using System;

public class IdentifierParselet : IPrefixParselet
{
    public StatementNode Parse(Parser _, Token token) {
        if (token.Kind == TokenKind.ident) {
            return new IdentNode(token.Representation, token);
        }

        throw new ArgumentException(nameof(token) + " needs to be an identifier.");
    }
}
