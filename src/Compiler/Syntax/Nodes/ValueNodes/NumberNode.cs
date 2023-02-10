namespace Lotus.Syntax;

public sealed record NumberNode(NumberToken Token)
: ValueNode(Token, Token.IsValid)
{
    public new static readonly NumberNode NULL = new(NumberToken.NULL);

    public new NumberToken Token { get => (base.Token as NumberToken)!; init => base.Token = value; }

    public object Value => Token.Value;

    public NumberKind Kind => Token.NumberKind;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}