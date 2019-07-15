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

    public ComplexToken VariableName {
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

public class AssignementNode : StatementNode
{
    protected ValueNode value;

    public ValueNode Value {
        get => value;
    }

    protected ComplexToken varName;

    public ComplexToken Name {
        get => varName;
    }

    public AssignementNode(ValueNode value, ComplexToken varName) : base(varName + " = " + value.ToText()) {
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
    protected ValueNode value;

    public new ValueNode Value {
        get => value;
    }

    protected ComplexToken functionName;

    public ComplexToken Name {
        get => functionName;
    }

    public FunctionNode(ValueNode value, ComplexToken functionName) : base(functionName + "(...)") {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier");

        this.functionName = functionName;
        this.value = value;
    }
}