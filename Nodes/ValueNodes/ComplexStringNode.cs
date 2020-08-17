using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ComplexStringNode : StringNode
{
    protected List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => sections.AsReadOnly();
    }

    public ComplexStringNode(ComplexStringToken token, IList<ValueNode> codeSections, bool isValid = true)
        : base(token.Representation, token, isValid)
    {
        sections = new List<ValueNode>(codeSections);
    }

    public void AddSection(ValueNode section) {
        sections.Add(section);
    }
}
