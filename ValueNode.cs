using System;
using System.Collections.Generic;
using System.Text;


[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    //// <summary>
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
                //return ((OperationNode)this).Value;
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
    protected List<ValueNode> operands;

    /// <summary>
    /// The operands of this operation
    /// </summary>
    /// <value>An array of ValueNode</value>
    public ValueNode[] Operands {
        get => operands.ToArray();
    }

    protected string opType;

    /// <summary>
    /// Indicates what type/kind of operation this object represents.
    /// </summary>
    /// <value>A string representing the type of operation this object represents.</value>
    public string OperationType {
        get => opType;
    }

    public OperationNode(string rep, ValueNode[] operands, string opType) : base(rep)
    {
        this.operands = new List<ValueNode>(operands);
        this.opType = opType.ToLower();
    }

    /// <summary>
    /// Adds new operands to the Operands array.
    /// </summary>
    /// <param name="operandList">The array of operands to add.</param>
    public void Add(params ValueNode[] operandList)
    {
        // For each operand in operandList, add it to the operands list
        foreach (var operand in operandList)
        {
            operands.Add(operand);
        }
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

    protected ValueNode value;

    public new ValueNode Value {
        get => value;
        set => this.value = value;
    }

    public IdentNode(string varName) : base(varName)
    {
        this.varName = varName;
    }
}