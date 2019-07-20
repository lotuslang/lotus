using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// TODO: Interpreter
// TODO: Implement built-in functions
// TODO: Allow loading of other files into the environment
public class Interpreter
{
    protected Parser parser;

    protected Environment environment;

    public Environment Environment {
        get => environment;
    }

    protected Interpreter() { }

    public Interpreter(IEnumerable<char> source, Environment environment) : this(new Parser(source), environment) { }

    public Interpreter(IEnumerable<string> source, Environment environment) : this(new Parser(new Tokenizer(source)), environment) { }

    public Interpreter(FileInfo file, Environment environment) : this(new Parser(file), environment) { }

    public Interpreter(Tokenizer tokenizer, Environment environment) : this(new Parser(tokenizer), environment) { }

    public Interpreter(Parser parser, Environment environment) : this(parser) {
        this.environment = new Environment(environment);
    }

    public Interpreter(IEnumerable<char> source) : this(new Parser(new Tokenizer(source))) { }

    public Interpreter(IEnumerable<string> source) : this(new Parser(new Tokenizer(source))) { }

    public Interpreter(FileInfo file) : this(new Parser(new Tokenizer(file))) { }

    public Interpreter(Tokenizer tokenizer) : this(new Parser(tokenizer)) { }

    public Interpreter(Parser parser) {
        this.parser = new Parser(parser);
        environment = new Environment(parser.Position.filename, new Dictionary<string, ValueNode>(), new FunctionDeclarationNode[0]);
    }

    public void Run() {
        var currNode = parser.Consume();

        if (currNode is AssignmentNode) {

            // the name of the variable being declared
            var varName = (currNode as AssignmentNode).Name;

            // the computed of the variable being declared
            var varValue = Compute((currNode as AssignmentNode).Value);

            if (!environment.HasVariable(varName)) {
                throw new Exception($"{parser.Position} : Variable {varName} is not declared in the current scope."
                    + $" Did you mean \"var {varName} = {varValue}\"");
            }
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
        }

        if (currNode is FunctionNode) {
            CallFunction(currNode as FunctionNode);
        }
    }

    public void RunAll() { }

    protected ValueNode Compute(ValueNode node) {

        if (node is NumberNode) return (node as NumberNode);

        if (node is StringNode) return (node as StringNode);

        if (node is IdentNode) {

        }

        if (node is FunctionNode) {
            return CallFunction(node as FunctionNode);
        }

        if (node is OperationNode) {
            var op = node as OperationNode;

            var computedOperands = new List<ValueNode>(from operand in op.Operands select Compute(operand));

            if (op.OperationType.StartsWith("unary")) {
                if (op.OperationType.EndsWith("Neg")) {

                }

                if (op.OperationType.EndsWith("Not")) {

                }

                if (op.OperationType.EndsWith("Pre")) {

                }

                if (op.OperationType.EndsWith("Post")) {

                }

                if (op.OperationType.EndsWith("Invoc")) {
                    
                }
            }

            if (op.OperationType.StartsWith("binary")) {
                if (op.OperationType.EndsWith("Add")) {

                }

                if (op.OperationType.EndsWith("Sub")) {

                }

                if (op.OperationType.EndsWith("Mul")) {

                }

                if (op.OperationType.EndsWith("Div")) {

                }

                if (op.OperationType.EndsWith("Pow")) {

                }

                if (op.OperationType.EndsWith("Access")) {

                }
            }

            if (op.OperationType.StartsWith("conditional")) {

            }
        }

        throw new Exception($"{parser.Position} : Unknown ValueNode type");
    }

    public ValueNode CallFunction(FunctionNode node) {
        return null;
    }

    protected static Environment GetEnvironmentFrom(string source) => GetEnvironmentFrom(new Parser(source));

    protected static Environment GetEnvironmentFrom(FileInfo file) => GetEnvironmentFrom(new Parser(file));

    protected static Environment GetEnvironmentFrom(Parser parser) {
        var interpreter = new Interpreter(parser);

        interpreter.RunAll();

        return interpreter.Environment;
    }
}