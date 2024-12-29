using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record CharNode(CharToken Token) : LiteralNode(Token)
{
    public new static readonly CharNode NULL = new(CharToken.NULL);

    public override object Value => Token.Value;
    public char RawValue => Token.Value;

    public new CharToken Token { get => Unsafe.As<CharToken>(base.Token); init => base.Token = value; }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}