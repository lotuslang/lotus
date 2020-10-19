using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SimpleBlock
{
    public ReadOnlyCollection<StatementNode> Content { get; }

    public Location Location { get; }

    public bool IsValid { get; set; }

    public bool IsOneLiner { get; }

    public Token OpeningToken { get; }

    public Token ClosingToken { get; }

    public SimpleBlock(IList<StatementNode> content, Location location, Token openingToken, Token closingToken, bool isValid = true) {
        Content = content.AsReadOnly();
        Location = location;
        OpeningToken = openingToken;
        ClosingToken = closingToken;
        IsValid = isValid;
        IsOneLiner = false;
    }

    public SimpleBlock(StatementNode content, Location location, bool isValid = true)
        : this(new[] { content }, location, Token.NULL, Token.NULL, isValid)
    {
        IsOneLiner = true;
    }

    public GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "block")
            .SetColor("darkviolet")
            .SetTooltip(nameof(SimpleBlock));

        foreach (var statement in Content) root.Add(statement.ToGraphNode());

        return root;
    }
}