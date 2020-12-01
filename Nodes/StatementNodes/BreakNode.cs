public class BreakNode : StatementNode
{
    public new static readonly BreakNode NULL = new BreakNode(ComplexToken.NULL, false);

    public BreakNode(ComplexToken breakToken, bool isValid = true) : base(breakToken, breakToken.Location, isValid) { }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}