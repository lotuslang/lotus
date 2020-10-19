using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ArrayLiteralNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> Content { get; }

    public Token ClosingBracket { get; }

    public ArrayLiteralNode(IList<ValueNode> content, Token leftSquareBracketToken, Token rightBracket, bool isValid = true)
    : base(leftSquareBracketToken, isValid)
    {
        Content = content.AsReadOnly();
        ClosingBracket = rightBracket;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
