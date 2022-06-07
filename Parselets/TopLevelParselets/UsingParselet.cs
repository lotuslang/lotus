public sealed class UsingParslet : ITopLevelParslet<UsingNode>
{

    private static UsingParslet _instance = new();
    public static UsingParslet Instance => _instance;

	private UsingParslet() : base() { }

    public UsingNode Parse(TopLevelParser parser, Token usingToken) {
        if (!(usingToken is Token usingKeyword && usingToken == "using"))
            throw Logger.Fatal(new InvalidCallException(usingToken.Location));

        var importName = parser.ExpressionParser.Consume();

        var isValid = true;

        if (!(importName is StringNode || Utilities.IsName(importName))) {
            Logger.Error(new UnexpectedValueTypeException(
                node: importName,
                context: "as a namespace in a using statement",
                expected: "string or name"
            ));

            isValid = false;
        }

        return new UsingNode(importName, usingKeyword, isValid);
    }
}