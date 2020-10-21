public class StatementNode
{
    public Token Token { get; protected set; }

    public LocationRange Location { get; set; }

    public string Representation { get; protected set; }

    public bool IsValid { get; set; }

    public static readonly StatementNode NULL = new StatementNode("", new Token('\0', TokenKind.EOF, new Location(-1, -1), false), new Location(-1, -1), false);

    public StatementNode(string representation, Token token, LocationRange range, bool isValid = true) {
        Representation = representation;
        Token = token;
        Location = range;
        IsValid = isValid;
    }

    public StatementNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    public virtual T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
