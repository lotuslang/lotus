using System;

public class TernaryOperatorParselet : IInfixParselet<OperationNode>
{
    public Precedence Precedence => Precedence.Ternary;

    public OperationNode Parse(Parser parser, Token questionMarkToken, ValueNode condition) {
        if (!(questionMarkToken is OperatorToken questionMarkOpToken && questionMarkOpToken == "?")) {
            throw new Exception();
        }

        var firstValue = parser.ConsumeValue();

        if (parser.Tokenizer.Consume() != ":") {
            throw new Exception();
        }

        var secondValue = parser.ConsumeValue();

        return new OperationNode(questionMarkOpToken, new[] {condition, firstValue, secondValue}, OperationType.Conditional);
    }
}