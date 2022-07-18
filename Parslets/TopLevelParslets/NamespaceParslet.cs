public sealed class NamespaceParslet : ITopLevelParslet<NamespaceNode>
{
    public static readonly NamespaceParslet Instance = new();

    public NamespaceNode Parse(TopLevelParser parser, Token namespaceToken) {
        if (namespaceToken is not Token namespaceKeyword || namespaceKeyword != "namespace") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, namespaceToken.Location));
        }

        return new NamespaceNode(
            parser.ExpressionParser.Consume<NameNode>(NameNode.NULL, @as: "a namespace name"),
            namespaceKeyword
        );
    }
}
