using System;
using System.Linq;
using System.Collections.Generic;

internal static class Internals
{
    #region Add

    internal static ValueNode Add(ValueNode op1, ValueNode op2) {

        if (op1 is BoolNode || op2 is BoolNode) throw new Exception("cannot add");

        // if op1 is a string, convert op2 to a string and add them
        if (op1 is StringNode str1) {
            return Add(str1, Convert.ToString(op2));
        }

        // if op2 is a string, convert op1 to a string and add them
        if (op2 is StringNode str2) {
            return Add(Convert.ToString(op1), str2);
        }

        // since we know that neither op1 nor op2 is a string, we don't need to check again for that

        // if op1 is a number, convert op2 to a number and add them
        if (op1 is NumberNode n1) {
            return Add(n1, Convert.ToNumber(op2));
        }

        // if op2 is a number, convert op1 to a number and add them
        if (op2 is NumberNode n2) {
            return Add(Convert.ToNumber(op1), n2);
        }

        throw new Exception("cannot add");
    }

    private static NumberNode Add(NumberNode n1, NumberNode n2) {
        return new NumberNode(n1.Value + n2.Value, n1.Token);
    }

    private static StringNode Add(StringNode str1, StringNode str2) {
        return new StringNode(str1.Representation + str2.Representation, str1.Token);
    }

    #endregion

    #region Sub

    internal static NumberNode Sub(ValueNode op1, ValueNode op2) {
        if (!(op1 is NumberNode && op2 is NumberNode)) {
            throw new Exception("cannot subtract");
        }

        return Sub((NumberNode)op1, (NumberNode)op2);
    }

    private static NumberNode Sub(NumberNode n1, NumberNode n2) {
        return new NumberNode(n1.Value - n2.Value, n1.Token);
    }

    #endregion

    #region Mul

    internal static NumberNode Mul(ValueNode op1, ValueNode op2) {
        if (!(op1 is NumberNode && op2 is NumberNode)) {
            throw new Exception("cannot multiply");
        }

        return Mul((NumberNode)op1, (NumberNode)op2);
    }

    private static NumberNode Mul(NumberNode n1, NumberNode n2) {
        return new NumberNode(n1.Value * n2.Value, n1.Token);
    }

    #endregion

    #region Div

    internal static NumberNode Div(ValueNode op1, ValueNode op2) {
        if (!(op1 is NumberNode && op2 is NumberNode)) {
            throw new Exception("cannot multiply");
        }

        return Div((NumberNode)op1, (NumberNode)op2);
    }

    private static NumberNode Div(NumberNode n1, NumberNode n2) {

        if (n2.Value == 0) throw new Exception("Cannot divide by zero");

        return new NumberNode(n1.Value / n2.Value, n1.Token);
    }

    #endregion

    #region Pow

    internal static NumberNode Pow(ValueNode op1, ValueNode op2) {
        if (!(op1 is NumberNode && op2 is NumberNode)) {
            throw new Exception("cannot multiply");
        }

        return Pow((NumberNode)op1, (NumberNode)op2);
    }

    private static NumberNode Pow(NumberNode n1, NumberNode n2) {
        return new NumberNode(Math.Pow(n1.Value, n2.Value), n1.Token);
    }

    #endregion

    #region Boolean operations

    internal static BoolNode Eq(ValueNode op1, ValueNode op2) {
        return new BoolNode(op1.Value == op2.Value, op1.Token);
    }

    internal static BoolNode NotEq(ValueNode op1, ValueNode op2) {
        return new BoolNode(op1.Value != op2.Value, op1.Token);
    }

    internal static BoolNode Greater(ValueNode op1, ValueNode op2) {

        if (op1 is NumberNode n1 && op2 is NumberNode n2) {
            return new BoolNode(n1.Value > n2.Value, op1.Token);
        }

        throw new Exception();
    }

    internal static BoolNode GreaterOrEq(ValueNode op1, ValueNode op2) {

        if (op1 is NumberNode n1 && op2 is NumberNode n2) {
            return new BoolNode(n1.Value >= n2.Value, op1.Token);
        }

        throw new Exception();
    }

    internal static BoolNode Less(ValueNode op1, ValueNode op2) {

        if (op1 is NumberNode n1 && op2 is NumberNode n2) {
            return new BoolNode(n1.Value < n2.Value, op1.Token);
        }

        throw new Exception();
    }

    internal static BoolNode LessOrEq(ValueNode op1, ValueNode op2) {

        if (op1 is NumberNode n1 && op2 is NumberNode n2) {
            return new BoolNode(n1.Value <= n2.Value, op1.Token);
        }

        throw new Exception();
    }

    internal static BoolNode Or(ValueNode op1, ValueNode op2) {

        if (op1 is BoolNode b1 && op2 is BoolNode b2) {
            return new BoolNode(b1.Value || b2.Value, op1.Token);
        }

        throw new Exception();
    }

    internal static BoolNode And(ValueNode op1, ValueNode op2) {

        if (op1 is BoolNode b1 && op2 is BoolNode b2) {
            return new BoolNode(b1.Value || b2.Value, op1.Token);
        }

        throw new Exception();
    }

    #endregion

    #region Functions

    internal static ValueNode Print(ValueNode[] args) {
        if (args.Length == 0) Console.WriteLine();

        if (args.Length == 1) Console.WriteLine(args[0].Value);

        if (args.Length > 1) {
            var strBuilder = new System.Text.StringBuilder();

            foreach (var arg in args) {
                strBuilder.Append(arg.Value);
            }

            Console.WriteLine(strBuilder.ToString());
        }

        return ValueNode.NULL;
    }

    #endregion
}