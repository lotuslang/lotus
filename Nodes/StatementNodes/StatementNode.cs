public class StatementNode : Node
{
    public new static readonly StatementNode NULL = new StatementNode("", Token.NULL, LocationRange.NULL, false);

    public string Representation { get; protected set; }


    public StatementNode(string representation, Token token, LocationRange range, bool isValid = true)
        : base(token, range, isValid)
    {
        Representation = representation;
    }

    public StatementNode(Token token, LocationRange range, bool isValid = true) : this(token.Representation, token, range, isValid)
    { }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public virtual T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);

    public static explicit operator TopLevelStatementNode(StatementNode node) => new TopLevelStatementNode(node);

    public static explicit operator TopLevelNode(StatementNode node) => (TopLevelStatementNode)node;
}
