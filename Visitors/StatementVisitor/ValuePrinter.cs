using System;
using System.Linq;
using System.Text;

public sealed class ValuePrinter : StatementVisitor<string>
{

    protected override string Default(StatementNode node)
        => throw Logger.Fatal(new InvalidCallException("ValuePrinter cannot print statement like " + node.GetType().Name, node.Token.Location));

    protected override string Default(ValueNode node)
        => ASTHelper.PrintToken(node.Token);


    public override string Visit(StatementExpressionNode node) => Print(node.Value);

    public override string Visit(BoolNode node)
        => ASTHelper.PrintToken(node.Token);

    public override string Visit(FunctionCallNode node)
        => Print(node.FunctionName)
         + Utilities.Join(",", Print, node.ArgList);

    public override string Visit(ObjectCreationNode node)
        => ASTHelper.PrintToken(node.Token) + Print(node.InvocationNode);

    public override string Visit(OperationNode node) {
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
                     + Utilities.Join(",", Print, node.Operands.Skip(1)) // indices
                     + ASTHelper.PrintToken(node.AdditionalTokens[0]); // ']'
            case OperationType.Conditional:
                return Print(node.Operands[0])
                     + ASTHelper.PrintToken(node.Token)
                     + Print(node.Operands[1])
                     + ASTHelper.PrintToken(node.AdditionalTokens[0])
                     + Print(node.Operands[2]);
            default:
                throw new Exception("Oho, someone forgot to implement a printer for an operation type...");
        }
    }

    public override string Visit(TupleNode node)
        => ASTHelper.PrintToken(node.OpeningToken)
         + Utilities.Join(",", Print, node.Values)
         + ASTHelper.PrintToken(node.ClosingToken);

    public override string Visit(SimpleBlock block)
        => throw Logger.Fatal(new InvalidCallException("ValuePrinter cannot print blocks", block.Location));


    public string Print(ValueNode node) => node.Accept(this);
}