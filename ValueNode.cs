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
    public virtual string Value {
        get => rep;
    }

    public ValueNode(string rep, Token token) : base(rep, token) { }

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

    public OperationNode(Token token, ValueNode[] operands, string opType) : base(token, token)
    {
        this.operands = new List<ValueNode>(operands);
        this.opType = opType.ToLower();
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

    public NumberNode(double value, Token token) : base(value.ToString(), token)
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

    public StringNode(string value, Token token) : base(value, token)
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

    public IdentNode(string varName, Token token) : base(varName, token)
    {
        this.varName = varName;
    }
}

public class FunctionNode : ValueNode
{
    protected ValueNode[] parameters;

    public ValueNode[] CallingParameters {
        get => parameters;
    }

    protected ComplexToken functionName;

    public ComplexToken Name {
        get => functionName;
    }

    public FunctionNode(ValueNode[] parameters, ComplexToken functionName) : base(functionName + "(...)", functionName) {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (call)");

        this.functionName = functionName;
        this.parameters = parameters;
    }
}