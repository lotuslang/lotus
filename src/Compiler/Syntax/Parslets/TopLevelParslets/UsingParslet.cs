namespace Lotus.Syntax;

public sealed class UsingParslet : ITopLevelParslet<UsingNode>
{
    public static readonly UsingParslet Instance = new();

    public UsingNode Parse(Parser parser, Token usingToken, ImmutableArray<Token> modifiers) {
        Debug.Assert(usingToken == "using");

        Parser.ReportIfAnyModifiers(modifiers, "using statements", out var isValid);

        isValid &= parser.TryConsumeEitherValues<StringNode, NameNode>(
            defaultVal: NameNode.NULL,
            out var import,
            out var importVal
        );

        if(!isValid) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = importVal,
                In = "a using statement",
                As = "a namespace",
                Expected = "either a string or a name"
            });
        }

        return new UsingNode(import, usingToken) { IsValid = isValid };
    }
}