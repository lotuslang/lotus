using System;

public class BoolNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public bool Value { get; protected set; }

    public BoolNode(bool value, Token boolToken, bool isValid = true) : base(value.ToString().ToLower(), boolToken, isValid) {
        Value = value;
    }

    public BoolNode(string repr, Token boolToken, bool isValid = true) : this(default(bool), boolToken, isValid)
    {
        var value = Value; // workaround for CS0206

        if (isValid && !Boolean.TryParse(repr, out value)) {
            throw Logger.Fatal(new InternalErrorException(
                message: $"Could not parse string {repr} as a boolean because a bool can only take the values 'true' and 'false'",
                Token.Location
            ));
        }

        Value = value;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
