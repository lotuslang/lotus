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
    public ComplexToken Name { get; protected set; }

    /// <summary>
    /// Creates a DeclarationNode.
    /// </summary>
    /// <param name="value">The value of the variable</param>
    /// <param name="varName">The name of the variable</param>
    /// <param name="varKeywordToken">The token of the "var" keyword used</param>
    /// <returns></returns>
    public DeclarationNode(ValueNode value, ComplexToken varName, ComplexToken varKeywordToken) : base(varKeywordToken) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        Name = varName;
        Value = value;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "var");

        root.AddProperty("color", "palegreen");
        root.AddProperty("tooltip", nameof(DeclarationNode));

        root.AddNode(Name.ToGraphNode("name"));

        root.AddNode(Value.ToGraphNode());

        return root;
    }
}
