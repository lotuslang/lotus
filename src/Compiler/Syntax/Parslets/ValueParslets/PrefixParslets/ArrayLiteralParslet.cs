namespace Lotus.Syntax;

public sealed class ArrayLiteralParslet : IPrefixParslet<TupleNode>
{
    public static readonly ArrayLiteralParslet Instance = new();

    private static readonly TupleParslet<ValueNode> _valuesParslet
        = new(static (parser) => parser.ConsumeValue()) {
            Start = "[",
            End = "]"
        };

    public TupleNode Parse(Parser parser, Token leftBracket) {
        Debug.Assert(leftBracket == "[");

        parser.Tokenizer.Reconsume();

        return new TupleNode(_valuesParslet.Parse(parser));
    }
}
