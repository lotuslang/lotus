using System;
using System.Linq;
using System.Collections.Generic;

public class Parser : IConsumer<StatementNode>
{
    private bool reconsumeLastDocNode;

    private Tokenizer tokenizer;

    protected int position;

    /// <summary>
    /// Indicates the position of this instance (i.e. how many StatementNode objects have been consumed).
    /// </summary>
    /// <value>The position of this instance.</value>
    public int Position {
        get => position;
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
        this.tokenizer = tokenizer;
    }

    public Parser(StringConsumer consumer) {
        tokenizer = new Tokenizer(consumer);
    }

    public Parser(IEnumerable<char> collection) {
        tokenizer = new Tokenizer(collection);
    }

    /// <summary>
    /// Reconsumes the last StatementNode object.
    /// </summary>
    public void Reconsume() => reconsumeLastDocNode = true;

    /// <summary>
    /// Consumes a StatementNode object and returns it.
    /// </summary>
    /// <param name="success">True if the operation succeeded, false otherwise.</param>
    /// <returns>The StatementNode object consumed.</returns>
    public StatementNode Consume(out bool success) {
        var node = Consume(); // consume a StatementNode

        success = node != null; // then checks that it isn't null

        return node; // and finally return the StatementNode consumed
    }

    /// <summary>
    /// Consume a StatementNode object and returns it.
    /// </summary>
    /// <returns>The StatementNode object consumed.</returns>
    public StatementNode Consume() {

        // If we are asked to reconsume, just return the current StatementNode
        if (reconsumeLastDocNode) {
            reconsumeLastDocNode = false;
            return current;
        }

        // Update the position
        position++;

        // Consume a token
        var currToken = tokenizer.Consume();

        if (currToken == "var") {
            tokenizer.Reconsume();

            current = ConsumeDeclaration();

            return current;
        }

        if (currToken == ";") {
            return Consume();
        }

		if (currToken == TokenKind.ident) {

			// if it's an identifier followed by '(', it's a function call
			if (tokenizer.Peek() == "(") {
				return new FunctionNode(ConsumeValue(), currToken as ComplexToken);
			}

			if (tokenizer.Peek() == "=") {
				tokenizer.Consume();
				return new AssignementNode(ConsumeValue(), currToken as ComplexToken);
			}

			throw new Exception($"{currToken.Location} : Unexpected {currToken.Kind}. Did you forget '=' or '()' ?");
		}

        return null;
    }

    protected DeclarationNode ConsumeDeclaration() {

        // consume a token (which should be the keyword "var")
        var varKeyword = tokenizer.Consume();

        // if the token isn't the keyword "var", throw an exception
        if (varKeyword != "var") throw new Exception($"{varKeyword.Location} : Unexpected {varKeyword.Kind} ({varKeyword.Representation}) in declaration. A declaration must start with the keyword 'var'");

        // consume a token (which is the name of the variable we're declaring)
        var name = tokenizer.Consume();

        // if the token isn't an identifier, throw an exception
        if (name != TokenKind.ident) throw new Exception($"{name.Location} : Unexpected {name.Kind} ({name.Representation}) in declaration. Expected an identifier");

        // consume a token
        var equalSign = tokenizer.Consume();

        if (equalSign != "=") throw new Exception($"{equalSign.Location} : Unexpected {equalSign.Kind} ({equalSign.Representation}) in declaration. Expected '='");

        // consume a ValueNode (which is the value of the variable we're declaring)
        var value = ConsumeValue();

        return new DeclarationNode(value, name as ComplexToken);
    }

