[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static new readonly ValueNode NULL = new ValueNode("", new Token('\0', TokenKind.EOF, default(Location)));

    public ValueNode(Token token) : this(token.Representation, token)
    { }

    public ValueNode(string rep, Token token) : base(rep, token)
    { }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), Representation)
            .SetColor("lightgrey")
            .SetTooltip(nameof(ValueNode));
}