using System;
using System.Linq;
using System.Collections.Generic;

/*public static class Operators
{
    public readonly static Func<ValueNode[], dynamic> unaryPositive = delegate (ValueNode[] operands)
    {

        if (operands.Length != 1)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand = operands[0];
        if (operand is NumberNode)
        {
            return ((NumberNode)operand).Value;
        }

        if (operand is OperationNode)
        {
            return ((OperationNode)operand).Value;
        }

        throw new NotImplementedException("Unknow ValueNode type");
    };

    public readonly static Func<ValueNode[], dynamic> unaryNegative = delegate (ValueNode[] operands)
    {

        if (operands.Length != 1)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand = operands[0];
        if (operand is NumberNode)
        {
            return -((NumberNode)operand).Value;
        }

        if (operand is OperationNode)
        {
            return -((OperationNode)operand).Value;
        }

        throw new NotImplementedException("Unknow ValueNode type");
    };

    public readonly static Func<ValueNode[], dynamic> add = delegate (ValueNode[] operands)
    {

        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand1 is IdentNode)
        {

        }

        if (operand1 is StringNode)
        {
            if (operand2 is StringNode)
            {
                return ((StringNode)operand1).Value + ((StringNode)operand2).Value;
            }

            if (operand2 is IdentNode)
            {
                //return (())
            }

            if (operand2 is NumberNode)
            {
                return ((StringNode)operand1).Value + ((NumberNode)operand2).Value.ToString();
            }

            if (operand2 is OperationNode)
            {
                return ((StringNode)operand1).Value + ((OperationNode)operand2).Value.ToString();
            }
        }

        if (operand1 is NumberNode)
        {
            if (operand2 is StringNode)
            {
                return ((NumberNode)operand1).Value.ToString() + ((StringNode)operand2).Value;
            }

            if (operand2 is NumberNode)
            {
                return ((NumberNode)operand1).Value + ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((NumberNode)operand1).Value + ((OperationNode)operand2).Value;
            }
        }

        if (operand1 is OperationNode)
        {
            if (operand2 is StringNode)
            {
                return ((OperationNode)operand1).Value.ToString() + ((StringNode)operand2).Value;
            }
            if (operand2 is NumberNode)
            {
                return ((OperationNode)operand1).Value + ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((OperationNode)operand1).Value + ((OperationNode)operand2).Value;
            }
        }

        throw new Exception("Cannot apply operator + to operands of type " + operand1.GetType().Name + " and " + operand2.GetType().Name);
    };

    public readonly static Func<ValueNode[], dynamic> substract = delegate (ValueNode[] operands)
    {

        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand1 is NumberNode)
        {
            if (operand2 is NumberNode)
            {
                return ((NumberNode)operand1).Value - ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((NumberNode)operand1).Value - ((OperationNode)operand2).Value;
            }
        }

        if (operand1 is OperationNode)
        {
            if (operand2 is NumberNode)
            {
                return ((OperationNode)operand1).Value - ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((OperationNode)operand1).Value - ((OperationNode)operand2).Value;
            }
        }

       throw new Exception("Cannot apply operator - to operands of type " + operand1.GetType().Name + " and " + operand2.GetType().Name);
    };

    public readonly static Func<ValueNode[], dynamic> multiply = delegate (ValueNode[] operands)
    {
        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand1 is NumberNode)
        {
            if (operand2 is NumberNode)
            {
                return ((NumberNode)operand1).Value * ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((NumberNode)operand1).Value * ((OperationNode)operand2).Value;
            }
        }

        if (operand1 is OperationNode)
        {
            if (operand2 is NumberNode)
            {
                return ((OperationNode)operand1).Value * ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((OperationNode)operand1).Value * ((OperationNode)operand2).Value;
            }
        }

        throw new Exception("Cannot apply operator * to operands of type " + operand1.GetType().Name + " and " + operand2.GetType().Name);
    };

    public readonly static Func<ValueNode[], dynamic> divide = delegate (ValueNode[] operands)
    {
        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand2.Representation == "0")
        {
            throw new Exception("Cannot divide by zero");
        }

        if (operand1 is NumberNode)
        {
            if (operand2 is NumberNode)
            {
                return ((NumberNode)operand1).Value / ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((NumberNode)operand1).Value / ((OperationNode)operand2).Value;
            }
        }

        if (operand1 is OperationNode)
        {
            if (operand2 is NumberNode)
            {
                return ((OperationNode)operand1).Value / ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((OperationNode)operand1).Value / ((OperationNode)operand2).Value;
            }
        }

        throw new Exception("Cannot apply operator / to operands of type " + operand1.GetType().Name + " and " + operand2.GetType().Name);
    };

    public readonly static Func<ValueNode[], dynamic> power = delegate (ValueNode[] operands)
    {

        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand1 is NumberNode)
        {
            if (operand2 is NumberNode)
            {
                return Math.Pow(((NumberNode)operand1).Value, ((NumberNode)operand2).Value);
            }

            if (operand2 is OperationNode)
            {
                return Math.Pow(((NumberNode)operand1).Value, ((OperationNode)operand2).Value);
            }
        }

        if (operand1 is OperationNode)
        {
            if (operand2 is NumberNode)
            {
                return Math.Pow(((OperationNode)operand1).Value, ((NumberNode)operand2).Value);
            }

            if (operand2 is OperationNode)
            {
                return Math.Pow(((OperationNode)operand1).Value, ((OperationNode)operand2).Value);
            }
        }

        throw new Exception("Cannot apply operator ^ to operands of type " + operand1.GetType().Name + " and " + operand2.GetType().Name);
    };
}*/

public enum Precedence {
    Comma = 0,
    Assignement = 1,
    LogicalOR = 2,
    LogicalAND = 3,
    Equal = 4,
    NotEqual = 4,
    LessThan = 5,
    GreaterThan = 5,
    LessThanOrEqual = 5,
    GreaterThanOrEqual = 5,
    Addition = 6,
    Substraction = 6,
    Multiplication = 7,
    Division = 7,
    Modulo = 7,
    Power = 8,
    Unary = 9,
    FuncCall = 10,
    Access = 10,
    Array = 10,
}