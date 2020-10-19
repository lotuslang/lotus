using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionCallNode : ValueNode
{
    public ReadOnlyCollection<ValueNode> CallingParameters { get; }

    public ValueNode FunctionName { get; protected set; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    public FunctionCallNode(IList<ValueNode> parameters, ValueNode functionName, Token functionToken, Token leftParen, Token rightParen, bool isValid = true)
        : base(functionName.Representation + "(...)", functionToken, isValid)
    {
        OpeningParenthesis = leftParen;
        ClosingParenthesis = rightParen;
        FunctionName = functionName;
        CallingParameters = parameters.AsReadOnly();
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
