namespace Lotus.Syntax.Visitors;

internal sealed class ValuePrinter : IValueVisitor<string>
{
    public string Default(ValueNode node)
        => ASTUtils.PrintToken(node.Token);

    public string Visit(FunctionCallNode node)
        => Print(node.Name)
         + Print(node.ArgList);

    public string Visit(ObjectCreationNode node)
        => ASTUtils.PrintToken(node.Token) + Print(node.Invocation);

    public string Visit(OperationNode node) {
        switch (node.OperationType) {
            // prefix stuff
            case OperationType.Positive:
            case OperationType.Negative:
            case OperationType.Not:
            case OperationType.PrefixIncrement:
            case OperationType.PrefixDecrement:
                return ASTUtils.PrintToken(node.Token) + Print(node.Operands[0]);

            // postfix stuff
            case OperationType.PostfixIncrement:
            case OperationType.PostfixDecrement:
                return Print(node.Operands[0]) + ASTUtils.PrintToken(node.Token);

            // normal infix stuff
            case OperationType.Addition:
            case OperationType.Subtraction:
            case OperationType.Multiplication:
            case OperationType.Division:
            case OperationType.Power:
            case OperationType.Modulo:
            case OperationType.Or:
            case OperationType.And:
            case OperationType.Xor:
            case OperationType.Equal:
            case OperationType.NotEqual:
            case OperationType.Less:
            case OperationType.LessOrEqual:
            case OperationType.Greater:
            case OperationType.GreaterOrEqual:
            case OperationType.Access:
            case OperationType.Assign:
                return Print(node.Operands[0])
                     + ASTUtils.PrintToken(node.Token)
                     + Print(node.Operands[1]);
            case OperationType.ArrayAccess:
                return Print(node.Operands[0]) // name of the array
                     + ASTUtils.PrintToken(node.Token) // '['
                     + MiscUtils.Join(",", Print, node.Operands.Skip(1)) // indices
                     + ASTUtils.PrintToken(node.AdditionalTokens[0]); // ']'
            case OperationType.Conditional:
                return Print(node.Operands[0])
                     + ASTUtils.PrintToken(node.Token)
                     + Print(node.Operands[1])
                     + ASTUtils.PrintToken(node.AdditionalTokens[0])
                     + Print(node.Operands[2]);
            case OperationType.Unknown:
                return ASTUtils.PrintToken(node.Token)
                     + (node.Operands.Length == 0
                        ?   ""
                        :   "(" + MiscUtils.Join(",", Print, node.Operands) + ")"
                    );
            default:
                throw new Exception("Oho, someone forgot to implement a printer for an operation type...");
        }
    }

    public string Visit(TupleNode node)
        => Print((Tuple<ValueNode>)node);

    public string Visit(ParenthesizedValueNode node)
        => ASTUtils.PrintToken(node.OpeningToken)
         + Print(node.Value)
         + ASTUtils.PrintToken(node.ClosingToken);

    public string Visit(NameNode name)
        => MiscUtils.Join(".", ASTUtils.PrintToken, name.Parts);

    public string Print<TVal>(Tuple<TVal> tuple) where TVal : ValueNode
        => ASTUtils.PrintToken(tuple.OpeningToken)
         + MiscUtils.Join(",", Print, tuple.Items)
         + ASTUtils.PrintToken(tuple.ClosingToken);

    public string Print(ValueNode node) => node.Accept(this);
}