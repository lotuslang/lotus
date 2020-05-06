/// <summary>
/// Represents a foreach loop statement (foreach (a in b) { })
/// </summary>
public class ForeachNode : StatementNode
{
    /// <summary>
    /// The token of the "in" keyword used
    /// </summary>
    public ComplexToken InToken { get; protected set; }

    /// <summary>
    /// The name of the variable used to represent the current item
    /// of the collection in the scope of the loop's body
    /// </summary>
    public IdentNode ItemName {
        get;
        protected set;
    }

    /// <summary>
    /// The collection being looped over.
    ///
    /// Can be an identifier, can be a function call,
    /// or really any class derived from ValueNode since the
    /// check that it really is a collection is going to be
    /// done by the semantic analysis
    /// </summary>
    public ValueNode Collection {
        get;
        protected set;
    }

    /// <summary>
    /// The body of the loop, what will be executed each time
    /// </summary>
    public SimpleBlock Body {
        get;
        protected set;
    }

    /// <summary>
    /// Creates a ForeachNode.
    /// </summary>
    /// <param name="foreachToken">The token of the 'foreach' keyword used</param>
    /// <param name="inToken">The token of the 'in' keyword used</param>
    /// <param name="itemName">The name of the variable used in the loop</param>
    /// <param name="collectionName">The name of the collection being looped over</param>
    /// <param name="body">The body of the foreach loop</param>
    public ForeachNode(ComplexToken foreachToken, ComplexToken inToken, IdentNode itemName, ValueNode collectionName, SimpleBlock body)
        : base(foreachToken)
    {
        InToken = inToken;
        ItemName = itemName;
        Collection = collectionName;
        Body = body;
    }

    public override GraphNode ToGraphNode() {
        var root = new GraphNode(GetHashCode(), "foreach");

        root.AddProperty("color", "pink");
        root.AddProperty("tooltip", "foreach loop");

        var inNode = new GraphNode(InToken.GetHashCode(), "in");

        inNode.AddProperty("tooltip", "in iterator");

        inNode.AddNode(ItemName.ToGraphNode());
        inNode.AddNode(Collection.ToGraphNode());

        root.AddNode(inNode);

        var bodyNode = Body.ToGraphNode();

        root.AddNode(bodyNode);

        return root;
    }
}
