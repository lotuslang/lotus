public sealed class DotAccessParslet : IInfixParslet<ValueNode>
{
    public Precedence Precedence => Precedence.Access;

    public static readonly DotAccessParslet Instance = new();

    private static readonly BinaryOperatorParslet _opParslet = new(Precedence.Access, OperationType.Access);

    public ValueNode Parse(ExpressionParser parser, Token dotToken, ValueNode left) {
        var dotOpToken = dotToken as OperatorToken;

        Debug.Assert(dotOpToken is not null);
        Debug.Assert(dotOpToken.Representation == ".");

        if (left is not NameNode leftName) {
            if (left is not IdentNode leftIdent) {
                return _opParslet.Parse(parser, dotToken, left);
            }

            leftName = leftIdent;
        }

        // Since '.' is left-associative, the right part is always a node with higher precedence than us,
        // and right now that means that they'll always be leaf-nodes, since array access are right-associative.
        //
        // Therefore, we only need to check that the right part is an identifier, since that's the only
        // value that could follow a name and a dot
        var rightPart = parser.Consume(Precedence.Access);

        if (rightPart is not IdentNode ident) {
            Logger.Error(new UnexpectedError<ValueNode>(ErrorArea.Parser) {
                Value = rightPart,
                As = "a member name",
                Expected = "an identifier",
            });

            return new OperationNode(dotOpToken, new[] { leftName, rightPart }, OperationType.Access) { IsValid = false };
        }

        return new FullNameNode(leftName, ident, dotOpToken);
    }
}