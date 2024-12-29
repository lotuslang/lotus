namespace Lotus.Syntax;

public sealed class StringLiteralParslet : IPrefixParslet<StringNode>
{
    public static readonly StringLiteralParslet Instance = new();

    public StringNode Parse(Parser parser, Token token) {
        var strToken = token as StringToken;
        Debug.Assert(strToken is not null);
        return new StringNode(strToken);
    }
}
