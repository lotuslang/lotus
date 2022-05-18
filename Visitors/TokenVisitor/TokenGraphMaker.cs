internal sealed class TokenGraphMaker : ITokenVisitor<GraphNode>
{
    public GraphNode Default(Token token)
        =>  new GraphNode(token.GetHashCode(), token.Representation)
                .SetColor("lightgrey");

    public GraphNode Default(TriviaToken? token) => Default(token is null ? Token.NULL : token as Token);

    public GraphNode ToGraphNode(Token token) => token.Accept(this);
}