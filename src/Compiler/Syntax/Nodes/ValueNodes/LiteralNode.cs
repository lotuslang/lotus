using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public abstract record LiteralNode(Token Token) : ValueNode(Token, Token.IsValid)
{
    public abstract object Value { get; }

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);

}