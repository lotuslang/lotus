using System;

public class DoWhileParselet : IStatementParselet<WhileNode>
{
    public WhileNode Parse(Parser parser, Token doToken) {
        if (!(doToken is ComplexToken doKeyword && doKeyword == "do")) {
            throw new Exception();
        }

        var body = parser.ConsumeSimpleBlock();

        if (!(parser.Tokenizer.Consume() is ComplexToken whileKeyword && whileKeyword == "while")) {
            throw new Exception();
        }

        // Wondering why do we do that ? See note in IfParselet.cs
        if (parser.Tokenizer.Consume() != "(") {
            throw new Exception();
        }

        var condition = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ")") {
            throw new Exception();
        }

        return new WhileNode(condition, body, whileKeyword, doKeyword);
    }
}