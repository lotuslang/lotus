public class StatementNode
{
    public Token Token { get; protected set; }

    public string Representation { get; protected set; }

    public static readonly StatementNode NULL = new StatementNode("", new Token('\0', TokenKind.EOF, default(Location)));

    public StatementNode(string representation, Token token) {
        Representation = representation;
        Token = token;
    }

    public StatementNode(Token token) : this(token.Representation, token)
    { }

    /// <summary>
    /// Uses the node's state to turn it into a GraphNode.
    ///
    /// Derived classes each differ in how they do it,
    /// and this method should be implemented by them.
    /// </summary>
    /// <returns>A GraphNode representing the state of the node</returns>
    public virtual GraphNode ToGraphNode() {
        var output = new GraphNode(GetHashCode(), Representation);

        output.AddProperty("color", "black");
        output.AddProperty("tooltip", nameof(StatementNode));

        return output;
    }
}
