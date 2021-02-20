using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FunctionDeclarationNode : StatementNode
{
    public new static readonly FunctionDeclarationNode NULL
        = new FunctionDeclarationNode(
            SimpleBlock.NULL,
            new FunctionParameter[0],
            ValueNode.NULL,
            IdentToken.NULL,
            ComplexToken.NULL,
            Token.NULL,
            Token.NULL,
            Token.NULL,
            false
        );

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
        : base(funcKeyword, new LocationRange(funcKeyword.Location, body.Location), isValid)
    {
        Name = functionName;
        Body = body;
        Parameters = parameters.AsReadOnly();
        ReturnType = returnType;
        OpeningParenthesis = openingParen;
        ClosingParenthesis = closingParen;
        ColonToken = colonToken;
    }

    [System.Diagnostics.DebuggerHidden()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.Diagnostics.DebuggerNonUserCode()]
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override T Accept<T>(StatementVisitor<T> visitor) => visitor.Visit(this);
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