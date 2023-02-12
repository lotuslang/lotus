using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record CharNode(CharToken Token) : ValueNode(Token, Token.IsValid)
{
    public new static readonly CharNode NULL = new(CharToken.NULL);

    public new CharToken Token { get => Unsafe.As<CharToken>(base.Token); init => base.Token = value; }

    public char Value => Token.Value;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}