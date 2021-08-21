using System;

public class BoolNode : ValueNode
{
    public new static readonly BoolNode NULL = new(false, BoolToken.NULL, false);

    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public bool Value { get; protected set; }

    public new BoolToken Token { get; protected set; }

    public BoolNode(bool value, BoolToken boolToken, bool isValid = true)
        : base(value.ToString(), boolToken, boolToken.Location, isValid)
    {
        Value = value;
        Token = boolToken;
    }

    public BoolNode(string repr, BoolToken boolToken, bool isValid = true) : this(default(bool), boolToken, isValid)
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

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
