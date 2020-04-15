using System;
using System.Linq;

public class ForeachParselet : IStatementParselet
{
    public StatementNode Parse(Parser parser, Token foreachToken)
    {
        if (foreachToken != "foreach")
            throw new UnexpectedTokenException("a foreach statement must start with the keyword 'foreach'", foreachToken);

        if (parser.Tokenizer.Consume() != "(") throw new Exception();

        var itemName = parser.Tokenizer.Consume();

        if (!(itemName is ComplexToken)) throw new Exception();

        var inToken = parser.Tokenizer.Consume();

        if (!(inToken is ComplexToken && inToken == "in")) throw new Exception();

        var collectionName = parser.ConsumeValue();

        if (!Utilities.IsName(collectionName)) {
            throw new Exception();
        }

        if (parser.Tokenizer.Consume() != ")") {
            throw new Exception();
        }

        var body = parser.ConsumeSimpleBlock();

        return new ForeachNode(foreachToken as ComplexToken, inToken as ComplexToken, new IdentNode(itemName.Representation, itemName as ComplexToken), collectionName, body);
    }
}