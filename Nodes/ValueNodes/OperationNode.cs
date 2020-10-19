using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OperationNode : ValueNode
{
    public ReadOnlyCollection<Token> AdditionalTokens { get; }

    /// <summary>
    /// The operands of this operation
    /// </summary>
    /// <value>An array of ValueNode</value>
    public ReadOnlyCollection<ValueNode> Operands { get; }

    public OperationType OperationType { get; }

    public OperationKind OperationKind {
        get => Operands.Count switch {
            1 => OperationKind.Unary,
            2 => OperationKind.Binary,
            3 => OperationKind.Ternary,
            _ => OperationKind.Unknown
        };
    }

    public OperationNode(OperatorToken token, IList<ValueNode> operands, OperationType opType, bool isValid = true, params Token[] additionalTokens)
        : base(token, isValid)
    {
        Operands = operands.AsReadOnly();
        OperationType = opType;
        AdditionalTokens = new ReadOnlyCollection<Token>(additionalTokens);
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
