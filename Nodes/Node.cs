public abstract class Node {
    public Token Token { get; protected set; }

    public LocationRange Location { get; set; }

    public static readonly Node NULL = ValueNode.NULL;

    protected Node(Token token) : this(token, token.Location) { }
    protected Node(Token token, LocationRange location) {
        Token = token;
        Location = location;
    }
}