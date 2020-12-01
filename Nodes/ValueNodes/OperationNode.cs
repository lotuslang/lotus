using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OperationNode : ValueNode
{
    public new static readonly OperationNode NULL = new OperationNode(OperatorToken.NULL, new ValueNode[0], OperationType.Unknown, false);

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
        : base(token, token.Location, isValid)
    {
        Operands = operands.AsReadOnly();
        OperationType = opType;
        AdditionalTokens = new ReadOnlyCollection<Token>(additionalTokens);

        Location = OperationKind switch {
            OperationKind.Unary => new LocationRange(token.Location, operands[0].Location),
            OperationKind.Binary => new LocationRange(operands[0].Location, operands[1].Location),
            OperationKind.Ternary => new LocationRange(operands[0].Location, operands[2].Location),
            _ => Token.Location
        };
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}
