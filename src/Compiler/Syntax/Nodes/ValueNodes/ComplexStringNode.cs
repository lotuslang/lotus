namespace Lotus.Syntax;

public sealed record ComplexStringNode : StringNode
{
    public new static readonly ComplexStringNode NULL = new(ComplexStringToken.NULL, ImmutableArray<ValueNode>.Empty) { IsValid = false };

    public ImmutableArray<ValueNode> CodeSections;

    // can't be moved to normal record decl because StringToken.Token's type is incompatible
    public ComplexStringNode(ComplexStringToken token, ImmutableArray<ValueNode> codeSections)
        : base(token)
    {
        CodeSections = codeSections;
    }

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(Visitors.IValueVisitor<T> visitor) => visitor.Visit(this);
}
