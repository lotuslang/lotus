public abstract class Node {
    public Token Token { get; protected set; }

    public LocationRange Location => Token.Location;

    public static readonly Node NULL = ValueNode.NULL;

    protected Node(Token token) { Token = token; }
}