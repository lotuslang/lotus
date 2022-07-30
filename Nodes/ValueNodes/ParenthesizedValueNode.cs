public sealed record ParenthesizedValueNode(ValueNode Value, Token Token, Token ClosingToken, bool IsValid = true)
: ValueNode(Token, new LocationRange(Token, ClosingToken), IsValid)
{
    public new static readonly ParenthesizedValueNode NULL = new(ValueNode.NULL, Token.NULL, Token.NULL, false);

    public Token OpeningToken => Token;

    public TupleNode AsTupleNode()
        => new(new[] { Value }, Token, ClosingToken, IsValid);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}