public class ContinueNode : StatementNode
{
    public ContinueNode(ComplexToken continueToken, bool isValid = true) : base(continueToken, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}