public sealed class IfParselet : IStatementParselet<IfNode>
{
    public IfNode Parse(StatementParser parser, Token ifToken) {
        if (!(ifToken is ComplexToken ifKeyword && ifKeyword == "if")) {
            throw Logger.Fatal(new InvalidCallException(ifToken.Location));
        }

        var isValid = true;

        var conditionNode = parser.ExpressionParser.ConsumeValue();

        if (!(conditionNode is ParenthesizedValueNode condition)) {
            Logger.Error(new UnexpectedValueTypeException(
                node: conditionNode,
                context: "as an if-statement condition",
                expected: "a condition between parenthesis (e.g. `(a == b)`)"
            ));

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                condition = new ParenthesizedValueNode(
                    tuple.Count == 0 ? ValueNode.NULL : tuple.Values[0],
                    tuple.OpeningToken,
                    tuple.ClosingToken
                );
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL);
            }
        }

        var body = parser.ConsumeSimpleBlock();

        if (parser.Tokenizer.Peek() == "else") {
            var elseNode = new ElseParselet().Parse(parser, parser.Tokenizer.Consume());

            return new IfNode(condition, body, elseNode, ifKeyword);
        }

        return new IfNode(condition, body, ifKeyword, isValid);
    }

    // TODO: Later
    /*static ElseNode[] FlattenElseChain(ElseNode elseNode) {
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new[] { elseNode };

        return (new[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}