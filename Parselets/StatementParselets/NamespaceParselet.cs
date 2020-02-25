using System;

public class NamespaceParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token token) {
        var name = parser.ConsumeValue();

        if (!(token is ComplexToken namespaceToken && token == "namespace")) {
            throw new Exception();
        }

        if (!Utilities.IsName(name)) {
            throw new Exception();
        }

        return new NamespaceNode(name, namespaceToken);
    }
}
