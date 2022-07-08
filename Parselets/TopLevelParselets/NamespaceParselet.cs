public sealed class NamespaceParslet : ITopLevelParslet<NamespaceNode>
{
    public static readonly NamespaceParslet Instance = new();

    public NamespaceNode Parse(TopLevelParser parser, Token namespaceToken) {
        var name = parser.ExpressionParser.Consume();

        if (namespaceToken is not Token namespaceKeyword || namespaceKeyword != "namespace") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, namespaceToken.Location));
        }

        var node = new NamespaceNode(name, namespaceKeyword);

        if (!Utilities.IsName(name)) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = name,
                Expected = "a valid name",
                As = "a namespace name",
            });
        }

        return node;
    }
}
