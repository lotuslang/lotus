public class ContinueNode : StatementNode
{
    public ContinueNode(ComplexToken continueToken, bool isValid = true) : base(continueToken, continueToken.Location, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}