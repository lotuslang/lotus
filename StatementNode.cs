using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class StatementNode
{
    public Token Token { get; protected set; }

    public string Representation { get; protected set; }

    public StatementNode(string representation, Token token) {
        Representation = representation;
        Token = token;
    }

    public StatementNode(Token token) : this(token.Representation, token)
        { }

    public virtual string GetFriendlyName()
        => "statement";
}

public class AssignmentNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public ComplexToken Name { get; protected set; }

    public AssignmentNode(ValueNode value, ComplexToken varName, ComplexToken equalToken) : base(equalToken) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        Name = varName;
        Value = value;
    }

    public new string GetFriendlyName()
        => "assignment";
}

public class ReturnNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public bool IsReturningValue { get; protected set; }

    public ReturnNode(ValueNode value, ComplexToken returnToken) : base(returnToken) {
        IsReturningValue = value == null;

        Value = value;
    }

    public new string GetFriendlyName()
        => "return";
}

public class DeclarationNode : StatementNode
{
    public ValueNode Value { get; protected set; }

    public ComplexToken Name { get; protected set; }

    public DeclarationNode(ValueNode value, ComplexToken varName, ComplexToken introToken) : base(introToken) {
        if (varName != TokenKind.ident) throw new ArgumentException("The variable name was not an identifier");

        Name = varName;
        Value = value;
    }

    public new string GetFriendlyName()
        => "variable declaration";
}

public class FunctionDeclarationNode : DeclarationNode
{
    internal bool isInternal = false;

    public new SimpleBlock Value { get; protected set; }

    protected List<ComplexToken> parameters;

    public ReadOnlyCollection<ComplexToken> Parameters {
        get => new ReadOnlyCollection<ComplexToken>(parameters);
    }

    public FunctionDeclarationNode(SimpleBlock value,
                                   ComplexToken[] parameters,
                                   ComplexToken functionName,
                                   ComplexToken defToken)
        : base(new ValueNode("block", functionName), functionName, defToken)
    {
        if (functionName != TokenKind.ident) throw new ArgumentException("The function name was not an identifier (declaration)");

        Value = value;
        this.parameters = new List<ComplexToken>(parameters);
    }

    public new string GetFriendlyName()
        => "function declaration";
}

public class NamespaceNode : StatementNode
{
    public ValueNode NamespaceName { get; protected set; }

    public NamespaceNode(ValueNode namespaceName, ComplexToken namespaceToken) : base(namespaceToken) {
        NamespaceName = namespaceName;
    }
}

public class ImportNode : StatementNode
{
    public ReadOnlyCollection<ValueNode> ImportsName { get; protected set; }

    public FromNode FromStatement { get; protected set; }

    public ImportNode(IEnumerable<ValueNode> imports, FromNode from, ComplexToken importToken) : base(importToken) {
        ImportsName = imports.ToList().AsReadOnly();
        FromStatement = from;
    }
}

public class FromNode : StatementNode
{
    // private protected : variable is accessible only to this class or derived classes within this assembly
    // (basically what i thought protected internal did)
    internal bool IsInternalOrigin { get; private protected set; }

    public ValueNode OriginName { get; protected set; }

    public FromNode(StringNode originName, ComplexToken fromToken) : this(originName, fromToken, false)
        { }

    internal FromNode(ValueNode originName, ComplexToken fromToken, bool isInternal) : base(fromToken) {
        OriginName = originName;
        IsInternalOrigin = isInternal;
    }
}

public class SimpleBlock
{
    protected List<StatementNode> content;

    public ReadOnlyCollection<StatementNode> Content {
        get => content.AsReadOnly();
    }

    public SimpleBlock(StatementNode[] content) {
        this.content = new List<StatementNode>(content);
    }
}