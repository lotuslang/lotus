public class StatementNode
{
    public Token Token { get; protected set; }

    public string Representation { get; protected set; }

    public bool IsValid { get; set; }

    public static readonly StatementNode NULL = new StatementNode("", new Token('\0', TokenKind.EOF, new Location(-1, -1), false), false);

    public StatementNode(string representation, Token token, bool isValid = true) {
        Representation = representation;
        Token = token;

        IsValid = isValid;
    }

    public StatementNode(Token token, bool isValid = true) : this(token.Representation, token, isValid)
    { }

    /// <summary>
    /// Uses the node's state to turn it into a GraphNode.
    ///
    /// Derived classes each differ in how they do it,
    /// and this method should be implemented by them.
    /// </summary>
    /// <returns>A GraphNode representing the state of the node</returns>
    public virtual GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), Representation)
            .SetColor("black")
            .SetTooltip(GetType().Name);
}
