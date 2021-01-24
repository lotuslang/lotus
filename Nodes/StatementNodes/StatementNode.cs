public class StatementNode
{
    public static readonly StatementNode NULL = new StatementNode("", Token.NULL, LocationRange.NULL, false);

    public Token Token { get; protected set; }

    public LocationRange Location { get; set; }

    public string Representation { get; protected set; }

    public bool IsValid { get; set; }

    public StatementNode(string representation, Token token, LocationRange range, bool isValid = true) {
        Representation = representation;
        Token = token;
        Location = range;
        IsValid = isValid;
    }

    public StatementNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
