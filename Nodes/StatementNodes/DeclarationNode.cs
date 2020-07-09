using System;

/// <summary>
/// Represents a variable declaration statement (var a = b)
/// </summary>
public class DeclarationNode : StatementNode
{
    /// <summary>
    /// The value the variable is being initialized to
    /// </summary>
    public ValueNode Value { get; protected set; }

    /// <summary>
    /// The name of the variable being declared
    /// </summary>
    /// <value></value>
    public IdentToken Name { get; protected set; }

    /// <summary>
    /// Creates a DeclarationNode.
    /// </summary>
    /// <param name="value">The value of the variable</param>
    /// <param name="varName">The name of the variable</param>
    /// <param name="varKeywordToken">The token of the "var" keyword used</param>
    /// <returns></returns>
    public DeclarationNode(ValueNode value, IdentToken varName, ComplexToken varKeywordToken) : base(varKeywordToken) {
        Name = varName;
        Value = value;
    }

    public override GraphNode ToGraphNode()
        => new GraphNode(GetHashCode(), "var") {
            Name.ToGraphNode(tooltip: "name"),
            Value.ToGraphNode()
        }.SetColor("palegreen")
         .SetTooltip(nameof(DeclarationNode));
}
