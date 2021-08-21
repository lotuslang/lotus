using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ComplexStringNode : StringNode
{
    public new static readonly ComplexStringNode NULL = new(ComplexStringToken.NULL, Array.Empty<ValueNode>(), false);

    protected List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => sections.AsReadOnly();
    }

    public ComplexStringNode(ComplexStringToken token, IList<ValueNode> codeSections, bool isValid = true)
        : base(token.Representation, token, isValid)
    {
        sections = new List<ValueNode>(codeSections);
    }

    public void AddSection(ValueNode section)
        => sections.Add(section);

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
