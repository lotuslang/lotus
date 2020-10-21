using System;
using System.Linq;
using System.Text;

public sealed class ValuePrinter : NodeVisitor<string>
{

    protected override string Default(StatementNode node)
        => throw Logger.Fatal(new InvalidCallException("ValuePrinter cannot print statement like " + node.GetType().Name, node.Token.Location));

    protected override string Default(ValueNode node)
        => ASTHelper.PrintToken(node.Token);


    public override string Visit(BoolNode node)
        => ASTHelper.PrintToken(node.Token);

    public override string Visit(ComplexStringNode node) {
        var output = new StringBuilder("\"");

        var str = node.Value;

        // please don't ask, i was "in the flow" (and really hungry too so goodbye)
        for (var i = 0; i < node.Value.Length; i++) {
            if (str[i] != '{' && str[i] != '\\') {
                output.Append(str[i]);
                continue;
            }

            if (i + 1 < str.Length && str[i+1] == '{') {
                output.Append(str[i]);
                continue;
            }

            // This code does three things :
            // 1. Extract the code section this piece of the string refers to
            // 2. Pretty print that code section
            // 3. Increment `i` by the right amount so that we don't double-parse or ignore anything

            // Example
            // Let's say we have the string "hello {name} !"
            // It will get translated by the tokenizer and parser as
            // ComplexStringNode("hello {0} !", codeSections = [ IdentNode("name") ])
            // and we have just lost the information of where `name` was in the string
            //
            // To pretty-print it, we have to put back "name" into its place while not ignoring any other character.
            // To do so, we first add every character before the "{0}" part to `output`.
            //
            // Note : the "{0}" part is called a "section marker" or simply a marker.
            //
            // 1. Then, when we arrive on '{', we initialize a counter that tracks how many digits this marker contains
            // In our case, it contains only 1 digit, so we have digitCount = 1. Finally, we extract the index from the
            // marker, by extracting a string (`sectionIndexStr`) and then parsing it into a number (`sectionIndex`).
            //
            // 2. Once we have the index, we can pretty-print the code section (`sectionStr`)
            //
            // 3. To clean-up, we have to increment `i` by the number of characters we handled ourselves (instead of through the main loop).
            // Since we consumed the index (which has digitCount = 1) and we don't want to print the closing '}' at the end of the marker,
            // we increment `i` by the number of digits + 1, to account for '}'

            var digitCount = 0;

            // increment digitCount while the character is not '}'
            while (str[digitCount + 1 + i] != '}') digitCount++;

            // extract the digits between '{' and '}'
            var sectionIndexStr = str.AsSpan(i + 1, digitCount);

            // parse the digit string into the index of code section this refers to.
            var sectionIndex = Int32.Parse(sectionIndexStr);

            // pretty-print the code section
            var sectionStr = Print(node.CodeSections[sectionIndex]);

            output.Append("{" + sectionStr + "}");

            i += digitCount + 1; // the `+ 1` is to account for the closing "}" at the end
        }

        return output.ToString() + '"';
    }

    public override string Visit(FunctionCallNode node)
        => Print(node.FunctionName)
         + ASTHelper.PrintToken(node.ArgList.OpeningToken)
         + Utilities.Join(",", Print, node.ArgList)
         + ASTHelper.PrintToken(node.ArgList.ClosingToken);

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

    public override string Visit(ParenthesizedValueNode node)
        => ASTHelper.PrintToken(node.OpeningToken) + Print(node.Value) + ASTHelper.PrintToken(node.ClosingToken);


    public override string Visit(TupleNode node) {
        var output = ASTHelper.PrintToken(node.OpeningToken);

        foreach (var value in node.Values) output += Print(value) + ",";

        return output + ASTHelper.PrintToken(node.ClosingToken);
    }

    public override string Visit(SimpleBlock block)
        => throw Logger.Fatal(new InvalidCallException("ValuePrinter cannot print blocks", block.Location));


    public string Print(ValueNode node) => node.Accept(this);
}