using System;

public sealed class IdentifierParselet : IPrefixParselet<IdentNode>
{
    public IdentNode Parse(Parser _, Token identToken) {
        if (identToken.Kind == TokenKind.ident) {
            return new IdentNode(identToken.Representation, identToken);
        }

        throw new ArgumentException("Token needs to be an identifier.");
    }
}
