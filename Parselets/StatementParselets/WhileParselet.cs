using System;

public class WhileParselet : IStatementParselet<WhileNode>
{
    public WhileNode Parse(Parser parser, Token whileToken) {
        if (!(whileToken is ComplexToken whileKeyword && whileKeyword == "while")) {
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

        var body = parser.ConsumeSimpleBlock();

        return new WhileNode(condition, body, whileKeyword);
    }
}