    public ValueNode ConsumeValue() {

        // converts to postfix notation
        var postfix = ToPostfixNotation(tokenizer);

        // postfix notation cannot contain an even number of tokens (since )
        if (postfix.Count == 0) throw new Exception($"{tokenizer.Current.Location} : Unknown error, could not consume a value.");

        // if only one value could be parsed
        if (postfix.Count == 1) {
            var token = postfix[0];

            // if the token is an identifieer, return an IdentNode
            if (token == TokenKind.ident) {
                return new IdentNode(token.Representation);
            }

            // if the token is a string, return a StringNode
            if (token == TokenKind.@string) {
                return new StringNode(token.Representation);
            }

            // if the token is a number, return a NumberNode
            if (token == TokenKind.number) {
                return new NumberNode((token as NumberToken).Value);
            }

            throw new Exception($"{token.Location} : Unexpected token {token.Representation} at {token.Location}. Expected string, number, identifier, or function call");
        }

        // otherwise, transform the postfix expression into a tree (we returnn the root of that tree)

        var operands = new Stack<ValueNode>();

        for (int i = 0; i < postfix.Count; i++)
        {
            var token = postfix[i];

            if (token is NumberToken) {
                operands.Push(new NumberNode((token as NumberToken).Value));
                continue;
            }

            if (token == TokenKind.ident) {
                operands.Push(new IdentNode(token));
                continue;
            }

            if (token == TokenKind.@string) {
                operands.Push(new StringNode(token));
                continue;
            }

            if (token == "[") {
                operands.Push(new ValueNode("["));
                continue;
            }

            if (token is OperatorToken) {

                OperationNode op;

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
						op = new OperationNode(token, new ValueNode[] { operands.Pop(), operands.Pop()}, "binaryAssign");
						break;
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
                    default: // if the operator is none of the above, then assume it is a function
                        var funcOperands = new List<ValueNode>();

						// while the next operand is not "[" (the delimiter for function calls)
                        while (operands.Peek().Representation != "[")
							// pop operands from the operand stack and add them to the function operands
                            funcOperands.Add(operands.Pop());

						// pop the "[" remaining
                        operands.Pop();

                        op = new OperationNode(token, funcOperands.ToArray(), "function");
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
    public static List<Token> ToPostfixNotation(Tokenizer tokenizer) {

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
            currToken = tokenizer.Consume(out areTokensLeft);

            // this this to check if an identifier is a function, since it is not done during tokenizing.

            // if the token is an identifier
            if (currToken == TokenKind.ident) {

                // peek the next token
                var nextToken = tokenizer.Peek();

                // if it is a left paren ('(')
                if (nextToken == "(") {

                    // add an identical token but identified as TokenKind.function to the output
                    currToken = new ComplexToken(currToken, TokenKind.function, currToken.Location);
                }
            }

            // if the token is a number, a string, or an identifier
            if (currToken is NumberToken
            || currToken == TokenKind.@string
            || currToken == TokenKind.ident) {

                // add it to the output list
                output.Add(currToken);
                continue;
            }

            // if the token is a function
            /* if (currToken == TokenKind.function) {

                // consume the '(' right after it
                tokenizer.Consume();

                // add a FunctionToken to the output with a RPN of the calling parenthesis as the arg list
                output.Add(new FunctionToken(currToken, ToPostfixNotation(tokenizer), currToken.Location));
                continue;
            }*/

            // if the token is a function
            if (currToken == TokenKind.function) {

                // push it to the operator stack
                operatorStack.Push(new OperatorToken(currToken, Precedence.FuncCall, false, currToken.Location));
                output.Add(new Token('[', TokenKind.delim, tokenizer.Peek().Location));
                continue;
            }

            // if the token is a left parenthesis ('(')
            if (currToken == "(") {

                // push it to the operator stack
                operatorStack.Push(new OperatorToken('(', 0, false, currToken.Location));
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

				// TODO: Implement type casting handling
				if (!(tokenizer.Peek() is OperatorToken)) {

				}

                continue;
            }

            if (currToken == ".") {
                currToken = new OperatorToken(currToken, Precedence.Access, true, currToken.Location);
            }

            if (currToken == "++" || currToken == "--") {
                if (lastToken == TokenKind.ident) {
                    currToken = new OperatorToken(currToken, Precedence.Unary, true, currToken.Location);
                }
                else if (tokenizer.Peek() == TokenKind.ident) {
                    currToken = new OperatorToken(currToken, Precedence.Unary, false, currToken.Location);
                }
                else throw new Exception($"Increment/Decrement operator with no associated identifier at location {currToken.Location}");
            }

            // if the token is an operator token
            if (currToken is OperatorToken) {

                // if the last token was an operator and the next token is an identifier, or the last token was a right parenthesis, a right parenthesis it means this operator is unary
                if ((lastToken is OperatorToken && tokenizer.Peek() == TokenKind.ident) || lastToken == "(" || tokenizer.Peek() == "(") {

                    // if the current token is "-"
                    if (currToken == "-") {
                        currToken = new OperatorToken("_", Precedence.Unary, false, currToken.Location);
                        //output.Add(new OperatorToken("_", 5, false, currToken.Location));
                        //continue;
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
                            // the operator has greater prcedence
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

            // pop an operator fromp the operator stack and add it to the output
            output.Add(operatorStack.Pop());
        }

        // return the output list
        return output;
    }
}