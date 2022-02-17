public abstract record Node : ILocalized {
    public static readonly Node NULL = ValueNode.NULL;

    public Token Token { get; init; }
    public LocationRange Location { get; init; }
    public bool IsValid { get; set; } = true;

    public Node(Token token, LocationRange location, bool isValid = true) {
        Token = token;
        Location = location;
        IsValid = isValid;
    }

    protected Node(Token token, bool isValid = true) : this(token, token.Location, isValid) { }
}