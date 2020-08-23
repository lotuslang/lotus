using System;
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
    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), Representation);

        if (CodeSections.Count != 0) {
            var sectionNode = new GraphNode(HashCode.Combine(this, "sections"), "code sections");

            foreach (var section in CodeSections) {
                sectionNode.Add(section.ToGraphNode());
            }

            root.Add(sectionNode);
        }

        return root;
    }
}
