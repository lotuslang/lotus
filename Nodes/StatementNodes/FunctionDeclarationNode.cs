using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    internal bool isInternal = false;

    public bool HasReturnType => ReturnType != ValueNode.NULL;

    public SimpleBlock Value { get; }

    public ReadOnlyCollection<(ValueNode type, ComplexToken name)> Parameters { get; }

    public ValueNode ReturnType { get; }

    public IdentToken Name { get; }

    public FunctionDeclarationNode(SimpleBlock value,
                                   IList<(ValueNode, ComplexToken)> parameters,
                                   ValueNode returnType,
                                   IdentToken functionName,
                                   ComplexToken funcKeyword)
        : base(funcKeyword)
    {
        Name = functionName;
        Value = value;
        Parameters = parameters.AsReadOnly();
        ReturnType = returnType;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "func " + Name.Representation)
            .SetColor("indianred")
            .SetTooltip(nameof(FunctionDeclarationNode));

        if (Parameters.Count == 0) {
            root.Add(new GraphNode(Parameters.GetHashCode(), "(no params)"));
        } else {
            var parametersNode = new GraphNode(Parameters.GetHashCode(), "param")
                .SetTooltip("parameters");

            foreach (var (type, name) in Parameters) { // FIXME: Write tooltips
                if (type == ValueNode.NULL) {
                    parametersNode.Add(name.ToGraphNode(tooltip: "parameter name"));

                    continue;
                }

                if (type is IdentNode) {
                    parametersNode.Add(new GraphNode(HashCode.Combine(type, name), name + "\\nof type " + type.Token));

                    continue;
                }

                parametersNode.Add(new GraphNode(name.GetHashCode(), name) {
                    new GraphNode(HashCode.Combine("type", name), "type") {
                        type.ToGraphNode()
                    }
                });
            }

            root.Add(parametersNode);
        }

        if (HasReturnType) {
            root.Add(new GraphNode(HashCode.Combine(ReturnType, this), "return type") { // FIXME: Color & Tooltip
                ReturnType.ToGraphNode()
            });
        }

        root.Add(Value.ToGraphNode().SetTooltip("body"));

        return root;
    }
}
