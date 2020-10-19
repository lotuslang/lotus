using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    internal bool isInternal = false;

    public bool HasReturnType => ReturnType != ValueNode.NULL;

    public SimpleBlock Body { get; }

    public ReadOnlyCollection<FunctionParameter> Parameters { get; }

    public ValueNode ReturnType { get; }

    public IdentToken Name { get; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    public Token ColonToken { get; }

    public FunctionDeclarationNode(SimpleBlock body,
                                   IList<FunctionParameter> parameters,
                                   ValueNode returnType,
                                   IdentToken functionName,
                                   ComplexToken funcKeyword,
                                   Token openingParen,
                                   Token closingParen,
                                   Token colonToken,
                                   bool isValid = true)
        : base(funcKeyword, isValid)
    {
        Name = functionName;
        Body = body;
        Parameters = parameters.AsReadOnly();
        ReturnType = returnType;
        OpeningParenthesis = openingParen;
        ClosingParenthesis = closingParen;
        ColonToken = colonToken;
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

        root.Add(Body.ToGraphNode().SetTooltip("body"));

        return root;
    }
}

public class FunctionParameter
{
    public ValueNode Type { get; }

    public IdentNode Name { get; }

    public Token EqualSign { get; }

    public ValueNode DefaultValue { get; }

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;

    public bool IsValid { get; set; }

    public FunctionParameter(ValueNode type, IdentNode name, ValueNode defaultValue, Token equalSign, bool isValid = true) {
        Type = type;
        Name = name;
        EqualSign = equalSign;
        DefaultValue = defaultValue;
        IsValid = isValid;
    }
}