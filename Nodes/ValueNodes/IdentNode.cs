public class IdentNode : ValueNode
{
    public new ComplexToken Token { get; }

    public string Value { get; protected set; }

    public IdentNode(string value, ComplexToken identToken) : base(value, identToken) {
        Token = identToken;
        Value = value;
    }
}
