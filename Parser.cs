using System;
using System.Linq;
using System.Collections.Generic;

public class Parser : IConsumer<StatementNode>
{
    private Queue<StatementNode> reconsumeQueue;

    private IConsumer<Token> tokenizer;

    public Location Position {
        get => tokenizer.Position;
    }

    protected StatementNode current;

    /// <summary>
    /// Gets the last StatementNode object consumed by this instance.
    /// </summary>
    /// <value>The last StatementNode consumed.</value>
    public StatementNode Current {
        get => current;
    }

    public Parser(Tokenizer tokenizer) {
        this.tokenizer = new Tokenizer(tokenizer);
        reconsumeQueue = new Queue<StatementNode>();
    }

    public Parser(IConsumer<Token> tokenConsumer) {
        this.tokenizer = tokenConsumer;
        reconsumeQueue = new Queue<StatementNode>();
    }

    public Parser(IConsumer<StatementNode> nodeConsumer) {
        reconsumeQueue = new Queue<StatementNode>();

        while (nodeConsumer.Consume(out _)) {
            reconsumeQueue.Enqueue(nodeConsumer.Current);
        }
    }

    public Parser(IConsumer<Token> tokenConsumer) : this(new Tokenizer(tokenConsumer)) { }

    public Parser(StringConsumer consumer) : this(new Tokenizer(consumer)) { }

    public Parser(IEnumerable<char> collection) : this(new Tokenizer(collection)) { }

    public Parser(System.IO.FileInfo file) : this(new Tokenizer(file)) { }

    public Parser(Parser parser) : this(parser.tokenizer) { }

    /// <summary>
    /// Reconsumes the last StatementNode object.
    /// </summary>
    public void Reconsume() {
        if (reconsumeQueue.TryPeek(out StatementNode node) && Object.ReferenceEquals(node, current)) return;
    }

    public StatementNode Peek()
        => new Parser(this).Consume();

    public StatementNode[] Peek(int n) {
        var parser = new Parser(this);

        var output = new List<StatementNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    public StatementNode Peek() => new Parser(this).Consume();

    public StatementNode[] Peek(int n) {
        var parser = new Parser(this);

        var output = new List<StatementNode>();

        for (int i = 0; i < n; i++) {
            output.Add(parser.Consume());
        }

        return output.ToArray();
    }

    /// <summary>
    /// Consumes a StatementNode object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise.</param>
    /// <returns>The StatementNode object consumed.</returns>
    public bool Consume(out StatementNode result) {
        result = Consume(); // consume a StatementNode

        return result != null;
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public StatementNode Consume() {

        // If we are instructed to reconsume the last node, then dequeue a node from the reconsumeQueue and return it
        if (reconsumeQueue.Count != 0) {
            return reconsumeQueue.Dequeue();
        }

        // Consume a token
        var currToken = tokenizer.Consume();

        // if the token is EOF, return null
        // TODO: find an alternative to null
        if (currToken == TokenKind.EOF) return null;

        // if the token is ';', return the next statement node
        if (currToken == ";") return Consume();

        // if the token is "var"
        if (currToken == "var") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a declaration and return it
            current = ConsumeDeclaration();

            return current;
        }

        // if the token is "new"
        if (currToken == "new") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a value and return it (since an object instantiation is just a fancy function call, and function calls can be parsed as values)
            current = ConsumeValue();

            return current;
        }

        // if the token is "def"
        if (currToken == "def") {

            // reconsume it
            tokenizer.Reconsume();

            // consume a function declaration and return it
            current = ConsumeFunctionDeclaration();

            return current;
        }

        if (currToken == "return") {

            // reconsume it
            tokenizer.Reconsume();

            current = ConsumeReturn();

            return current;
        }

        // if the token is an identifier
		if (currToken == TokenKind.ident) {

			// if it's an identifier followed by '(', suppose it's a function call
			if (tokenizer.Peek() == "(") {

                // reconsume the identifier
                tokenizer.Reconsume();

                /* // consume a value (which is gonna be an OperationNode representing a function)
                var paramsAsValue = ConsumeValue();

                if (!(paramsAsValue is OperationNode)) throw new Exception($"{currToken.Location} : Failed to parse function call");

                // consume a value, create a FunctionNode with its value set to the ValueNode returned, and return it
				current = new FunctionNode(((OperationNode)paramsAsValue).Operands, currToken as ComplexToken);*/

                current = ConsumeValue();

                return current;
			}

            // if the next token is '='
			if (tokenizer.Peek() == "=") {

                // consume it
				tokenizer.Consume();

                // consume a value, create an AssignmentNode with its value set to the ValueNode returned, and return it
				current = new AssignmentNode(ConsumeValue(), currToken as ComplexToken);

                return current;
			}

            // if the next token is "."
            if (tokenizer.Peek() == ".") {

                // reconsume the identifier
                tokenizer.Reconsume();

                // consume a value
                current = ConsumeValue();

                // if the value returned is not an OperationNode, throw an exception
                if (!(current is OperationNode)) throw new Exception($"Unknown error at {currToken.Location}");

                // return the value otherwise
                return current;
            }

			throw new UnexpectedTokenException($"Did you mean to assign a value or call a function ?", currToken);
		}

        throw new Exception("what²²");
    }

