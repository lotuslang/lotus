public sealed record ComplexStringNode : StringNode
{
    public new static readonly ComplexStringNode NULL = new(ComplexStringToken.NULL, ImmutableArray<ValueNode>.Empty, false);

    public ImmutableArray<ValueNode> CodeSections;

    public ComplexStringNode(ComplexStringToken token, ImmutableArray<ValueNode> codeSections, bool isValid = true)
        : base(token)
    {
        IsValid = isValid;
        CodeSections = codeSections;
    }

    public void AddSection(ValueNode section)
        => CodeSections = CodeSections.Add(section);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
