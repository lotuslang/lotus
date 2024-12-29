using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record StringNode(StringToken Token)
    : ValueNode(Token)
{
    public new StringToken Token { get => Unsafe.As<StringToken>(base.Token); init => base.Token = value; }

    public string Value => Token.Representation;

    public new static readonly StringNode NULL = new(StringToken.NULL);

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}
