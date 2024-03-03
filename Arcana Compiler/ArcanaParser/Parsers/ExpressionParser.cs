using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System.Data.Common;
using System.Linq.Expressions;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class ExpressionParser : BaseParser<ExpressionNode> {
        public ExpressionParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override ExpressionNode Parse() {
            return ParseExpression();
        }

        private ExpressionNode ParseExpression() {
            return ParseBinaryOperation();
        }

        private ExpressionNode ParseBinaryOperation(int parentPrecedence = 0) {
            ExpressionNode leftHandSide = ParseUnaryOperation();

            while (true) {
                int precedence = GetPrecedence(CurrentToken.Type);
                if (precedence <= parentPrecedence) {
                    return leftHandSide;
                }

                Token operatorToken = CurrentToken;
                Eat(operatorToken.Type);
                ExpressionNode rightHandSide = ParseBinaryOperation(precedence);
                leftHandSide = new BinaryOperationNode(leftHandSide, operatorToken, rightHandSide);
            }
        }

        private ExpressionNode ParseUnaryOperation() {
            if (IsUnaryOperator(CurrentToken)) {
                Token operatorToken = CurrentToken;
                Eat(operatorToken.Type);
                ExpressionNode operand = ParseUnaryOperation(); // Unary operations have the highest precedence
                return new UnaryOperationNode(operatorToken, operand, UnaryOperatorPosition.Prefix);
            }

            // Handle primary expressions, including parentheses
            return ParsePrimaryExpression();
        }

        private ExpressionNode ParsePrimaryExpression() {
            ExpressionNode expression;
            if (CurrentToken.Type == TokenType.OPEN_PARENTHESIS) {
                Eat(TokenType.OPEN_PARENTHESIS);
                expression = ParseExpression();
                Eat(TokenType.CLOSE_PARENTHESIS);
            } else if (CurrentToken.Type == TokenType.NEW) {
                // Handle array initialization with size
                Eat(TokenType.NEW);
                var typeIdentifier = ParseIdentifierName();
                Eat(TokenType.OPEN_BRACKET);
                ExpressionNode size = ParseExpression();
                Eat(TokenType.CLOSE_BRACKET);
                expression = new ArrayInitializationNode(size);
            } else if (CurrentToken.Type == TokenType.OPEN_BRACKET) {
                Eat(TokenType.OPEN_BRACKET);
                List<ExpressionNode> values = new List<ExpressionNode>();
                if (CurrentToken.Type != TokenType.CLOSE_BRACKET) {
                    values.Add(ParseExpression());
                    while (CurrentToken.Type == TokenType.COMMA) {
                        Eat(TokenType.COMMA);
                        values.Add(ParseExpression());
                    }
                }
                Eat(TokenType.CLOSE_BRACKET);
                expression = new ArrayInitializationNode(values);
            } else {
                // Parse other primary expressions (literals, variables, etc.)
                expression = ParseNode<PrimaryExpressionNode>();
            }

            while (CurrentToken.Type == TokenType.OPEN_BRACKET) {
                Eat(TokenType.OPEN_BRACKET);
                ExpressionNode index = ParseExpression();
                Eat(TokenType.CLOSE_BRACKET);
                expression = new ArrayAccessNode(expression, index);
            }

            return ParsePostfixUnaryOperation(expression);
        }

        private ExpressionNode ParsePostfixUnaryOperation(ExpressionNode operand) {
            while (IsPostfixUnaryOperator(CurrentToken)) {
                Token operatorToken = CurrentToken;
                Eat(operatorToken.Type);
                operand = new UnaryOperationNode(operatorToken, operand, UnaryOperatorPosition.Postfix);
            }
            return operand;
        }
    }
    public class PrimaryExpressionParser : BaseParser<PrimaryExpressionNode> {
        public PrimaryExpressionParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory) : base(lexer, errorReporter, parserFactory) {
        }

        public override PrimaryExpressionNode Parse() {
            PrimaryExpressionNode expression;
            if (CurrentToken.Type == TokenType.IDENTIFIER) {
                expression = ParseIdentifierOrMethodCall();
            } else if (CurrentToken.Type == TokenType.THIS) {
                expression = ParseThisExpression();
            } else if (IsLiteral(CurrentToken)) {
                expression = ParseNode<LiteralNode>();
            } else if (CurrentToken.Type == TokenType.NEW) {
                Eat(TokenType.NEW);
                IdentifierName className = ParseIdentifierName();
                List<ASTNode> constructorArguments = new List<ASTNode>();
                if (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                    constructorArguments = ParseArguments();
                }
                return new ObjectInstantiationNode(className, constructorArguments);
            } else {
                throw new UnexpectedTokenException(CurrentToken);
            }

            return expression;
        }

        private PrimaryExpressionNode ParseIdentifierOrMethodCall() {
            int lookaheadDistance = 1;
            bool methodCallDetected = false;

            while (true) {
                var nextToken = PeekNextToken(lookaheadDistance++);
                if (nextToken.Type == TokenType.DOT) {
                    lookaheadDistance++;
                } else if (nextToken.Type == TokenType.OPEN_PARENTHESIS) {
                    methodCallDetected = true;
                    break;
                } else {
                    break;
                }
            }

            if (methodCallDetected) {
                return ParseNode<MethodCallNode>();
            } else {
                IdentifierName identifierName = ParseIdentifierName();
                return new VariableAccessNode(identifierName);
            }
        }

        private PrimaryExpressionNode ParseThisExpression() {
            Eat(TokenType.THIS);
            Eat(TokenType.DOT);


            ExpressionNode expression =  ParseNode<ExpressionNode>();

            ThisExpressionNode thisNode = new ThisExpressionNode(expression);

            return thisNode;
        }

        private List<ASTNode> ParseArguments() {
            // Initialize the list to hold argument AST nodes
            List<ASTNode> arguments = new List<ASTNode>();

            // Expect an opening parenthesis
            Eat(TokenType.OPEN_PARENTHESIS);

            // Loop until a closing parenthesis is encountered
            while (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                ExpressionNode expression = ParseNode<ExpressionNode>();
                arguments.Add(expression);
                if (CurrentToken.Type != TokenType.COMMA) {
                    break;
                }
                Eat(TokenType.COMMA);
            }

            Eat(TokenType.CLOSE_PARENTHESIS);

            return arguments;
        }

    }

    public class MethodCallParser : BaseParser<MethodCallNode> {
        public MethodCallParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override MethodCallNode Parse() {
            IdentifierName identifierName = ParseIdentifierName();

            Eat(TokenType.OPEN_PARENTHESIS);
            List<ASTNode> arguments = new List<ASTNode>();

            if (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                arguments.Add(ParseExpression());
                while (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    arguments.Add(ParseExpression());
                }
            }
            if (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                throw new SyntaxErrorException(TokenType.CLOSE_PARENTHESIS, CurrentToken);
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return new MethodCallNode(identifierName, arguments);
        }

        private ExpressionNode ParseExpression() {
            ExpressionNode expressionNode = ParseNode<ExpressionNode>();
            return expressionNode;
        }
    }

    public class LiteralParser : BaseParser<LiteralNode> {
        public LiteralParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override LiteralNode Parse() {
            switch (CurrentToken.Type) {
                case TokenType.NUMBER:
                    return ParseNumberLiteral();
                case TokenType.STRING:
                    return ParseStringLiteral();
                /*case TokenType.TRUE:
                case TokenType.FALSE:
                    return ParseBooleanLiteral();*/
                default:
                    throw new UnexpectedTokenException(CurrentToken);
            }
        }

        private LiteralNode ParseNumberLiteral() {
            var token = CurrentToken;
            Eat(TokenType.NUMBER);
            if (token.Value.Contains(".")) {
                if (float.TryParse(token.Value, out float floatValue)) {
                    return new LiteralNode(floatValue);
                } else {
                    throw new ParsingException($"Invalid float literal '{token.Value}'.");
                }
            } else {
                if (int.TryParse(token.Value, out int intValue)) {
                    return new LiteralNode(intValue);
                } else {
                    throw new ParsingException($"Invalid integer literal '{token.Value}'.");
                }
            }
        }

        private LiteralNode ParseStringLiteral() {
            var token = CurrentToken;
            Eat(TokenType.STRING);
            string stringValue = token.Value.Trim('"');
            return new LiteralNode(stringValue);
        }

        private LiteralNode ParseBooleanLiteral() {
            throw new NotImplementedException();
            /*var token = CurrentToken;
            bool value = token.Type == TokenType.TRUE;
            Eat(token.Type); // Consumes either TRUE or FALSE
            return new LiteralNode(value);*/
        }
    }
}
