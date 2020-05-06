public class IdentNode : ValueNode
{

    public string Value { get; protected set; }

    public IdentNode(string value, Token identToken) : base(value, identToken)
    {
        Value = value;
    }
}
