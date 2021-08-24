public abstract record Node(Token Token, LocationRange Location, bool IsValid = true) {
    public static readonly Node NULL = ValueNode.NULL;

    protected Node(Token token, bool isValid = true) : this(token, token.Location, isValid) { }
}