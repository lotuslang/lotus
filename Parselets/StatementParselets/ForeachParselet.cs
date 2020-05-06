using System;
using System.Linq;

public sealed class ForeachParselet : IStatementParselet<ForeachNode>
{
    public ForeachNode Parse(Parser parser, Token foreachToken)
    {
        if (!(foreachToken is ComplexToken @foreach && @foreach == "foreach"))
            throw new UnexpectedTokenException(foreachToken, "in a foreach header", "the 'foreach' keyword");

        if (parser.Tokenizer.Consume() != "(") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "in foreach header", "an open parenthesis '('");
        }

        var itemNameToken = parser.Tokenizer.Consume();

        if (!(itemNameToken is ComplexToken itemName && itemName == TokenKind.ident)) {
            throw new UnexpectedTokenException(itemNameToken, "in a foreach header", TokenKind.ident);
        }

        var inToken = parser.Tokenizer.Consume();

        if (!(inToken is ComplexToken @in && @in == "in")) {
            throw new UnexpectedTokenException(itemNameToken, "in a foreach header", "the 'in' keyword");
        }

        var collectionName = parser.ConsumeValue();

        if (!Utilities.IsName(collectionName)) {
            throw new UnexpectedValueType(collectionName, "in a foreach header", typeof(OperationNode), typeof(IdentNode));
        }

        if (parser.Tokenizer.Consume() != ")") {
            throw new UnexpectedTokenException(parser.Tokenizer.Current, "in a foreach header", "a closing parenthesis ')'");
        }

        var body = parser.ConsumeSimpleBlock();

        return new ForeachNode(@foreach, @in, new IdentNode(itemNameToken.Representation, itemName), collectionName, body);
    }
}