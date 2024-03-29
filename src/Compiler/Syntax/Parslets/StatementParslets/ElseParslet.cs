namespace Lotus.Syntax;

public sealed class ElseParslet : IStatementParslet<ElseNode>
{
    public static readonly ElseParslet Instance = new();

    public ElseNode Parse(Parser parser, Token elseToken) {
        Debug.Assert(elseToken == "else");

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = IfParslet.Instance.Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseToken);
        }

        return new ElseNode(parser.ConsumeStatementBlock(), elseToken);
    }
}