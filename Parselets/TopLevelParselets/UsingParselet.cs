public sealed class UsingParslet : ITopLevelParslet<UsingNode>
{
    public static readonly UsingParslet Instance = new();

    public UsingNode Parse(TopLevelParser parser, Token usingToken) {
        if (usingToken is not Token usingKeyword || usingToken != "using")
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, usingToken.Location));

        var isValid = parser.ExpressionParser.TryConsumeEither<StringNode, NameNode>(
            defaultVal: NameNode.NULL,
            out var import,
            out var importVal
        );

        if(!isValid) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = importVal,
                As = "a namespace",
                Expected = "either a string or a name"
            });
        }

        return new UsingNode(import, usingKeyword, isValid);
    }
}