[System.Diagnostics.DebuggerDisplay("{Token.Representation}")] // TODO: Write debugger displays for every data class
public abstract class Node {
    public Token Token { get; protected set; }

    public LocationRange Location { get; set; }

    public bool IsValid = false;

    public static readonly Node NULL = ValueNode.NULL;

    protected Node(Token token, bool isValid = true) : this(token, token.Location, isValid) { }
    protected Node(Token token, LocationRange location, bool isValid = true) {
        Token = token;
        Location = location;
        IsValid = isValid;
    }
}