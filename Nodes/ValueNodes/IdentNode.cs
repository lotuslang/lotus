public record IdentNode(IdentToken Token, bool IsValid = true)
: ValueNode(Token, IsValid)
{
    public new static readonly IdentNode NULL = new(IdentToken.NULL, false);

    public new IdentToken Token { get => (base.Token as IdentToken)!; init => base.Token = value; }

    public string Value => Token.Representation;

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
