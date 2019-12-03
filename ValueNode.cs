using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;


[System.Diagnostics.DebuggerDisplay("{Representation}")]
public class ValueNode : StatementNode
{
    /// <summary>
    /// This constant is the equivalent of "null". When a function doesn't return, it will actually set the `#return` variable to this constant.
    /// Variables that are assigned to a non-returning functions will actually be assigned this value.
    /// </summary>
    public static readonly ValueNode NULL = new ValueNode("null", new Token('\0', TokenKind.EOF, null));

    //// <summary>
    /// The value of this ValueNode
    /// </summary>
    /// <value></value>
    public virtual string Value {
        get => rep;
    }

    public ValueNode(Token token) : this(token.Representation, token) { }

    public ValueNode(string rep, Token token) : base(rep, token) { }

    public new string GetFriendlyName()
        => "value";
}

public class OperationNode : ValueNode
{
    public new OperatorToken Token {
        get => token as OperatorToken;
    }

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

    public OperationNode(OperatorToken token, ValueNode[] operands, string opType) : base(token, token)
    {
        this.operands = new List<ValueNode>(operands);
        this.opType = opType;
    }

    public new string GetFriendlyName()
        => "operation";
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

    public NumberNode(double value, Token token) : base(value.ToString(), token) {
        this.value = value;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }

    public new string GetFriendlyName()
        => "number";
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

    public new string GetFriendlyName()
        => "string";
}

public class ComplexStringNode : StringNode
{
    protected List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => new ReadOnlyCollection<ValueNode>(sections);
    }

    public ComplexStringNode(ComplexStringToken token, List<ValueNode> codeSections) : base(token.Representation, token) {
        sections = codeSections;
    }

    public void AddSection(ValueNode section) {
        sections.Add(section);
    }
}

public class BoolNode : ValueNode
{
    protected bool value;

    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new bool Value {
        get => value;
    }

    public BoolNode(bool value, Token token) : base(value.ToString().ToLower(), token)
    {
        this.value = value;
    }

    public BoolNode(string repr, Token token) : this(repr == "true", token) { }

    public new string GetFriendlyName()
        => "boolean";
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

    public new string GetFriendlyName()
        => "variable name";
}

public class FunctionCallNode : ValueNode
{
    protected List<ValueNode> parameters;

    public ReadOnlyCollection<ValueNode> CallingParameters {
        get => new ReadOnlyCollection<ValueNode>(parameters);
    }

    protected ValueNode name;

    public ValueNode FunctionName {
        get => name;
    }

    public FunctionCallNode(ValueNode[] parameters, ValueNode functionName, Token token)
        : base(functionName.Representation + "(...)", token)
    {

        this.name = functionName;
        this.parameters = new List<ValueNode>(parameters);
    }

    public new string GetFriendlyName()
        => "function";
}