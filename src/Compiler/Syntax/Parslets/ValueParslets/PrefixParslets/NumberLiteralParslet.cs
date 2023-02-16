namespace Lotus.Syntax;

public sealed class NumberLiteralParslet : IPrefixParslet<NumberNode>
{
    public static readonly NumberLiteralParslet Instance = new();

    public NumberNode Parse(Parser parser, Token token) {
        var numberToken = token as NumberToken;

        Debug.Assert(numberToken is not null);

        return new NumberNode(numberToken);
    }
}
