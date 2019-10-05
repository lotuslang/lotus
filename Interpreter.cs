using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

// TODO: Interpreter
// TODO: Implement built-in functions
// TODO: Allow loading of other files into the environment
public class Interpreter
{
    protected IConsumer<StatementNode> parser;

    protected Environment environment;

    public Environment Environment {
        get => environment;
    }

    protected Interpreter() { }

    public Interpreter(IEnumerable<char> source, Environment environment) : this(new Parser(source), environment) { }

    public Interpreter(IEnumerable<string> source, Environment environment) : this(new Parser(new Tokenizer(source)), environment) { }

    public Interpreter(FileInfo file, Environment environment) : this(new Parser(file), environment) { }

    public Interpreter(IConsumer<Token> tokenConsumer, Environment environment) : this(new Parser(tokenConsumer), environment) { }

    public Interpreter(IConsumer<StatementNode> nodeConsumer, Environment environment) : this(nodeConsumer) {
        this.environment = new Environment(environment);
    }

    public Interpreter(IEnumerable<char> source) : this(new Tokenizer(source)) { }

    public Interpreter(IEnumerable<string> source) : this(new Tokenizer(source)) { }

    public Interpreter(FileInfo file) : this(new Tokenizer(file)) { }

    public Interpreter(IConsumer<Token> tokenConsumer) : this(new Parser(tokenConsumer)) { }

    public Interpreter(IConsumer<StatementNode> nodeConsumer) {
        this.parser = new Consumer<StatementNode>(nodeConsumer);
        environment = new Environment(nodeConsumer.Position.filename, new Dictionary<string, ValueNode>(), new FunctionDeclarationNode[0]);
        environment.RegisterVariable("#return", null);
    }

    public Interpreter(Parser parser) {
        this.parser = new Parser(parser);
        environment = new Environment(parser.Position.filename, new Dictionary<string, ValueNode>(), new FunctionDeclarationNode[0]);
        environment.RegisterVariable("#return", null);
    }

    public Interpreter(Interpreter interpreter) : this(interpreter.parser, interpreter.environment) { }

    public void Run() {

        // if you can't consume a node
        if (!parser.Consume(out StatementNode currNode)) {

            // set the environment variable `#return` to the NULL ValueNode constants
            environment.SetVariableValue("#return", ValueNode.NULL);
            return;
        }

        if (currNode is AssignmentNode) {

            // the name of the variable being declared
            var varName = (currNode as AssignmentNode).Name;

            // the computed of the variable being declared
            var varValue = Compute((currNode as AssignmentNode).Value);

            if (!environment.HasVariable(varName)) {
                throw new Exception($"{parser.Position} : Variable {varName} is not declared in the current scope."
                    + $" Did you mean \"var {varName} = {varValue}\"");
            }
            return;
        }

        if (currNode is DeclarationNode) {

            // the name of the variable being declared
            var varName = (currNode as DeclarationNode).Name;

            // the computed value of the variable being declared
            var varValue = Compute((currNode as DeclarationNode).Value);

            // try to register the variable with its name and its computed value
            if (!environment.HasVariable(varName)) {
                throw new Exception($"{parser.Position} : Variable {varName} could not be declared because"
                    + $" it was already declared at line {parser.Position.line} in file {parser.Position.filename}."
                    + $" Did you mean \"{varName} = {varValue}\" ?");
            }

            // try to register the variable with its name and its computed value
            if (!environment.TryRegisterVariable(varName, varValue)) {
                throw new Exception($"{parser.Position} : Variable {varName} could not be declared");
            }

            return;
        }

        if (currNode is FunctionCallNode) {
            CallFunction(currNode as FunctionCallNode);
            return;
        }

        if (currNode is ReturnNode) {
            if ((currNode as ReturnNode).IsReturningValue) {
                environment.SetVariableValue("#return", Compute((currNode as ReturnNode).Value));
                return;
            }

            environment.SetVariableValue("#return", ValueNode.NULL);
            return;
        }
    }

