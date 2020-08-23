using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    internal bool isInternal = false;

    public bool HasReturnType => ReturnType != ValueNode.NULL;

    public SimpleBlock Value { get; }

    public ReadOnlyCollection<FunctionParameter> Parameters { get; }

    public ValueNode ReturnType { get; }

    public IdentToken Name { get; }

    public FunctionDeclarationNode(SimpleBlock value,
                                   IList<FunctionParameter> parameters,
                                   ValueNode returnType,
                                   IdentToken functionName,
                                   ComplexToken funcKeyword,
                                   bool isValid = true)
        : base(funcKeyword, isValid)
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

            foreach (var parameter in Parameters) { // FIXME: Write tooltips

                var paramNameNode = parameter.Name.ToGraphNode();

                if (parameter.Type == ValueNode.NULL) {
                    paramNameNode.Add(new GraphNode(HashCode.Combine(ValueNode.NULL, parameter), "any"));
                } else {
                    paramNameNode.Add(parameter.Type.ToGraphNode());
                }

                if (parameter.HasDefaultValue) {
                    paramNameNode.Add(parameter.DefaultValue.ToGraphNode());
                }

                parametersNode.Add(paramNameNode);
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

public class FunctionParameter
{
    public ValueNode Type { get; }

    public IdentNode Name { get; }

    public ValueNode DefaultValue { get; }

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;

    public bool IsValid { get; set; }

    public FunctionParameter(ValueNode type, IdentNode name, ValueNode defaultValue, bool isValid = true) {
        Type = type;
        Name = name;
        DefaultValue = defaultValue;
        IsValid = isValid;
    }
}