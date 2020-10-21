public class BreakNode : StatementNode
{
    public BreakNode(ComplexToken breakToken, bool isValid = true) : base(breakToken, breakToken.Location, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}