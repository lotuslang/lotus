public class ReturnParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token returnKeyword) {

        if (returnKeyword != "return") throw new UnexpectedTokenException(returnKeyword, "in return statement", "return");

        if (parser.Tokenizer.Peek() == ";") return new ReturnNode(ValueNode.NULL, returnKeyword as ComplexToken);

        var value = parser.ConsumeValue();

        return new ReturnNode(value, returnKeyword as ComplexToken);
    }
}
