using System.Collections.ObjectModel;

public sealed record ComplexStringNode : StringNode
{
    public new static readonly ComplexStringNode NULL = new(ComplexStringToken.NULL, Array.Empty<ValueNode>(), false);

    private List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => sections.AsReadOnly();
    }

    public ComplexStringNode(ComplexStringToken token, IList<ValueNode> codeSections, bool isValid = true)
        : base(token)
    {
        IsValid = isValid;
        sections = new List<ValueNode>(codeSections);
    }

    public void AddSection(ValueNode section)
        => sections.Add(section);

    [DebuggerHidden()]
    [DebuggerStepThrough()]
    [DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
