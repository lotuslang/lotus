public class FunctionCallNode : ValueNode
{
    public TupleNode ArgList { get; }

    public ValueNode FunctionName { get; protected set; }

    public FunctionCallNode(TupleNode args, ValueNode functionName, Token functionToken, bool isValid = true)
        : base(functionName.Representation + "(...)", functionToken, args.Location, isValid)
    {
        FunctionName = functionName;
        ArgList = args;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
