using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ArrayLiteralNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> Content { get; }

    public Token ClosingBracket { get; }

    public ArrayLiteralNode(IList<ValueNode> content, Token leftBracket, Token rightBracket, bool isValid = true)
        : base(leftBracket, new LocationRange(leftBracket.Location, rightBracket.Location), isValid)
    {
        Content = content.AsReadOnly();
        ClosingBracket = rightBracket;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
