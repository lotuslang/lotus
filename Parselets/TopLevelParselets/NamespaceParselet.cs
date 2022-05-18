public sealed class NamespaceParslet : ITopLevelParslet<NamespaceNode>
{
    public NamespaceNode Parse(TopLevelParser parser, Token namespaceToken) {
        var name = parser.ExpressionParser.Consume();

        if (namespaceToken is Token namespaceKeyword && namespaceKeyword == "namespace") {
            if (Utilities.IsName(name)) {
                return new NamespaceNode(name, namespaceKeyword);
            }
        }

        throw Logger.Fatal(new InvalidCallException(namespaceToken.Location));
    }
}
