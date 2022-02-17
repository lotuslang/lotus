public sealed class IfParslet : IStatementParslet<IfNode>
{

    private static IfParslet _instance = new();
    public static IfParslet Instance => _instance;

	private IfParslet() : base() { }

    public IfNode Parse(StatementParser parser, Token ifToken) {
        if (ifToken is not Token ifKeyword || ifKeyword != "if") {
            throw Logger.Fatal(new InvalidCallError(ErrorArea.Parser, ifToken.Location));
        }

        var isValid = true;

        var conditionNode = parser.ExpressionParser.Consume();

        if (conditionNode is not ParenthesizedValueNode condition) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = conditionNode,
                As = "an if-statement condition",
                Expected = "a condition between parenthesis (e.g. `(a == b)`)"
            });

            isValid = false;

            if (conditionNode is TupleNode tuple) {
                Logger.errorStack.Pop();

                Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                    Value = conditionNode,
                    Message = "You can't use a tuple as a condition. ",
                    ExtraNotes = "If you want to combine multiple conditions, "
                            +"you can use the logical operators (OR ||, AND &&, XOR ^^, etc...)"
                });

                condition = new ParenthesizedValueNode(
                    tuple.Count == 0 ? ValueNode.NULL : tuple.Values[0],
                    tuple.OpeningToken,
                    tuple.ClosingToken,
                    isValid: false
                );
            } else {
                condition = new ParenthesizedValueNode(conditionNode, Token.NULL, Token.NULL, isValid: false);
            }
        }

        var body = parser.ConsumeSimpleBlock();

        var elseNode = ElseNode.NULL;

        if (parser.Tokenizer.Peek() == "else") {
            elseNode = ElseParslet.Instance.Parse(parser, parser.Tokenizer.Consume());
        }

        return new IfNode(condition, body, elseNode, ifKeyword, isValid);
    }

    // TODO: Later
    /*static ElseNode[] FlattenElseChain(ElseNode elseNode) {
        if (!elseNode.HasIf || !elseNode.IfNode.HasElse) return new[] { elseNode };

        return (new[] { elseNode }).Concat(FlattenElseChain(elseNode.IfNode.ElseNode)).ToArray();
    }*/
}