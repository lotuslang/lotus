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
    public static new readonly ValueNode NULL = new ValueNode("null", new Token('\0', TokenKind.EOF, null));

    //// <summary>
    /// The value of this ValueNode
    /// </summary>
    /// <value></value>
    public virtual string Value { get; protected set; }

    public ValueNode(Token token) : this(token.Representation, token) { }

    public ValueNode(string rep, Token token) : base(rep, token) { }

    public new string GetFriendlyName()
        => "value";
}

public class OperationNode : ValueNode
{
    public new OperatorToken Token {
        get => Token;
    }

    protected List<ValueNode> operands;

    /// <summary>
    /// The operands of this operation
    /// </summary>
    /// <value>An array of ValueNode</value>
    public ReadOnlyCollection<ValueNode> Operands {
        get => operands.AsReadOnly();
    }

    /// <summary>
    /// Indicates what type/kind of operation this object represents.
    /// </summary>
    /// <value>A string representing the type of operation this object represents.</value>
    public string OperationType { get; protected set; }

    public OperationNode(OperatorToken token, ValueNode[] operands, string opType) : base(token)
    {
        this.operands = new List<ValueNode>(operands);
        OperationType = opType;
    }

    public new string GetFriendlyName()
        => "operation";
}

public class NumberNode : ValueNode
{
    /// <summary>
    /// The value of this NumberNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new double Value { get; protected set; }

    public NumberNode(double value, Token token) : base(value.ToString(), token) {
        Value = value;
    }

    public NumberNode(NumberToken token) : this(token.Value, token)
    { }

    public new string GetFriendlyName()
        => "number";
}


public class StringNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new string Value { get; protected set; }

    public StringNode(string value, Token token) : base(value, token)
    {
        Value = value;
    }

    public new string GetFriendlyName()
        => "string";
}

public class ComplexStringNode : StringNode
{
    protected List<ValueNode> sections;

    public ReadOnlyCollection<ValueNode> CodeSections {
        get => sections.AsReadOnly();
    }

    public ComplexStringNode(ComplexStringToken token, List<ValueNode> codeSections) : base(token.Representation, token) {
        sections = new List<ValueNode>(codeSections);
    }

    public void AddSection(ValueNode section) {
        sections.Add(section);
    }
}

public class BoolNode : ValueNode
{
    /// <summary>
    /// The value of this StringNode.
    /// </summary>
    /// <value>The number represented by this object.</value>
    public new bool Value { get; protected set; }

    public BoolNode(bool value, Token boolToken) : base(value.ToString().ToLower(), boolToken)
    {
        Value = value;
    }

    public BoolNode(string repr, Token boolToken) : this(repr == "true", boolToken) { }

    public new string GetFriendlyName()
        => "boolean";
}

public class IdentNode : ValueNode
{

    public override string Value { get; protected set; }

    public IdentNode(string value, Token identToken) : base(value, identToken)
    {
        Value = value;
    }

    public new string GetFriendlyName()
        => "variable name";
}

public class FunctionCallNode : ValueNode
{
    protected List<ValueNode> parameters;

    public ReadOnlyCollection<ValueNode> CallingParameters {
        get => parameters.AsReadOnly();
    }

    public ValueNode FunctionName { get; protected set; }

    public FunctionCallNode(ValueNode[] parameters, ValueNode functionName, Token functionToken)
        : base(functionName.Representation + "(...)", functionToken)
    {

        FunctionName = functionName;
        this.parameters = new List<ValueNode>(parameters);
    }

    public new string GetFriendlyName()
        => "function";
}

public class ArrayLiteralNode : ValueNode
{
    protected List<ValueNode> items;

    public ReadOnlyCollection<ValueNode> Content {
        get => items.AsReadOnly();
    }
    public ArrayLiteralNode(ValueNode[] content, Token leftSquareBracketToken) : base(leftSquareBracketToken) {
        items = new List<ValueNode>(content);
    }
}

public class TypeCastNode : ValueNode
{
    public ValueNode Type { get; protected set; }

    public ValueNode Operand { get; protected set; }

    public TypeCastNode(ValueNode type, ValueNode operand, Token parenToken) : base(parenToken) {
        Type = type;
        Operand = operand;
    }
}

public class ObjectCreationNode : ValueNode
{
    public FunctionCallNode InvocationNode { get; protected set; }

    public ObjectCreationNode(FunctionCallNode invoke, ComplexToken newToken) : base(newToken) {
        InvocationNode = invoke;
    }
}