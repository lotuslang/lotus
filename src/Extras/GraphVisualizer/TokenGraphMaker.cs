using Lotus.Syntax.Visitors;

namespace Lotus.Extras.Graphs;

internal sealed class TokenGraphMaker : ITokenVisitor<GraphNode>
{
    public GraphNode Default(Token token)
        =>  new GraphNode(token.GetHashCode(), token.Representation)
                .SetColor("lightgrey");

    public GraphNode Default(TriviaToken? token) => Default(token ?? Token.NULL);

    public GraphNode ToGraphNode(Token token) => token.Accept(this);
}