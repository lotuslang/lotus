namespace Lotus.Syntax;

public sealed record ParenthesizedValueNode(ValueNode Value, Token Token, Token ClosingToken)
: ValueNode(Token, new LocationRange(Token, ClosingToken))
{
    public new static readonly ParenthesizedValueNode NULL = new(ValueNode.NULL, Token.NULL, Token.NULL) { IsValid = false };

    public Token OpeningToken => Token;

    public TupleNode AsTupleNode()
        => new(ImmutableArray.Create(Value), Token, ClosingToken) { IsValid = IsValid };

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}