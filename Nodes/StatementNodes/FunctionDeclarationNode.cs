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
        var root = new GraphNode(GetHashCode(), "def " + Name.Representation);

        root.AddProperty("color", "indianred");
        root.AddProperty("tooltip", nameof(FunctionDeclarationNode));

        if (Parameters.Count == 0) {
            root.AddNode(new GraphNode(Parameters.GetHashCode(), "(no params)"));

        } else {

            var parameters = new GraphNode(Parameters.GetHashCode(), "param");

            parameters.AddProperty("tooltip", "parameters");

            foreach (var parameter in Parameters) {
                parameters.AddNode(parameter.ToGraphNode("parameter"));
            }

            root.AddNode(parameters);
        }

        var body = Value.ToGraphNode();

        body.AddProperty("tooltip", "body");

        root.AddNode(body);

        return root;
    }
}
