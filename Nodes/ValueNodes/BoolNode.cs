
public record BoolNode(BoolToken Token, bool IsValid = true) : ValueNode(Token, IsValid)
{
    public new static readonly BoolNode NULL = new(BoolToken.NULL, false);

    public bool Value => Token.Value;

    public new BoolToken Token { get => (base.Token as BoolToken)!; init => base.Token = value; }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
