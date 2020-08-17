public class IdentNode : ValueNode
{
    public new ComplexToken Token { get; }

    public string Value { get; protected set; }

    public IdentNode(string value, IdentToken identToken, bool isValid = true) : base(value, identToken, isValid) {
        Token = identToken;
        Value = value;
    }
}
