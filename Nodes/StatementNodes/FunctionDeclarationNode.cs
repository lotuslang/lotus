using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    internal bool isInternal = false;

    public SimpleBlock Value { get; }

    public ReadOnlyCollection<ComplexToken> Parameters { get; }

    public ComplexToken Name { get; }

    public FunctionDeclarationNode(SimpleBlock value,
                                   IList<ComplexToken> parameters,
                                   ComplexToken functionName,
                                   ComplexToken defToken)
        : base(defToken)
    {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (declaration)");

        Name = functionName;
        Value = value;
        Parameters = parameters.AsReadOnly();
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "def " + Name.Representation)
            .SetColor("indianred")
            .SetTooltip(nameof(FunctionDeclarationNode));

        if (Parameters.Count == 0) {
            root.Add(new GraphNode(Parameters.GetHashCode(), "(no params)"));
        } else {
            var parameters = new GraphNode(Parameters.GetHashCode(), "param")
                .SetTooltip("parameters");

            foreach (var parameter in Parameters) {
                parameters.Add(parameter.ToGraphNode("parameter"));
            }

            root.Add(parameters);
        }

        root.Add(Value.ToGraphNode().SetTooltip("body"));

        return root;
    }
}
