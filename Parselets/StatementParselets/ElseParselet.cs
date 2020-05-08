using System;
using System.Collections.Generic;

public class ElseParselet : IStatementParselet<ElseNode>
{
    public ElseNode Parse(Parser parser, Token elseToken) {
        if (!(elseToken is ComplexToken elseKeyword && elseKeyword == "else")) {
            throw new Exception();
        }

        if (parser.Tokenizer.Peek() == "if") {
            var ifNode = new IfParselet().Parse(parser, parser.Tokenizer.Consume());

            return new ElseNode(ifNode, elseKeyword);
        }

        return new ElseNode(parser.ConsumeSimpleBlock(), elseKeyword);
    }
}