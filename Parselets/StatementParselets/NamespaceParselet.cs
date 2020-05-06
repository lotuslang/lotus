using System;

public sealed class NamespaceParselet : IStatementParselet<NamespaceNode>
{
    public NamespaceNode Parse(Parser parser, Token token) {
        var name = parser.ConsumeValue();

        if (token is ComplexToken namespaceToken && token == "namespace") {
            if (Utilities.IsName(name)) {
                return new NamespaceNode(name, namespaceToken);
            }
        }

        throw new UnexpectedTokenException(token, "in namespace statement", "namespace");
    }
}
