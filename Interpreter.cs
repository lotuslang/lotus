using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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

    public Interpreter(IConsumer<StatementNode> nodeConsumer, Environment environment) : this(new Parser(nodeConsumer), environment) { }

    public Interpreter(Parser parser, Environment environment) : this(parser) {
        this.environment = new Environment(environment);
    }

    public Interpreter(IEnumerable<char> source) : this(new Tokenizer(source)) { }

    public Interpreter(IEnumerable<string> source) : this(new Tokenizer(source)) { }

    public Interpreter(FileInfo file) : this(new Tokenizer(file)) { }

    public Interpreter(IConsumer<Token> tokenConsumer) : this(new Parser(tokenConsumer)) { }

    public Interpreter(IConsumer<StatementNode> nodeConsumer) : this(new Parser(nodeConsumer)) { }

    public Interpreter(Parser parser) {
        this.parser = new Parser(parser);
        environment = new Environment(parser.Position.filename, new Dictionary<string, ValueNode>(), new FunctionDeclarationNode[0]);

        environment.RegisterVariable("#return", ValueNode.NULL);
    }

    public Interpreter(Interpreter interpreter) : this(interpreter.parser, interpreter.environment) { }

    public void RunAll() {
        do {
            Run();
        } while (environment.GetVariableValue("#return") != null);
    }

    public void Run() {

        // if you can't consume a node
        if (!parser.Consume(out StatementNode currNode)) {

            // set the environment variable `#return` to the NULL ValueNode constant
            environment.SetVariableValue("#return", null);
            return;
        }

        if (currNode.Representation == "=") {

            Compute(currNode as OperationNode);

            return;
        }

        if (currNode is FunctionDeclarationNode funcDec) {

            var funcName = funcDec.Name.Representation;

            if (environment.HasFunction(funcDec)) {
                throw new Exception($"{parser.Position} : Function {funcName} could not be declared because"
                    + $" it was already registered with the same number of argument."
                    + $" You can only override a function of the same name with a different number of argument."
                );
            }

            environment.RegisterFunction(funcDec);

            return;
        }

        if (currNode is DeclarationNode declaration) {

            // the name of the variable being declared
            var varName = declaration.Name;

            // if the variable is already declared
            if (environment.HasVariable(varName)) {
                throw new Exception($"{parser.Position} : Variable {varName} could not be declared because"
                    + $" it was already registered."
                );
            }

            // the computed value of the variable being declared
            var varValue = Compute(declaration.Value);

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

            environment.SetVariableValue("#return", null);
            return;
        }

        environment.SetVariableValue("#return", null);
    }


    /// <summary>
    /// Compute a literal value for a ValueNode. If it's a function, calls the function and returns the value.
    /// </summary>
    /// <param name="node">The node to compute.</param>
    /// <returns>A constant value for `node`.</returns>
    public ValueNode Compute(ValueNode node, bool resolveName = true) {

        // if the node is a number, just return it
        if (node is NumberNode) return node;

        // if the node is a complex string, compute its constant value and return it
        if (node is ComplexStringNode) return ComputeString(node as ComplexStringNode);

        // if the node is a string, just return it
        if (node is StringNode) return node;

        // if the node is a boolean, just return it
        if (node is BoolNode) return node;

        // if the node is an identifier
        if (node is IdentNode varName) {

            if (!resolveName) return node;

            // if there's no variable with that name, throw
            if (!environment.HasVariable(varName)) {
                throw new Exception($"{varName.Token.Location} : Variable '{varName.Representation}' is not declared in the current scope.");
            }

            // otherwise, return the variable's value
            return environment.GetVariableValue(varName);
        }

        // if the node is a function call, call it and return the returned value
        if (node is FunctionCallNode func) return CallFunction(node as FunctionCallNode);

        // if the node is an operation (oh boy)
        if (node is OperationNode op) {

            if (op.OperationType == "arrayAccess") {

                if (!(Compute(op.Operands[0], false) is IdentNode arrayName)) throw new Exception("a");

                if (!(Compute(op.Operands[1]) is NumberNode index)) throw new Exception("b");

                if (!environment.TryGetVariableValue(arrayName, out ValueNode array)) throw new Exception("c");

                throw new Exception("yay");
            }

            // if the operation is a prefix operation (i.e. the operator is in front of the operand)
            if (op.OperationType.StartsWith("prefix")) {

                // if the 
                if (op.OperationType.EndsWith("Incr")) {

                    var variable = Compute(op.Operands[0], false);

                    // we can't use `operand` here because it has already been resolved (we applied Compute() to every operand of computedOperands, and `operand` comes from it),
                    // so it will never be IdentNode.
                    if (variable is IdentNode ident) {
                        if (!environment.HasVariable(ident.Representation)) {
                            //throw new UnknownNameException(ident);
                        }

                        var value = environment.GetVariableValue(ident);

                        // Because `operand` value is already resolved, we don't need to get the value of the variable.
                        if (!(value is NumberNode)) {
                            throw new InvalidOperationException(op, value, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((value as NumberNode).Value + 1, ident.Token));

                        return environment.GetVariableValue(ident);
                    }

                    throw new InvalidOperationException(op, op.Operands[0], "identifier");
                }

                if (op.OperationType.EndsWith("Decr")) {

                    var variable = Compute(op.Operands[0], false);

                    if (variable is IdentNode ident) {
                        if (!environment.HasVariable(ident.Representation)) {
                            //throw new UnknownNameException(ident);
                        }

                        var value = environment.GetVariableValue(ident);

                        if (!(value is NumberNode)) {
                            throw new InvalidOperationException(op, value, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((value as NumberNode).Value - 1, ident.Token));

                        return environment.GetVariableValue(ident);
                    }

                    throw new InvalidOperationException(op, op.Operands[0], "identifier");
                }

                // compute the operation's operand
                var operand = Compute(op.Operands[0]);

                // if the operation is a logical negation ('!' prefix operator)
                if (op.OperationType.EndsWith("Not")) {

                    // if the operand is a boolean
                    if (operand is BoolNode boolNode) {

                        // return a boolean with the opposite value (False if the value was True, True if the value was False).
                        return new BoolNode(!boolNode.Value, boolNode.Token);
                    }

                    // otherwise throw
                    throw new InvalidOperationException(op, operand, "boolean");
                }

                // if the operation is an unary minus ('-' prefix operator)
                if (op.OperationType.EndsWith("Neg")) {

                    // if the operand is a number
                    if (operand is NumberNode) {

                        // return the opposite of that number
                        return new NumberNode(-(operand as NumberNode).Value, operand.Token);
                    }

                    // otherwise throw
                    throw new InvalidOperationException(op, operand, "number");
                }

                // if the operation is an unary positive ('+' prefix operator)
                if (op.OperationType.EndsWith("Pos")) {

                    // if the operand is a number
                    if (operand is NumberNode) {

                        // return the same
                        return operand as NumberNode;
                    }

                    // otherwise throw
                    throw new InvalidOperationException(op, operand, "number");
                }
            }

            if (op.OperationType.StartsWith("postfix")) {

                // if the
                if (op.OperationType.EndsWith("Incr")) {

                    var variable = Compute(op.Operands[0], false);

                    // we can't use `operand` here because it has already been resolved (we applied Compute() to every operand of computedOperands, and `operand` comes from it),
                    // so it will never be IdentNode.
                    if (variable is IdentNode ident) {
                        if (!environment.HasVariable(ident.Representation)) {
                            //throw new UnknownNameException(ident);
                        }

                        var value = environment.GetVariableValue(ident.Representation);

                        // Because `operand` value is already resolved, we don't need to get the value of the variable.
                        if (!(value is NumberNode)) {
                            throw new InvalidOperationException(op, variable, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((value as NumberNode).Value + 1, ident.Token));

                        return value;
                    }

                    throw new InvalidOperationException(op, variable, "identifier");
                }

                if (op.OperationType.EndsWith("Decr")) {

                    var variable = Compute(op.Operands[0], false);

                    if (variable is IdentNode ident) {
                        if (!environment.HasVariable(ident.Representation)) {
                            //throw new UnknownNameException(ident);
                        }

                        var value = environment.GetVariableValue(ident);

                        if (!(value is NumberNode)) {
                            throw new InvalidOperationException(op, variable, "number");
                        }

                        environment.SetVariableValue(ident.Representation, new NumberNode((value as NumberNode).Value - 1, ident.Token));

                        return value;
                    }

                    throw new InvalidOperationException(op, variable, "identifier");
                }

                // compute the operation's operand
                var operand = Compute(op.Operands[0]);
            }

            if (op.OperationType.StartsWith("binary")) {

                // we need to check for access first because one or more the identifiers used will probably not resolve to a local variable or function,
                // so computing them first would throw an error
                /*if (op.OperationType.EndsWith("Access")) {
                    if (!(op.Operands[0] is IdentNode) || !(op.Operands[1] is IdentNode)) {
                        throw new InvalidOperationException(op, op.Operands[0], op.Operands[1], "identifier");
                    }
                }*/

                if (op.OperationType.EndsWith("Assign")) {

                    if (Compute(op.Operands[0], false) is IdentNode variable) {
                        if (!environment.HasVariable(variable)) {
                            //throw new UnknownNameException(variable);
                        }

                        var value = Compute(op.Operands[1]);

                        if (value == ValueNode.NULL) {
                            throw new InvalidOperationException(op, value);
                        }

                        environment.SetVariableValue(variable, value);

                        return ValueNode.NULL;
                    }

                    if (op.Operands[0].Representation == "[") {
                        return ValueNode.NULL;
                    }

                    throw new InvalidOperationException(op, op.Operands[0]);
                }

                // we take every operand in Op.Operands and compute their value
                var computedOperands = new List<ValueNode>(
                    from operand in op.Operands // we need to reverse the array because postfix inverted the operands
                    select Compute(operand)
                );

                var operand1 = computedOperands[0];
                var operand2 = computedOperands[1];

                if (op.OperationType.EndsWith("Add")) {
                    return Internals.Add(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Sub")) {
                    return Internals.Sub(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Mul")) {
                    return Internals.Mul(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Div")) {
                    return Internals.Div(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Pow")) {
                    return Internals.Pow(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Eq")) {
                    return Internals.Eq(operand1, operand2);
                }

                if (op.OperationType.EndsWith("NotEq")) {
                    return Internals.NotEq(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Or")) {
                    return Internals.Or(operand1, operand2);
                }

                if (op.OperationType.EndsWith("And")) {
                    return Internals.And(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Greater")) {
                    return Internals.Greater(operand1, operand2);
                }

                if (op.OperationType.EndsWith("GreaterOrEq")) {
                    return Internals.GreaterOrEq(operand1, operand2);
                }

                if (op.OperationType.EndsWith("Less")) {
                    return Internals.Less(operand1, operand2);
                }

                if (op.OperationType.EndsWith("LessOrEq")) {
                    return Internals.LessOrEq(operand1, operand2);
                }
            }

            throw new Exception($"{parser.Position} : Unknown operation {op.OperationType}");
        }

        // if we get there, there's a BIG problem
        throw new Exception($"{parser.Position} : Unknown ValueNode type {node.GetType()} ({node.Representation})");
    }

    protected StringNode ComputeString(ComplexStringNode node) {
        var str = node.Value;

        // compute every code section and convert them to a string
        var computedSections = (from section in node.CodeSections
                                select Convert.ToString(Compute(section)).Value).ToArray();

        // format the string using computedSections
        return new StringNode(String.Format(str, computedSections), node.Token);
    }

    public ValueNode CallFunction(FunctionCallNode node) {

        // compute each parameter of the function call
        var parameters = (from parameter in node.CallingParameters
                          select Compute(parameter)).ToArray();

        // if the function is an internal function
        if (Constants.internalFunctions.Contains(node.FunctionName.Representation)) {

            // the name of the function
            var funcName = node.FunctionName.Representation;

            // if the name is print
            if (funcName == "print") {
                return Internals.Print(parameters);
            }

            // ... implement more internal functions
        }

        // Check if the function exists in the current environnement, throw an error if not
        if (!environment.HasFunction(node)) {
            // TODO: Use a throw helper or create an appropriate exception
            throw new Exception($"{node.Token.Location} : Function '{node.FunctionName.Representation}' is not declared in the current scope.");
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
                funcEnvironment.SetVariableValue(func.Parameters[i].Representation, parameters[i]);
                continue;
            }

            // if the parameter isn't already a variable in the global scope, just register it with the value of the parameter
            funcEnvironment.RegisterVariable(func.Parameters[i].Representation, parameters[i]);
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

        // create a new interpreter from the parser
        var interpreter = new Interpreter(parser);

        // run the entire program
        interpreter.RunAll();

        // then return the final environment
        return interpreter.Environment;
    }
}