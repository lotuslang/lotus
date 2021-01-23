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

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
