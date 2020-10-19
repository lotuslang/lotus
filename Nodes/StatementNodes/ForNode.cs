using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Represents a for-loop statement
/// </summary>
public class ForNode : StatementNode
{
    public ReadOnlyCollection<StatementNode> Header { get; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    public SimpleBlock Body { get; }

    public ForNode(ComplexToken forToken, IList<StatementNode> header, SimpleBlock body, Token openingParen, Token closingParen, bool isValid = true)
        : base(forToken, isValid)
    {
        OpeningParenthesis = openingParen;
        ClosingParenthesis = closingParen;
        Header = header.AsReadOnly();
        Body = body;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "for loop"); // FIXME: choose color and tooltip

        if (Header.Count != 0) {
            var headerNode = new GraphNode(Header.GetHashCode(), "header")
                .SetColor("deepskyblue")
                .SetTooltip("for-loop header");

            foreach (var statement in Header) headerNode.Add(statement.ToGraphNode());

            root.Add(headerNode);

        } else {
            root.Add(new GraphNode(Header.GetHashCode(), "(empty header)"));
        }

        root.Add(Body.ToGraphNode());

        return root;
    }
}
