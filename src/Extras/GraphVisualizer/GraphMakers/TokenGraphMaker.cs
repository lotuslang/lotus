using Lotus.Syntax.Visitors;

namespace Lotus.Extras.Graphs;

internal sealed partial class GraphMaker : ITokenVisitor<GraphNode>
{
    public GraphNode Default(Token token)
        =>  new GraphNode(token.Representation)
                .SetColor("lightgrey");

    public GraphNode Visit(Token token)
        => token.Kind == TokenKind.EOF ? new GraphNode("<EOF>") : Default(token);

    public GraphNode Default(TriviaToken? token) => Default(token ?? Token.NULL);

    public GraphNode Visit(NumberToken token)
        => Default(token).SetTooltip(token.NumberKind.ToString());

    public GraphNode ToGraphNode(Token token) => token.Accept(this);
}