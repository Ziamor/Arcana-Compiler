using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
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
            } else {
                // Parse other primary expressions (literals, variables, etc.)
                expression = parserFactory.CreateParser<PrimaryExpressionNode>().Parse();
            }
            CurrentToken = Lexer.GetCurrentToken();
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
            } else if (IsLiteral(CurrentToken)) {
                expression = parserFactory.CreateParser<LiteralNode>().Parse();
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
                return parserFactory.CreateParser<MethodCallNode>().Parse();
            } else {
                IdentifierName identifierName = ParseIdentifierName();
                return new VariableAccessNode(identifierName);
            }
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
