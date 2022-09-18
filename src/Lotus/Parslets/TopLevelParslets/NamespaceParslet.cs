public sealed class NamespaceParslet : ITopLevelParslet<NamespaceNode>
{
    public static readonly NamespaceParslet Instance = new();

    public NamespaceNode Parse(TopLevelParser parser, Token namespaceToken) {
        Debug.Assert(namespaceToken == "namespace");

        return new NamespaceNode(
            parser.ExpressionParser.Consume<NameNode>(NameNode.NULL, @as: "a namespace name"),
            namespaceToken
        );
    }
}
