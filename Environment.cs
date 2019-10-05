using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class Environment
{
    protected string name;

    public string Name {
        get => name;
    }

    protected Dictionary<string, ValueNode> variables;

    public ReadOnlyDictionary<string, ValueNode> Variables {
        get => new ReadOnlyDictionary<string, ValueNode>(variables);
    }

    protected List<FunctionDeclarationNode> functions;

    public ReadOnlyCollection<FunctionDeclarationNode> Functions {
        get => functions.AsReadOnly();
    }

    public Environment(string name, Dictionary<string, ValueNode> variables, ICollection<FunctionDeclarationNode> functions) {
        this.variables = new Dictionary<string, ValueNode>(variables);
        this.functions = new List<FunctionDeclarationNode>(functions);
        this.name = name;
    }

    public Environment(Environment environment) : this(environment.name, environment.variables, environment.functions) { }

    public bool HasVariable(string varName)
        => variables.ContainsKey(varName);

    public bool HasFunction(FunctionDeclarationNode node) {

        // I chose to not perform a simple `functions.Contains(node)` because it would do a "shallow comparison", which means that
        // functions with the same name and number of arguments would not be treated as equal if they did not have the same reference
        // (e.g. if they were created using different new() expressions).

        // And, even though I could use a custom comparer, it would add complexity for a simple problem that only occurs a couple of time.

        // Try to find a function with the same name and same number of arguments as the input function. If we can't find anything, then the result will be null.
        // So, we return true if the result is not null, and false otherwise.
        return functions.Find(func => func.Name.Representation == node.Name.Representation && func.Parameters.Count == node.Parameters.Count) != null;
    }

    public bool HasFunction(FunctionCallNode node)
        // Try to find a function with the same name and same number of arguments as input function. If we can't find anything, then the result will be null.
        // So, we return true if the result is not null, and false otherwise.
        => functions.Find(func => func.Name.Representation == node.Function.Representation && func.Parameters.Count == node.CallingParameters.Length) != null;

    public bool HasFunction(string functionName)
        // Try to find a function with the name `functionName`. If we can't find anything, then the result will be null.
        // So, we return true if the result is not null, otherwise we return false.
        => functions.Find(func => func.Name == functionName) != null;

    public bool HasFunction(string functionName, ComplexToken[] parameters) {

        // Try to find a function with the name `functionName` and the same number of arguments as `parameters.Count`. If we can't find anything, then the result
        // will be null. So, we return true if the result is not null, and false otherwise.
        return functions.Find(func => func.Name == functionName && func.Parameters.Count == parameters.Length) != null;
    }

    public ValueNode GetVariableValue(string varName) {

        // Try to get the value with the key `varName`, and output the result in the variable `output`.
        // If the method fails (i.e. there's no variable with this name), output will be set to null.
        variables.TryGetValue(varName, out ValueNode output);

        // return output
        return output;
    }

    public FunctionDeclarationNode GetFunction(FunctionCallNode node)
        // Find a function with the same name and parameter count as the input. If the search fails, it will return false
        => functions.Find(func => func.Name.Representation == node.Function.Representation && func.Parameters.Count == node.CallingParameters.Length);

    public void SetVariableValue(string varName, ValueNode newValue)
        => variables[varName] = newValue;

    public void RegisterVariable(string varName, ValueNode value)
        => variables.Add(varName, value);

    public void RegisterFunction(string functionName, ComplexToken[] parameters, SimpleBlock body)
        => RegisterFunction(new FunctionDeclarationNode(body, parameters, new ComplexToken(functionName, TokenKind.ident, null)));

    public void RegisterFunction(FunctionDeclarationNode node)
        => functions.Add(node);

    public bool TryRegisterVariable(string varName, ValueNode value)
        // The Dictionnary.TryAdd function tries to add an key/value pair to the dictionnary and returns true if the operation succeeded, and false otherwise
        => variables.TryAdd(varName, value);

    public bool TryRegisterFunction(string functionName, ComplexToken[] parameters, SimpleBlock body)
        // call TryRegisterFunction with a new FunctionDeclarationNode crafted from the arguments to this function
        => TryRegisterFunction(new FunctionDeclarationNode(body, parameters, new ComplexToken(functionName, TokenKind.ident, null)));

    public bool TryRegisterFunction(FunctionDeclarationNode node) {

        // if this function already exist, do nothing and return false
        if (HasFunction(node)) return false;

        // otherwise, add it to the list of functions
        functions.Add(node);

        // and return true
        return true;
    }

    public bool TrySetVariable(string varName, ValueNode newValue) {

        // if this variable doesn't exist yet, do nothing and return true
        if (!HasVariable(varName)) return false;

        // otherwise, set the value of the variable to `newValue`
        variables[varName] = newValue;

        // and return true
        return true;
    }
}