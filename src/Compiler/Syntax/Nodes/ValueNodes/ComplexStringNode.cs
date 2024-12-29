using System.Runtime.CompilerServices;

namespace Lotus.Syntax;

public sealed record ComplexStringNode(
    ComplexStringToken Token,
    ImmutableArray<ValueNode> CodeSections
) : ValueNode(Token)
{
    public new static readonly ComplexStringNode NULL = new(ComplexStringToken.NULL, []) { IsValid = false };

    public new ComplexStringToken Token { get => Unsafe.As<ComplexStringToken>(base.Token); init => base.Token = value; }

    public ImmutableArray<string> TextSections => Token.TextSections;

    [DebuggerHidden]
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}
