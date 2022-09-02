internal sealed class ValuePrinter : IValueVisitor<string>
{
    public string Default(ValueNode node)
        => ASTHelper.PrintToken(node.Token);

    public string Visit(FunctionCallNode node)
        => Print(node.Name)
         + Print(node.ArgList);

    public string Visit(ObjectCreationNode node)
        => ASTHelper.PrintToken(node.Token) + Print(node.Invocation);

    public string Visit(OperationNode node) {
        switch (node.OperationType) {
            // prefix stuff
            case OperationType.Positive:
            case OperationType.Negative:
            case OperationType.Not:
            case OperationType.PrefixIncrement:
            case OperationType.PrefixDecrement:
                return ASTHelper.PrintToken(node.Token) + Print(node.Operands[0]);

            // postfix stuff
            case OperationType.PostfixIncrement:
            case OperationType.PostfixDecrement:
                return Print(node.Operands[0]) + ASTHelper.PrintToken(node.Token);

            // normal infix stuff
            case OperationType.Addition:
            case OperationType.Substraction:
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
                     + ASTHelper.PrintToken(node.Token)
                     + Print(node.Operands[1]);
            case OperationType.ArrayAccess:
                return Print(node.Operands[0]) // name of the array
                     + ASTHelper.PrintToken(node.Token) // '['
                     + Utils.Join(",", Print, node.Operands.Skip(1)) // indices
                     + ASTHelper.PrintToken(node.AdditionalTokens[0]); // ']'
            case OperationType.Conditional:
                return Print(node.Operands[0])
                     + ASTHelper.PrintToken(node.Token)
                     + Print(node.Operands[1])
                     + ASTHelper.PrintToken(node.AdditionalTokens[0])
                     + Print(node.Operands[2]);
            case OperationType.Unknown:
                return ASTHelper.PrintToken(node.Token)
                     + (node.Operands.Length == 0
                        ?   ""
                        :   "(" + Utils.Join(",", Print, node.Operands) + ")"
                    );
            default:
                throw new Exception("Oho, someone forgot to implement a printer for an operation type...");
        }
    }

    public string Visit(TupleNode node)
        => Print((Tuple<ValueNode>)node);

    public string Visit(ParenthesizedValueNode node)
        => ASTHelper.PrintToken(node.OpeningToken)
         + Print(node.Value)
         + ASTHelper.PrintToken(node.ClosingToken);

    public string Visit(NameNode name)
        => Utils.Join(".", ASTHelper.PrintToken, name.Parts);

    public string Print<TVal>(Tuple<TVal> tuple) where TVal : ValueNode
        => ASTHelper.PrintToken(tuple.OpeningToken)
         + Utils.Join(",", Print, tuple.Items)
         + ASTHelper.PrintToken(tuple.ClosingToken);

    public string Print(ValueNode node) => node.Accept(this);

}