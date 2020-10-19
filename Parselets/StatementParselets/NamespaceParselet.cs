public sealed class NamespaceParselet : IStatementParselet<NamespaceNode>
{
    public NamespaceNode Parse(Parser parser, Token namespaceToken) {
        var name = parser.ConsumeValue();

        if (namespaceToken is ComplexToken namespaceKeyword && namespaceKeyword == "namespace") {
            if (Utilities.IsName(name)) {
                return new NamespaceNode(name, namespaceKeyword);
            }
        }

        throw Logger.Fatal(new InvalidCallException(namespaceToken.Location));
    }
}
