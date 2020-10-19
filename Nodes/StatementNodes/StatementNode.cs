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

    public virtual T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
