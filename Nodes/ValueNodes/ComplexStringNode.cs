using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ComplexStringNode : StringNode
{
    protected List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => sections.AsReadOnly();
    }

    public ComplexStringNode(ComplexStringToken token, IList<ValueNode> codeSections) : base(token.Representation, token) {
        sections = new List<ValueNode>(codeSections);
    }

    public void AddSection(ValueNode section) {
        sections.Add(section);
    }
}
