public record StringNode(StringToken Token)
: ValueNode(Token, Token.IsValid)
{
    public new StringToken Token { get => (base.Token as StringToken)!; init => base.Token = value; }

    public string Value => Token.Representation;

    protected StringNode(StringToken token, bool isValid) : this(token) {
        IsValid = isValid;
    }

    public new static readonly StringNode NULL = new(StringToken.NULL);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
