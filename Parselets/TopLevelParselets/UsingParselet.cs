public sealed class UsingParslet : ITopLevelParslet<UsingNode>
{

    private static UsingParslet _instance = new();
    public static UsingParslet Instance => _instance;

	private UsingParslet() : base() { }

    public UsingNode Parse(TopLevelParser parser, Token usingToken) {
        if (usingToken is not Token usingKeyword || usingToken != "using")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, usingToken.Location));

        var importName = parser.ExpressionParser.Consume();

        var isValid = true;

        if (importName is not StringNode && !Utilities.IsName(importName)) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = importName,
                As = "a namespace in a using statement",
                Expected = "string or name"
            });

            isValid = false;
        }

        return new UsingNode(importName, usingKeyword, isValid);
    }
}