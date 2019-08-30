using System;
using System.Linq;
using System.Collections.Generic;

public class StatementNode
{
    protected Token token;

    public Token Token {
        get => token;
    }

    protected string rep;

    public string Representation {
        get => rep;
    }

    public StatementNode(string representation, Token token) {
        rep = representation;
        this.token = token;
    }

    public new virtual string ToString() {
        return rep;
    }

    public virtual string GetFriendlyName()
        => "statement";
}

public class AssignmentNode : StatementNode
{
    protected ValueNode value;

    public ValueNode Value {
        get => value;
    }

    protected ComplexToken varName;

    public ComplexToken Name {
        get => varName;
    }

    public AssignmentNode(ValueNode value, ComplexToken varName) : base(varName , varName) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        this.varName = varName;
        this.value = value;
    }

    public override string ToString() {
        return varName + " = " + value.ToText();
    }

    public new string GetFriendlyName()
        => "assignment";
}

public class ReturnNode : StatementNode
{
    protected ValueNode value;

    public ValueNode Value {
        get => value;
    }

    protected bool isReturningValue;

    public bool IsReturningValue {
        get => isReturningValue;
    }

    public ReturnNode(ValueNode value, ComplexToken returnToken) : base(returnToken, returnToken) {
        isReturningValue = value == null;

        this.value = value;
    }
}

public class DeclarationNode : StatementNode
{
    protected ValueNode value;

    public ValueNode Value {
        get => value;
    }

    protected ComplexToken varName;

    public ComplexToken Name {
        get => varName;
    }

    public DeclarationNode(ValueNode value, ComplexToken varName) : base("var", varName) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        this.varName = varName;
        this.value = value;
    }

    public override string ToString() {
        return "var " + varName + " = " + value.ToText();
    }

    public new string GetFriendlyName()
        => "variable declaration";
}

public class FunctionDeclarationNode : DeclarationNode
{
    internal bool isInternal = false;
    protected new SimpleBlock value;

    public new SimpleBlock Value {
        get => value;
    }

    protected List<ComplexToken> parameters;

    public List<ComplexToken> Parameters {
        get => parameters;
    }

    public FunctionDeclarationNode(SimpleBlock value, ComplexToken[] parameters, ComplexToken functionName) : base(new ValueNode("block", functionName), functionName) {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (declaration)");

        this.value = value;
        this.parameters = new List<ComplexToken>(parameters);
    }

    public new string GetFriendlyName()
        => "function declaration";
}

public class SimpleBlock
{
    protected List<StatementNode> content;

    public List<StatementNode> Content {
        get => content;
    }

    public SimpleBlock(StatementNode[] content) {
        this.content = new List<StatementNode>(content);
    }
}