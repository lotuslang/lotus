public record NumberNode(NumberToken Token)
: ValueNode(Token, Token.IsValid)
{
    public new static readonly NumberNode NULL = new(NumberToken.NULL);

    public new NumberToken Token { get => (base.Token as NumberToken)!; init => base.Token = value; }

    public double Value => Token.Value;

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}