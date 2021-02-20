public class TopLevelStatementNode : TopLevelNode
{
    public StatementNode Node { get; }
    public TopLevelStatementNode(StatementNode node) : base(node.Token) {
        Node = node;
    }
}