using System;
using System.Collections.Generic;

public class IfParselet : IStatementParselet<IfNode>
{
    public IfNode Parse(Parser parser, Token ifToken) {
        if (!(ifToken is ComplexToken ifKeyword && ifKeyword == "if")) {
            throw new Exception();
        }

        // We have to parse only the value inside because, if we use parentheses and have
        // only an identifier inside (`isSomething`, `condition`, or even just `true`),
        // LeftParenParselet while parse it as a type cast, and thus will try to consume the
        // value that should be after the type cast; except that since this is an if condition,
        // it is usually followed by a bracket or another statement, so it will error-out.
        // Therefore, we have to consume the opening parenthesis, then the value inside, then
        // the closing parenthesis
        if (parser.Tokenizer.Consume() != "(") {
            throw new Exception();
        }

        var condition = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            throw new Exception();
        }

        var body = parser.ConsumeSimpleBlock();

        if (parser.Tokenizer.Peek() == "else") {
            var elseNode = new ElseParselet().Parse(parser, parser.Tokenizer.Consume());

            return new IfNode(condition, body, elseNode, ifKeyword);
        }

        return new IfNode(condition, body, ifKeyword);
    }

    // TODO: Later
    /*static ElseNode[] FlattenElseChain(ElseNode elseNode) {
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new[] { elseNode };

        return (new[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}