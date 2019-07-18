using System;
using System.Linq;
using System.Collections.Generic;

public class StatementNode
{
    protected string rep;

    public string Representation {
        get => rep;
    }

    public StatementNode(string representation) {
        rep = representation;
    }

    public new virtual string ToString() {
        return rep;
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

    public DeclarationNode(ValueNode value, ComplexToken varName) : base("svar " + varName + " = " + value.ToText()) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        this.varName = varName;
        this.value = value;
    }

    public override string ToString() {
        return "svar " + varName + " = " + value.ToText();
    }
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

    public AssignmentNode(ValueNode value, ComplexToken varName) : base(varName + " = " + value.ToText()) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        this.varName = varName;
        this.value = value;
    }

    public override string ToString() {
        return varName + " = " + value.ToText();
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

    public FunctionNode(ValueNode[] parameters, ComplexToken functionName) : base(functionName + "(...)") {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (call)");

        this.functionName = functionName;
        this.parameters = parameters;
    }
}

public class FunctionDeclarationNode : DeclarationNode
{
    protected new SimpleBlock value;

    public new SimpleBlock Value {
        get => value;
    }

    protected List<ComplexToken> parameters;

    public List<ComplexToken> Parameters {
        get => parameters;
    }

    public FunctionDeclarationNode(SimpleBlock value, ComplexToken[] parameters, ComplexToken functionName) : base(new ValueNode("block"), functionName) {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (declaration)");

        this.value = value;
        this.parameters = new List<ComplexToken>(parameters);
    }
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