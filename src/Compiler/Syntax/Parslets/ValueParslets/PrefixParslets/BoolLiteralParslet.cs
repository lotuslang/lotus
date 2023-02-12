namespace Lotus.Syntax;

public sealed class BoolLiteralParslet : IPrefixParslet<BoolNode>
{
    public static readonly BoolLiteralParslet Instance = new();

    public BoolNode Parse(ExpressionParser parser, Token token) {
        var boolToken = token as BoolToken;

        Debug.Assert(boolToken is not null);

        return new BoolNode(boolToken);
    }
}
