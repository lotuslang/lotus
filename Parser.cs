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

        if (currToken == "svar") {
            tokenizer.Reconsume();

            current = ConsumeDeclaration();

            return current;
        }

        return null;
    }

    protected DeclarationNode ConsumeDeclaration() {

        // consume a token (which should be the keyword "svar")
        var svarKeyword = tokenizer.Consume();

        // if the token isn't the keyword "svar", throw an exception
        if (svarKeyword != "svar") throw new Exception($"{svarKeyword.Location} : Unexpected {svarKeyword.Kind} ({svarKeyword.Representation}) in declaration. A declaration must start with the keyword 'svar'");

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

    protected ValueNode ConsumeValue() {

        // converts to postfix notation
        var tokens = ToPostfixNotation(tokenizer);

        // postfix notation cannot contain an even number of tokens (since )
        if (tokens.Count == 0) throw new Exception($"{tokenizer.Current.Location}) : Unknow error, could not consume a value.");

        // if only one value could be parsed
        if (tokens.Count == 1) {
            var token = tokens[0];

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
        }
    }

    /// <summary>
    /// This is an implementation of the shunting-yard algorithm by Edsger Dijkstra.
    /// It takes in a tokenizer from which it will consume tokens, and outputs a list of Token,
    /// representing the input operation in Postfix Notation (also called Reverse Polish Notation).
    /// It can process numbers, identifiers, functions, and operators.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to consume the tokens from.</param>
    /// <returns>A list of tokens representing the input operation in Postfix Notation (a.k.a. Reverse Polish Notation).</returns>
    protected static List<Token> ToPostfixNotation(Tokenizer tokenizer) {

        // the output queue
        var output = new List<Token>();

        // the operator stack
        var operatorStack = new Stack<OperatorToken>();

        // the current token
        Token currToken;

        // variable to check if there are tokens left
        bool areTokensLeft = true;

        while (areTokensLeft) {
            // consume a token
            currToken = tokenizer.Consume(out areTokensLeft);

            // this check if a token is a function, since it is not done during tokenizing.
            // if the token is an identifier
            if (currToken == TokenKind.ident) {

                // consume a token and check if it is a left parenthesis ('(')
                if (tokenizer.Consume(out areTokensLeft) == "(") {

                    // if yes, then change its kind to TokenKind.function
                    currToken = new ComplexToken(currToken, TokenKind.function, currToken.Location);
                }

                // reconsume the token we just consumed
                tokenizer.Reconsume();
            }

            // if the token is a number, a string, or an identifier
            if (currToken is NumberToken
            || currToken == TokenKind.@string
            ||  currToken == TokenKind.ident) {

                // add it to the output list
                output.Add(currToken);
                continue;
            }

            // if the token is a function
            if (currToken == TokenKind.function) {

                // push it to the operator stack
                operatorStack.Push(new OperatorToken(currToken, 5, false, currToken.Location));
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

                continue;
            }

            // if the token is an operator token
            if (currToken is OperatorToken) {

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
                * while ((the operator at the top of the operator stack is a function)
                *        (the operator at the top of the operator stack has greater precedence)
                *        or (the operator at the top of the operator stack has equal precedence and is left associative))
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
                            // the operator is not a function
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

            if (currToken == "," || currToken == ".") {
                output.Add(currToken);
                continue;
            }

            // since there can be other tokens than the operation's ones,
            // immediatly reconsume and exit if the current token didn't
            // qualify for any condition above
            tokenizer.Reconsume();
            break;
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