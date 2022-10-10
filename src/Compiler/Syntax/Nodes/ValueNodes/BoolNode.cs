namespace Lotus.Syntax;

public sealed record BoolNode(BoolToken Token) : ValueNode(Token, Token.IsValid)
{
    public new static readonly BoolNode NULL = new(BoolToken.NULL);

    public bool Value => Token.Value;

    public new BoolToken Token { get => (base.Token as BoolToken)!; init => base.Token = value; }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}
