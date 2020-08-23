[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static new readonly ValueNode NULL = new ValueNode("", new Token('\0', TokenKind.EOF, new Location(), false), false);

    public ValueNode(Token token, bool isValid = true) : this(token.Representation, token, isValid)
    { }

    public ValueNode(string rep, Token token, bool isValid = true) : base(rep, token, isValid)
    { }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), Representation)
            .SetColor("lightgrey")
            .SetTooltip(GetType().Name);
}