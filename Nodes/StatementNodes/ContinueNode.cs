public class ContinueNode : StatementNode
{
    public new static readonly ContinueNode NULL = new ContinueNode(ComplexToken.NULL, false);

    public ContinueNode(ComplexToken continueToken, bool isValid = true) : base(continueToken, continueToken.Location, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}