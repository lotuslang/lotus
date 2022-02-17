public sealed class NamespaceParslet : ITopLevelParslet<NamespaceNode>
{

    private static NamespaceParslet _instance = new();
    public static NamespaceParslet Instance => _instance;

	private NamespaceParslet() : base() { }

    public NamespaceNode Parse(TopLevelParser parser, Token namespaceToken) {
        var name = parser.ExpressionParser.Consume();

        if (namespaceToken is Token namespaceKeyword && namespaceKeyword == "namespace") {
            if (Utilities.IsName(name)) {
                return new NamespaceNode(name, namespaceKeyword);
            }
        }

        throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, namespaceToken.Location));
    }
}
