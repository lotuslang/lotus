namespace Lotus.Syntax;

[DebuggerDisplay("[{GetType().Name}]: {Token.Representation}")]
public abstract record Node : ILocalized
{
    public static readonly Node NULL = ValueNode.NULL;

    public Token Token { get; init; }
    public LocationRange Location { get; init; }
    public bool IsValid { get; set; } = true;

    public Node(Token token, LocationRange location) {
        Token = token;
        Location = location;
    }

    protected Node(Token token) : this(token, token.Location) { }
}