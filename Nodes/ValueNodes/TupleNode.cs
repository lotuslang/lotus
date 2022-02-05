
public record TupleNode(IList<ValueNode> Values, Token Token, Token ClosingToken, bool IsValid = true)
: ValueNode(Token, new LocationRange(Token.Location, ClosingToken.Location), IsValid)
{
    public new static readonly TupleNode NULL = new(Array.Empty<ValueNode>(), Token.NULL, Token.NULL, false);

    public Token OpeningToken { get => Token; init => Token = value; }

    public int Count => Values.Count;

    /// <summary>
    /// <strong>TRUNCATES</strong> the tuple to the first element and turns it into a paren expression.
    /// </summary>
    public ParenthesizedValueNode AsParenthesized()
        => new(
            Values[0],
            OpeningToken,
            ClosingToken,
            IsValid
        );

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}