    protected DeclarationNode ConsumeDeclaration() {

        // consume the keyword "var" token
        var varKeyword = tokenizer.Consume();

        // if the token isn't the keyword "var", throw an exception
        if (varKeyword != "var") throw new UnexpectedTokenException(varKeyword, "in declaration", "var");

        // consume the name of the variable we're declaring
        var name = tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (name != TokenKind.ident) throw new UnexpectedTokenException(name, TokenKind.ident);

        // consume a token
        var equalSign = tokenizer.Consume();

        // if this token wasn't an equal sign, throw an exception
        if (equalSign != "=") throw new UnexpectedTokenException(equalSign, "=");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = ConsumeValue();

        // return that value
        return new DeclarationNode(value, name as ComplexToken);
    }

    protected FunctionDeclarationNode ConsumeFunctionDeclaration() {

        // consume the keyword "def" token
        var defKeyword = tokenizer.Consume();

        // if the token consumed was not "def", then throw an exception
        if (defKeyword != "def") throw new UnexpectedTokenException("A function definition must start with the keyword 'def'", defKeyword);

        // consume the name of the function
        var funcName = tokenizer.Consume();

        // if the token consumed was not an identifier, then throw an exception
        if (funcName != TokenKind.ident) throw new UnexpectedTokenException(funcName, "in function declaration", TokenKind.ident);

        // the list of the parameters the function has
        var parameters = new List<ComplexToken>();

        // consume a token
        var parenthesis = tokenizer.Consume();

        // if it wasn't an open parenthesis, throw an exception
        if (parenthesis != "(") throw new UnexpectedTokenException(parenthesis, "in function declaration", "(");

        // consume a token
        var paramName = tokenizer.Consume();

        // while this token is not a parenthesis
        while (paramName != ")") {

            // if the token is not an identifier, throw an exception
            if (paramName != TokenKind.ident) throw new UnexpectedTokenException(paramName, "in function argument declaration", TokenKind.ident);

            // add the parameter (as a ComplexToken) to the list of parameters
            parameters.Add(paramName as ComplexToken);

            // consume a token
            var comma = tokenizer.Consume();

            // if the token was a closing parenthesis, break out
            if (comma == ")") break;

            // if the token was not a comma, throw an exception
            if (comma != ",") throw new UnexpectedTokenException(comma, "in function argument declaration", ",");

            // assign paramName to a new token
            paramName = tokenizer.Consume();
        }

        // consume a simple block
        var block = ConsumeSimpleBlock();

        // return a new FunctionDeclarationNode with 'block' as the value, 'parameters' as the list of params, and funcName as the name
        return new FunctionDeclarationNode(block, parameters.ToArray(), funcName as ComplexToken);
    }

