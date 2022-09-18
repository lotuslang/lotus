namespace Lotus.Syntax;

public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{
    public static readonly ArrayLiteralParslet Instance = new();

    private static readonly ValueTupleParslet<ValueNode> _valuesParslet
        = new(static (parser) => parser.Consume()) {
            Start = "[",
            End = "]"
        };

    public TupleNode Parse(ExpressionParser parser, Token leftBracket) {
        Debug.Assert(leftBracket == "[");

        parser.Tokenizer.Reconsume();

        return new TupleNode(_valuesParslet.Parse(parser));
    }
}
