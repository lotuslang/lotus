public class TopLevelNode : Node
{
    public new static readonly TopLevelNode NULL = new TopLevelNode(Token.NULL, false);

    public TopLevelNode(Token token, bool isValid = true) : base(token) { }
}