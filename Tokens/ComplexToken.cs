public class ComplexToken : Token
{
    public ComplexToken(string representation, TokenKind kind, Location location, bool isValid = true, TriviaToken? leading = null, TriviaToken? trailing = null)
        : base(representation, kind, location, isValid, leading, trailing) { }

    public virtual void Add(char ch)
        => rep += ch;

    public virtual void Add(string str)
        => rep += str;

    public GraphNode ToGraphNode() {
        var output = new GraphNode(GetHashCode(), Representation);

        output.SetColor("lightgrey");

        return output;
    }

    public GraphNode ToGraphNode(string tooltip)
        => ToGraphNode().SetTooltip(tooltip);
}