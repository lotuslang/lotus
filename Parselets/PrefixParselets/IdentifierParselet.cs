using System;

public sealed class IdentifierParselet : IPrefixParselet<IdentNode>
{
    public IdentNode Parse(Parser _, Token identToken) {
        if (identToken is ComplexToken ident && ident.Kind == TokenKind.ident) {
            return new IdentNode(identToken.Representation, ident);
        }

        throw new ArgumentException("Token needs to be an identifier.");
    }
}
