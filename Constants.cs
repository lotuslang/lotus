using System;
using System.Linq;
using System.Collections.Generic;

public static class Operators
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

        if (operand1 is NumberNode)
        {
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
            if (operand2 is NumberNode)
            {
                return ((OperationNode)operand1).Value + ((NumberNode)operand2).Value;
            }

            if (operand2 is OperationNode)
            {
                return ((OperationNode)operand1).Value + ((OperationNode)operand2).Value;
            }
        }

        throw new NotImplementedException("Unknow ValueNode type");
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

        throw new NotImplementedException("Unknow ValueNode type");
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

        throw new NotImplementedException("Unknow ValueNode type");
    };

    public readonly static Func<ValueNode[], dynamic> divide = delegate (ValueNode[] operands)
    {
        if (operands.Length != 2)
        {
            throw new ArgumentException("Unexpected number of arguments", nameof(operands));
        }

        var operand1 = operands[0];
        var operand2 = operands[1];

        if (operand1.Representation == "0" || operand2.Representation == "0")
        {
            Console.WriteLine("Dividing by zero is not possible");
            return null;
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

        throw new NotImplementedException("Unknow ValueNode type");
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

        throw new NotImplementedException("Unknow ValueNode type");
    };
}