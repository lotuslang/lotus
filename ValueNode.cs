using System;
using System.Collections.Generic;
using System.Text;


[System.Diagnostics.DebuggerDisplay("{representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// The value of this ValueNode
    /// </summary>
    /// <value></value>
    public dynamic Value
    {
        get
        {
            // If it is a NumberNode, returns its value.
            if (this is NumberNode)
            {
                return ((NumberNode)this).Value;
            }

            // If it is an OperationNode, returns its value (which needs to be calculated).
            if (this is OperationNode)
            {
                return ((OperationNode)this).Value;
            }

            // Otherwise, returns the string representation of this object
            return rep;
        }
    }

    public ValueNode(string rep) : base(rep) { }

    /// <summary>
    /// A human-readable version of this ValueNode
    /// </summary>
    /// <returns>Returns a human-friendly string representing this ValueNode</returns>
    public virtual string ToText() => rep;
}

public class OperationNode : ValueNode
{
    protected Func<ValueNode[], dynamic> op;

    /// <summary>
    /// The operation that this OperationNode represents.
    /// </summary>
    /// <value>A delegate that takes in an array of ValueNode as argument and returns the result of that operation.</value>
    public Func<ValueNode[], dynamic> Operation {
        get => op;
    }

    protected ValueNode[] operands;

    /// <summary>
    /// The operands of this operation
    /// </summary>
    /// <value>An array of ValueNode[]</value>
    public ValueNode[] Operands {
        get => operands;
    }

    protected string opType;

    /// <summary>
    /// Indicates what type/kind of operation this object represents.
    /// </summary>
    /// <value>A string representing the type of operation this object represents.</value>
    public string OperationType {
        get => opType;
    }

    /// <summary>
    /// Basically applies the operation to the operands.
    /// </summary>
    /// <value>The value of the operation this object represents.</value>
    public new dynamic Value
    {
        get => op(operands);
    }

    public OperationNode(string rep, ValueNode[] operands, Func<ValueNode[], dynamic> operation, string opType) : base(rep)
    {
        op = operation;
        this.operands = operands;
        this.opType = opType.ToLower();
    }

    /// <summary>
    /// Adds new operands to the Operands array.
    /// </summary>
    /// <param name="operandList">The array of operands to add.</param>
    public void Add(params ValueNode[] operandList)
    {
        // Create a list based on the current operands.
        var temp = new List<ValueNode>(operands);

        // For each operand in operandList, add it to that list
        foreach (var operand in operandList)
        {
            temp.Add(operand);
        }

        // Set the operands list to the temp list made earlier.
        operands = temp.ToArray();
    }

    /// <summary>
    /// A human-readable version of this operation.
    /// </summary>
    /// <returns>Returns a human-friendly string representing this operation.</returns>
    public override string ToText()
    {
        // Create a new string builder
        var strBuilder = new StringBuilder();

        // If this operation is unary
        if (operands.Length == 1)
        {
            // Append a representation with the operator in front of the only operand
            strBuilder.Append(rep + operands[0].ToText());
        }

        // If this operation is binary
        if (operands.Length == 2)
        {
            // If the first operand is an operation
            if (Operands[0] is OperationNode)
                // Put its representation in parenthesis
                strBuilder.Append("(" + operands[0].ToText() + ")");
            else
                // Otherwise (its a NumberNode), append its representation
                strBuilder.Append(operands[0].Representation);

            // Append the representation (preceeded and followed by spaces)
            strBuilder.Append(" " + rep + " ");

            // If the second operand is an operation
            if (operands[1] is OperationNode)
                // Put its representation in parenthesis
                strBuilder.Append("(" + operands[1].ToText() + ")");
            else
                // Otherwise, (its a NumberNode), append its representation
                strBuilder.Append(operands[1].Representation);
        }

        // returns the string builder
        return strBuilder.ToString();
    }
}

public class NumberNode : ValueNode
{
    protected double value;

    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new double Value {
        get => value;
     }

    public NumberNode(double value) : base(value.ToString())
    {
        this.value = value;
    }
}


public class StringNode : ValueNode
{
    protected string value;

    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new string Value {
        get => value;
    }

    public StringNode(string value) : base(value)
    {
        this.value = value;
    }
}

public class IdentNode : ValueNode
{
    protected string varName;

    public string Value {
        get => varName;
    }

    public IdentNode(string varName) : base(varName)
    {
        this.varName = varName;
    }
}