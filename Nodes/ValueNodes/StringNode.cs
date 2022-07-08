public record StringNode(StringToken Token, bool IsValid = true)
: ValueNode(Token, IsValid)
{
    public new StringToken Token { get => (base.Token as StringToken)!; init => base.Token = value; }

    public string Value => Token.Representation;

    public new static readonly StringNode NULL = new(StringToken.NULL, false);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
