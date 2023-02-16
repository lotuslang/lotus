namespace Lotus.Syntax;

public sealed class CharLiteralParslet : IPrefixParslet<CharNode>
{
    public static readonly CharLiteralParslet Instance = new();

    public CharNode Parse(Parser parser, Token token) {
        var charToken = token as CharToken;

        Debug.Assert(charToken is not null);

        return new CharNode(charToken);
    }
}
