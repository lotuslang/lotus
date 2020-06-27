public class ComplexToken : Token
{
    public ComplexToken(string representation, TokenKind kind, Location location, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, kind, location, leading, trailing) { }

    public void Add(char ch)
        => rep.Append(ch);

    public void Add(string str)
        => rep.Append(str);

    public GraphNode ToGraphNode() {
        var output = new GraphNode(GetHashCode(), Representation);

        output.SetColor("lightgrey");

        return output;
    }

    public GraphNode ToGraphNode(string tooltip)
        => ToGraphNode().SetTooltip(tooltip);
}