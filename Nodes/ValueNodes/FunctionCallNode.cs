public class FunctionCallNode : ValueNode
{
    public new static readonly FunctionCallNode NULL = new(TupleNode.NULL, ValueNode.NULL, false);

    public TupleNode ArgList { get; }

    public ValueNode FunctionName { get; protected set; }

    public FunctionCallNode(TupleNode args, ValueNode functionName, bool isValid = true)
        : base(functionName.Representation + "(...)", functionName.Token, args.Location, isValid)
    {
        FunctionName = functionName;
        ArgList = args;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IValueVisitor<T> visitor) => visitor.Visit(this);
}
