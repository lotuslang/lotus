using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    internal bool isInternal = false;

    public bool HasReturnType => ReturnType != ValueNode.NULL;

    public SimpleBlock Body { get; }

    public ReadOnlyCollection<FunctionParameter> Parameters { get; }

    public ValueNode ReturnType { get; }

    public IdentToken Name { get; }

    public Token OpeningParenthesis { get; }

    public Token ClosingParenthesis { get; }

    public Token ColonToken { get; }

    public FunctionDeclarationNode(SimpleBlock body,
                                   IList<FunctionParameter> parameters,
                                   ValueNode returnType,
                                   IdentToken functionName,
                                   ComplexToken funcKeyword,
                                   Token openingParen,
                                   Token closingParen,
                                   Token colonToken,
                                   bool isValid = true)
        : base(funcKeyword, isValid)
    {
        Name = functionName;
        Body = body;
        Parameters = parameters.AsReadOnly();
        ReturnType = returnType;
        OpeningParenthesis = openingParen;
        ClosingParenthesis = closingParen;
        ColonToken = colonToken;
    }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}

public class FunctionParameter
{
    public ValueNode Type { get; }

    public IdentNode Name { get; }

    public Token EqualSign { get; }

    public ValueNode DefaultValue { get; }

    public bool HasDefaultValue => DefaultValue != ValueNode.NULL;

    public bool IsValid { get; set; }

    public FunctionParameter(ValueNode type, IdentNode name, ValueNode defaultValue, Token equalSign, bool isValid = true) {
        Type = type;
        Name = name;
        EqualSign = equalSign;
        DefaultValue = defaultValue;
        IsValid = isValid;
    }
}