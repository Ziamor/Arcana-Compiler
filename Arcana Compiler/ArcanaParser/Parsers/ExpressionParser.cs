using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class ExpressionParser : BaseParser<ExpressionNode> {
        public ExpressionParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override ExpressionNode Parse() {
            if (IsLiteral(CurrentToken)) {
                return parserFactory.CreateParser<LiteralNode>().Parse();
            } else if (IsUnaryOperator(CurrentToken) || IsUnaryOperator(PeekNextToken())) {
                return parserFactory.CreateParser<UnaryOperationNode>().Parse();
            } else if (IsBinaryOperator(PeekNextToken())) {
                return parserFactory.CreateParser<BinaryOperationNode>().Parse();
            } else if (CurrentToken.Type == TokenType.IDENTIFIER) {
                return ParseIdentifierOrMethodCall();
            } else {
                throw new UnexpectedTokenException(CurrentToken);
            }
        }

        private ExpressionNode ParseIdentifierOrMethodCall() {
            int lookaheadDistance = 1;
            bool methodCallDetected = false;

            while (true) {
                var nextToken = PeekNextToken(lookaheadDistance++);
                if (nextToken.Type == TokenType.DOT) {
                    lookaheadDistance++; // Skip the expected identifier.
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
                return parserFactory.CreateParser<VariableAccessNode>().Parse();
            }
        }
    }

    public class UnaryOperationParser : BaseParser<UnaryOperationNode> {
        public UnaryOperationParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override UnaryOperationNode Parse() {
            Token operatorToken = CurrentToken;
            UnaryOperatorPosition position;
            ExpressionNode operand;

            // Determine if it's a prefix unary operation
            if (IsUnaryOperator(operatorToken)) {
                position = UnaryOperatorPosition.Prefix;
                Eat(operatorToken.Type);
                operand = parserFactory.CreateParser<ExpressionNode>().Parse();
            } else {
                throw new ParsingException("Invalid unary operator position.");
            }

            if (operand is UnaryOperationNode unaryOperand && position == UnaryOperatorPosition.Prefix) {
                throw new ParsingException($"Chaining unary operators is not allowed at line {operatorToken.LineNumber}, position {operatorToken.Position}.");
            }

            return new UnaryOperationNode(operatorToken, operand, position);
        }
    }


    public class BinaryOperationParser : BaseParser<BinaryOperationNode> {
        public BinaryOperationParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override BinaryOperationNode Parse() {
            ExpressionNode left = parserFactory.CreateParser<ExpressionNode>().Parse();

            BinaryOperationNode? result = null;

            while (IsBinaryOperator(CurrentToken) && GetPrecedence(CurrentToken.Type) > 0) {
                Token operatorToken = CurrentToken;
                int operatorPrecedence = GetPrecedence(operatorToken.Type);
                Eat(operatorToken.Type);

                ExpressionNode right = ParseRightHandSide(operatorPrecedence);

                result = new BinaryOperationNode(left, operatorToken, right);

                left = result;
            }

            if (result == null) {
                throw new InvalidOperationException("Expected a binary operation but found none. This may indicate a logic error in parsing.");
            }

            return result;
        }

        private ExpressionNode ParseRightHandSide(int leftOperatorPrecedence) {
            ExpressionNode node = parserFactory.CreateParser<ExpressionNode>().Parse();

            while (IsBinaryOperator(CurrentToken) && GetPrecedence(CurrentToken.Type) > leftOperatorPrecedence) {
                Token operatorToken = CurrentToken;
                Eat(operatorToken.Type); // Consume the operator
                ExpressionNode rightNode = parserFactory.CreateParser<ExpressionNode>().Parse();
                node = new BinaryOperationNode(node, operatorToken, rightNode);
            }

            return node;
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