    protected SimpleBlock ConsumeSimpleBlock() {

        var bracket = tokenizer.Consume();

        if (bracket != "{") throw new UnexpectedTokenException(bracket, "at the start of simple block", "{");

        var statements = new List<StatementNode>();

        while (tokenizer.Peek() != "}" && tokenizer.Peek() != TokenKind.EOF) {
            statements.Add(Consume());
            if (tokenizer.Peek() == ";") tokenizer.Consume();
        }

        bracket = tokenizer.Consume();

        if (bracket == ";") bracket = tokenizer.Consume();

        if (bracket != "}") throw new Exception("what² (" + bracket.Representation + ")");

        return new SimpleBlock(statements.ToArray());
    }

    protected ReturnNode ConsumeReturn() {
        var returnToken = tokenizer.Consume();

        if (returnToken != "return") throw new UnexpectedTokenException(returnToken, "in return statement", "return");

        if (tokenizer.Peek() == ";") return new ReturnNode(null, returnToken as ComplexToken);

        var value = ConsumeValue();

        return new ReturnNode(value, returnToken as ComplexToken);
    }

    protected ValueNode ConsumeValue() {

        // converts to postfix notation
        var postfix = ToPostfixNotation(tokenizer);

#if DEBUG
    Console.WriteLine(String.Join(" ", postfix));
#endif // DEBUG


        // postfix notation cannot contain an even number of tokens (since )
        if (postfix.Count == 0) return null;

        // if only one value could be parsed
        if (postfix.Count == 1) {
            var token = postfix[0];

            if (token == "}") {
                return new ValueNode("}", token);
            }

            // if the token is an identifier, return an IdentNode
            if (token == TokenKind.ident) {
                return new IdentNode(token.Representation, token);
            }

            // if the token is a string, return a StringNode
            if (token == TokenKind.@string) {
                return new StringNode(token.Representation, token);
            }

            // if the token is a number, return a NumberNode
            if (token is NumberToken number) {
                return new NumberNode(number.Value, token);
            }

            // if the token is a bool, return a BoolNode
            if (token is BoolToken boolToken) {
                return new BoolNode(boolToken.Value, token);
            }

            throw new UnexpectedTokenException(token, TokenKind.@string, TokenKind.ident, TokenKind.number, TokenKind.function);
        }

        // otherwise, transform the postfix expression into a tree (we return the root of that tree)

        var operands = new Stack<ValueNode>();

        for (int i = 0; i < postfix.Count; i++)
        {
            var token = postfix[i];

            // if the token is a number, push a number node to the operands stack
            if (token is NumberToken number) {
                operands.Push(new NumberNode(number.Value, number));
                continue;
            }

            // if the token is a bool, push a bool node to the operands stack
            if (token is BoolToken boolToken) {
                operands.Push(new BoolNode(boolToken.Value, boolToken));
            }

            // if the token is an identifier, push a identifier node to the operands stack
            if (token == TokenKind.ident) {
                operands.Push(new IdentNode(token, token));
                continue;
            }

            // if the token is a string, push a string node to the operands stack
            if (token == TokenKind.@string || token == TokenKind.complexString) {
                operands.Push(new StringNode(token, token));
                continue;
            }

            // The '[' character is used delimit function calls. When we encounter it, we push it to the operands stack.
            // Then, when a function "collects" its arguments, it will stop at the first '[' operand it founds.
            // if the token is '[', push it to the operands stack
            if (token == "[") {
                operands.Push(new ValueNode("[", token));
                continue;
            }

            if (token is OperatorToken) {

                ValueNode op;

                switch (token)
                {
                    case "+":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binaryAdd");
                        break;
                    case "-":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binarySub");
                        break;
                    case "*":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binaryMul");
                        break;
                    case "/":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binaryDiv");
                        break;
                    case "^":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binaryPow");
                        break;
                    case ".":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "binaryAccess");
                        break;
					case "=":
						throw new Exception($"Unexpected assignment in value at location {token.Location}.");
                    case "_":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop() }, "unaryNeg");
                        break;
                    case "!":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop() }, "unaryNot");
                        break;
                    case "++":
                    case "--":
                        // if the operator is left associative, it's a postfix ++/--, otherwise, it's a prefix ++/--
                        op = new OperationNode(token, new ValueNode[] { operands.Pop() }, (token as OperatorToken).IsLeftAssociative ? "unaryPost" : "unaryPre");
                        break;
                    case "||":
		    			op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionOr");
						break;
					case "&&":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionAnd");
						break;
					case "!=":
						op = new OperationNode(token, new ValueNode[] {operands.Pop(), operands.Pop() }, "conditionalNotEq");
						break;
					case "==":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionalEq");
						break;
					case ">=":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionalGreaterOrEq");
						break;
					case "<=":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionalLessOrEq");
						break;
					case ">":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionalGreater");
						break;
					case "<":
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop() }, "conditionalLess");
						break;
                    /* case "new":
                        op = new OperationNode(token, new ValueNode[] { operands.Pop() }, "unaryInit");
                        break;*/
                    case "var":
                        throw new Exception($"Unexpected variable declaration at location {token.Location}.");
                    case "def":
                        throw new Exception($"Unexpected function definition at location {token.Location}.");
                    default: // if the operator is none of the above, then assume it is a function
                        var funcOperands = new List<ValueNode>();

						// while the next operand is not "[" (the delimiter for function calls)
                        while (operands.Peek().Representation != "[")
							// pop operands from the operand stack and add them to the function operands
                            funcOperands.Add(operands.Pop());

						// pop the "[" remaining
                        operands.Pop();

                        op = new FunctionCallNode(funcOperands.ToArray(), new ComplexToken(token, TokenKind.ident, token.Location));
                        break;
                }

                operands.Push(op);
                continue;
            }
        }

        return operands.Pop();
    }

    /// <summary>
    /// This is an implementation of the shunting-yard algorithm by Edsger Dijkstra.
    /// It takes in a tokenizer from which it will consume tokens, and outputs a list of Token,
    /// representing the input operation in Postfix Notation (also called Reverse Polish Notation).
    /// It can process numbers, identifiers, functions, and operators. It was a bit modified for convenience.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to consume the tokens from.</param>
    /// <returns>A list of tokens representing the input operation in Postfix Notation (a.k.a. Reverse Polish Notation).</returns>
    public static List<Token> ToPostfixNotation(IConsumer<Token> tokenizer) {

        // the output queue
        var output = new List<Token>();

        // the operator stack
        var operatorStack = new Stack<OperatorToken>();

        // the current token
        Token currToken = null;

        // the last token consumed
        Token lastToken;

        // variable to check if there are tokens left
        bool areTokensLeft = true;

        while (areTokensLeft) {

            // basically sets the "lastToken" variable to the "currToken" before it is updated
            lastToken = currToken;

            /// consume a token
            areTokensLeft = tokenizer.Consume(out currToken);

            // this this to check if an identifier is a function, since it is not done during tokenizing.

            // if the token is an identifier
            if (currToken == TokenKind.ident) {
                if (tokenizer.Peek() == "(") {

                    // set the current token to an identical token but identified as TokenKind.function
                    currToken = new ComplexToken(currToken, TokenKind.function, currToken.Location);
                }
            }

            // if the token is a number, a string, or an identifier
            if (currToken is NumberToken
            ||  currToken is BoolToken
            ||  currToken == TokenKind.@string
            ||  currToken == TokenKind.complexString
            ||  currToken == TokenKind.ident) {

                // add it to the output list
                output.Add(currToken);
                continue;
            }

            // if the token is a function
            if (currToken == TokenKind.function) {

                // push it to the operator stack
                operatorStack.Push(new OperatorToken(currToken, Precedence.FuncCall, "right", currToken.Location));
                output.Add(new Token('[', TokenKind.delim, tokenizer.Peek().Location));
                continue;
            }

            // if the token is a left parenthesis ('(')
            if (currToken == "(") {

                // push it to the operator stack
                operatorStack.Push(new OperatorToken(currToken, 0, "right", currToken.Location));
                continue;
            }

            // if the token is a right parenthesis (')')
            if (currToken == ")") {

                try {
                        // while the operator at the top of the operator stack is not a left parenthesis
                    while (operatorStack.Peek() != "(") {

                        // pop an operator from the operator stack and add it to the output
                        output.Add(operatorStack.Pop());
                    }

                    operatorStack.Pop();
                } catch {

                    // if no left parenthesis was found, there are mismatched parenthesis
                    throw new Exception($"Mismatched parenthesis at location {currToken.Location}");
                }

                //
				// TODO: Implement type casting handling
                //

                continue;
            }

            // if the token is '.'
            if (currToken == ".") {

                // set the current token to a left-associative operator with precedence Precedence.Access
                currToken = new OperatorToken(currToken, Precedence.Access, "left", currToken.Location);
            }

            // if the token is "++" or "--"
            if (currToken == "++" || currToken == "--") {

                // if the last token was an identifier
                if (lastToken == TokenKind.ident) {

                    // set the current token to a left-associative operator with precedence Precedence.Unary
                    currToken = new OperatorToken(currToken, Precedence.Unary, "left", currToken.Location);
                }
                else if (tokenizer.Peek() == TokenKind.ident) {

                    // set the current token to a right-associative operator with precedence Precedence.Unary
                    currToken = new OperatorToken(currToken, Precedence.Unary, "right", currToken.Location);
                }
                else throw new Exception($"Increment/Decrement operator with no associated identifier at location {currToken.Location}");
            }

            // if the token is an operator token
            if (currToken is OperatorToken) {

                // if the last token was an operator and the next token is an identifier, or the last token was a right parenthesis, a right parenthesis it means this operator is unary
                if (lastToken == null || ((lastToken is OperatorToken && tokenizer.Peek() == TokenKind.ident) || lastToken == "(" || tokenizer.Peek() == "(")) {

                    // if the current token is "-"
                    if (currToken == "-") {
                        currToken = new OperatorToken("_", Precedence.Unary, "right", currToken.Location);
                    }

                    // if the current token is "+", we skip it (since it is redundant)
                    if (currToken == "+") {
                        continue;
                    }
                }

                /*
                * while
                *       (
                *               (the operator at the top of the operator stack is a function)
                *           or  (the operator at the top of the operator stack has greater precedence)
                *           or  (
                *                       it has equal precedence
                *                   and is left associative
                *               )
                *       )
                *   and it is not a left parenthesis
                */

                /* Or :
                * while (   (the operator at the top of the operator stack is a function)
                *           or (the operator at the top of the operator stack has greater precedence)
                *           or ((the operator at the top of the operator stack has equal precedence)
                *               and (the operator at the top of the operator stack is left associative))
                *       and (the operator at the top of the operator stack is not a left parenthesis)
                *
                */

                while (
                    (
                        // the stack is not empty
                        operatorStack.Count != 0
                    )
                    && // and
                    (
                        (
                            // the operator is a function
                            operatorStack.Peek() == TokenKind.function
                        )
                        || // or
                        (
                            // the operator has greater precedence
                            operatorStack.Peek().Precedence > (currToken as OperatorToken).Precedence
                        )
                        || // or
                        (
                            (
                                // the operator has equal precedence
                                operatorStack.Peek().Precedence == (currToken as OperatorToken).Precedence
                            )
                            && // and
                            (
                                // the operator is left associative
                                operatorStack.Peek().IsLeftAssociative
                            )
                        )
                    )
                    && // and
                    (
                        // the operator is not a left parenthesis
                        operatorStack.Peek() != "("
                    )
                )

                {
                    // pop the operator from the stack and add it to the output
                    output.Add(operatorStack.Pop());
                }

                // push the token to the stack
                operatorStack.Push(currToken as OperatorToken);
                continue;
            }

            /*if (currToken == ",") {
                while (operatorStack.Peek() != "(") {
                    output.Add(operatorStack.Pop());
                }
			}*/

            // if this is the end of the statement, reconsume the token and break
            if (currToken == ";") {
                tokenizer.Reconsume();
                break;
            }

            if (currToken == TokenKind.EOF) {
                tokenizer.Reconsume();
                break;
            }

            output.Add(currToken);
        }

        // add every operator in the operator stack to the output, unless it is a parenthesis
        while (operatorStack.Count != 0) {

            // if there is still a parenthesis in the stack
            if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")") {
                // then there are mismatched parenthesis
                throw new Exception($"Mismatched parenthesis at location {operatorStack.Peek().Location}");
            }

            // pop an operator from the operator stack and add it to the output
            output.Add(operatorStack.Pop());
        }

        // return the output list
        return output;
    }
}