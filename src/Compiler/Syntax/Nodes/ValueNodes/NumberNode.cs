using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record NumberNode(NumberToken Token)
: ValueNode(Token, Token.IsValid)
{
    public new static readonly NumberNode NULL = new(NumberToken.NULL);

    public new NumberToken Token { get => Unsafe.As<NumberToken>(base.Token); init => base.Token = value; }

    public object Value => Token.Value;

    public NumberKind Kind => Token.NumberKind;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}