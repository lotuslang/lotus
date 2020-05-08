using System;
using System.Collections.Generic;

public class IfParselet : IStatementParselet<IfNode>
{
    public IfNode Parse(Parser parser, Token ifToken) {
        if (!(ifToken is ComplexToken ifKeyword && ifKeyword == "if")) {
            throw new Exception();
        }

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
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new ElseNode[] { elseNode };

        return (new ElseNode[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}