    public void RunAll() {
        while (environment.GetVariableValue("#return") == null) {
            Run();
        }
    }

    public ValueNode Compute(ValueNode node) {

        if (node is NumberNode) return node;

        if (node is StringNode) return node;

        if (node is BoolNode)   return node;

        if (node is ComplexStringNode) return ComputeString(node as ComplexStringNode);

        if (node is IdentNode) {
            if (!environment.HasVariable(node.Representation)) {
                throw new Exception($"{node.Token.Location} : Variable '{node.Representation}' is not declared in the current scope.");
            }

            return environment.GetVariableValue(node.Representation);
        }

        if (node is FunctionCallNode) {
            var value = CallFunction(node as FunctionCallNode);

        }

        if (node is OperationNode) {
            var op = node as OperationNode;

            if (op.OperationType.StartsWith("unary")) {

                var operand = Compute(op.Operands[0]);

                if (op.OperationType.EndsWith("Not")) {
                    if (operand is BoolNode boolNode) {
                        return new BoolNode(!boolNode.Value, boolNode.Token);
                    }

                    throw new InvalidOperationException(op, operand, "boolean");
                }

                if (op.OperationType.EndsWith("Neg")) {
                    if (operand is BoolNode boolNode) {
                        return new BoolNode(!boolNode.Value, boolNode.Token);
                    }

                    if (operand is NumberNode) {
                        return new NumberNode(-(operand as NumberNode).Value, operand.Token);
                    }

                    throw new InvalidOperationException(op, operand, "number");
                }

                if (op.OperationType.EndsWith("Pre")) {
                    // we can't use `operand` here because it has already been resolved (we applied Compute() to every operand of computedOperands, and `operand` comes from it),
                    // so it will never be IdentNode.
                    if (op.Operands[0] is IdentNode ident) {
                        // Because `operand` value is already resolved, we don't need to get the value of the variable.
                        if (!(operand is NumberNode)) {
                            throw new InvalidOperationException(op, operand, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((operand as NumberNode).Value + 1, ident.Token));

                        return environment.GetVariableValue(ident.Representation);
                    }

                    throw new InvalidOperationException(op, op.Operands[0], "identifier");
                }

                if (op.OperationType.EndsWith("Post")) {
                    if (operand is IdentNode ident) {
                        if (!environment.HasVariable(ident.Representation)) {
                            //throw new UnknownVariableException(ident);
                        }
                        if (!(operand is NumberNode)) {
                            throw new InvalidOperationException(op, operand, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((operand as NumberNode).Value + 1, ident.Token));

                        return operand;
                    }

                    throw new InvalidOperationException(op, op.Operands[0], "identifier");
                }

                // throw if we don't know/support that operation
                throw new InvalidOperationException(op, operand);
            }

            if (op.OperationType.StartsWith("binary")) {

                // we need to check for access first because one or more the identifiers used will probably not resolve to a local variable or function,
                // so computing them first would throw an error
                if (op.OperationType.EndsWith("Access")) {
                    if (!(op.Operands[0] is IdentNode) || !(op.Operands[1] is IdentNode)) {
                        throw new InvalidOperationException(op, op.Operands[0], op.Operands[1], "identifier");
                    }
                }

                // we take every operand in Op.Operands and compute their value
                var computedOperands = new List<ValueNode>(
                    from operand in op.Operands.Reverse() // we need to reverse the array because postfix inverted the operands
                    select Compute(operand)
                );

                var operand1 = computedOperands[0];
                var operand2 = computedOperands[1];

                if (op.OperationType.EndsWith("Add")) {
                    Console.WriteLine("return Constants.Add(operand1, operand2)");
                }

                if (op.OperationType.EndsWith("Sub")) {
                    Console.WriteLine("return Constants.Sub(operand1, operand2)");
                }

                if (op.OperationType.EndsWith("Mul")) {
                    Console.WriteLine("return Constants.Mul(operand1, operand2)");
                }

                if (op.OperationType.EndsWith("Div")) {
                    Console.WriteLine("return Constants.Div(operand1, operand2)");
                }

                if (op.OperationType.EndsWith("Pow")) {
                    Console.WriteLine("return Constants.Pow(operand1, operand2)");
                }
            }

            if (op.OperationType.StartsWith("conditional")) {
                if (op.OperationType.EndsWith("Eq")) {

                }

                if (op.OperationType.EndsWith("NotEq")) {

                }

                if (op.OperationType.EndsWith("Or")) {

                }

                if (op.OperationType.EndsWith("And")) {

                }

                if (op.OperationType.EndsWith("Greater")) {

                }

                if (op.OperationType.EndsWith("GreaterOrEq")) {

                }

                if (op.OperationType.EndsWith("Less")) {

                }

                if (op.OperationType.EndsWith("LessOrEq")) {

                }
            }
        }

        throw new Exception($"{parser.Position} : Unknown ValueNode type");
    }

    protected StringNode ComputeString(ComplexStringNode node) {
        var str = node.Value;

        var strBuilder = new StringBuilder("");

        for (int i = 0; i < str.Length; i++) {
            if (str[i] != '{') {
                strBuilder.Append(str[i]);
            }

            if (i != 0) {
               if (str[i-1] == '\\') {
                    continue;
                }
            }

            if (i + 1 < str.Length && !Char.IsDigit(str[i + 1])) {
                throw new Exception("wut");
            }

            var sectionIndexRep = "";

            while (i < str.Length && Char.IsDigit(str[i])) {
                sectionIndexRep += str[i];
            }

            var sectionIndex = Int32.Parse(sectionIndexRep);

            Console.WriteLine("strBuilder.Append(Constants.ToString(Compute(node.CodeSections[sectionIndex])))");
        }

        return new StringNode(strBuilder.ToString(), node.Token);
    }

    public ValueNode CallFunction(FunctionCallNode node) {

        // Check if the function exists in the current environnement, throw an error if not
        if (!environment.HasFunction(node)) {
            // TODO: Use a throw helper or create an appropriate exception
            throw new Exception($"{node.Token.Location} : Function '{node.Function.Representation}' is not declared in the current scope.");
        }

        // gets the FunctionDeclarationNode object corresponding to the function called
        var func = environment.GetFunction(node);

        // creates a consumer of StatementNode from the body of the function, and then creates an interpreter from that consumer and the current environment
        var funcInterpreter = new Interpreter(new Consumer<StatementNode>(func.Value.Content), environment);

        // the environment of the function interpreter
        var funcEnvironment = funcInterpreter.Environment;

        // for each parameter of the function
        for (int i = 0; i < func.Parameters.Count; i++) {

            // if a variable with the same name as the parameter already exists in the global scope (i.e. funcEnvironment before we modify it),
            // we "shadow" it by replacing it's current value by the value of the parameter.
            // howeever, since we created a completely new environment from the current one, and not just a copy (i.e. it doesn't reference the same object),
            // we don't actually modify the value of the variable in the global scope
            if (funcEnvironment.Variables.ContainsKey(func.Parameters[i].Representation)) {
                funcEnvironment.SetVariableValue(func.Parameters[i].Representation, node.CallingParameters[i]);
                continue;
            }

            // if the parameter isn't already a variable in the global scope, just register it with the value of the parameter
            funcEnvironment.RegisterVariable(func.Parameters[i].Representation, node.CallingParameters[i]);
        }

        // after modifying the environment, we can evaluate the whole function body
        funcInterpreter.RunAll();

        // once we finished the body, we return the value of `#return` (see docs for more info on `#return`)
        return funcEnvironment.GetVariableValue("#return");
    }

    protected static Environment GetEnvironmentFrom(string source)
        => GetEnvironmentFrom(new Parser(source));

    protected static Environment GetEnvironmentFrom(FileInfo file)
        => GetEnvironmentFrom(new Parser(file));

    protected static Environment GetEnvironmentFrom(Parser parser) {
        var interpreter = new Interpreter(parser);

        interpreter.RunAll();

        return interpreter.Environment;
    }
}