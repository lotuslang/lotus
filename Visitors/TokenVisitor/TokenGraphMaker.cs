public sealed class TokenGraphMaker : TokenVisitor<GraphNode>
{
    protected override GraphNode Default(Token token) => Visit(token);

    protected override GraphNode Default(TriviaToken token) => null!;


    public override GraphNode Visit(Token token) {
        var output = new GraphNode(token.GetHashCode(), token.Representation);

        output.SetColor("lightgrey");

        return output;
    }

    //TODO: this
    //public override GraphNode Visit(ComplexStringToken token) { }

    public GraphNode ToGraphNode(Token token) => token.Accept(this);
}