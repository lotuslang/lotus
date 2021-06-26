/// <summary>
/// Represents a foreach loop statement (foreach (item in collection) { })
/// </summary>
public class ForeachNode : StatementNode
{
    public new static readonly ForeachNode NULL
        = new ForeachNode(
            ComplexToken.NULL,
            ComplexToken.NULL,
            IdentNode.NULL,
            ValueNode.NULL,
            SimpleBlock.NULL,
            Token.NULL,
            Token.NULL,
            false
        );


    /// <summary>
    /// The token of the "in" keyword used
    /// </summary>
    public ComplexToken InToken { get; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    /// <summary>
    /// The name of the variable used to represent the current item
    /// of the collection in the scope of the loop's body
    /// </summary>
    public IdentNode ItemName { get; }

    /// <summary>
    /// The collection being looped over.
    ///
    /// Can be an identifier, can be a function call,
    /// or really any class derived from ValueNode since the
    /// check that it really is a collection is going to be
    /// done by the semantic analysis
    /// </summary>
    public ValueNode Collection { get; }

    /// <summary>
    /// The body of the loop, what will be executed each time
    /// </summary>
    public SimpleBlock Body { get; }

    /// <summary>
    /// Creates a ForeachNode.
    /// </summary>
    /// <param name="foreachToken">The token of the 'foreach' keyword used</param>
    /// <param name="inToken">The token of the 'in' keyword used</param>
    /// <param name="itemName">The name of the variable used in the loop</param>
    /// <param name="collectionName">The name of the collection being looped over</param>
    /// <param name="body">The body of the foreach loop</param>
    public ForeachNode(ComplexToken foreachToken,
                       ComplexToken inToken,
                       IdentNode itemName,
                       ValueNode collectionName,
                       SimpleBlock body,
                       Token openParen,
                       Token closeParen,
                       bool isValid = true)
        : base(foreachToken, new LocationRange(foreachToken.Location, body.Location), isValid)
    {
        InToken = inToken;
        OpeningParenthesis = openParen;
        ClosingParenthesis = closeParen;
        ItemName = itemName;
        Collection = collectionName;
        Body = body;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.Visit(this);
}
