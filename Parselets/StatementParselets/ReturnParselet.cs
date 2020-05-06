public sealed class ReturnParselet : IStatementParselet<ReturnNode>
{
    public ReturnNode Parse(Parser parser, Token returnToken) {

        if (!(returnToken is ComplexToken returnKeyword && returnKeyword == "return")) throw new UnexpectedTokenException(returnToken, "in return statement", "return");

        if (parser.Tokenizer.Peek() == ";" || parser.Tokenizer.Peek() == "}") {

            if (parser.Tokenizer.Peek() == ";") parser.Tokenizer.Consume();

            return new ReturnNode(ValueNode.NULL, returnKeyword);
        }

        var value = parser.ConsumeValue();

        return new ReturnNode(value, returnKeyword);
    }
}
