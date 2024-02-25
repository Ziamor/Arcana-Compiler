using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaParser.Parsers.Contexts;
using Arcana_Compiler.Common;
using System.Linq.Expressions;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class ExpressionParser : BaseParser<ExpressionNode> {
        public ExpressionParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override ExpressionNode Parse() {
            /*ExpressionNode left;

            if (IsUnaryOperator(CurrentToken) || (IsUnaryOperator(PeekNextToken()) && IsPostfixUnaryOperator(PeekNextToken()))) {
                left = parserFactory.CreateParser<UnaryOperationNode>().Parse();
            } else if (CurrentToken.Type == TokenType.IDENTIFIER) {
                left = ParseIdentifierOrMethodCall();
            } else if (IsLiteral(CurrentToken)) {
                left = parserFactory.CreateParser<LiteralNode>().Parse();
            } else {
                throw new UnexpectedTokenException(CurrentToken);
            }
            CurrentToken = Lexer.GetCurrentToken();

            if (IsBinaryOperator(CurrentToken)) {
                int precedence = 0;
                BinaryOperationParserContext binaryOperationParserContext = new BinaryOperationParserContext(precedence, left);
                IParser<BinaryOperationNode> binaryOperationNodeParser = parserFactory.CreateParser<BinaryOperationNode>(binaryOperationParserContext);
                return binaryOperationNodeParser.Parse();
            } else {
                return left;
            }*/

            PrimaryExpressionNode expression = parserFactory.CreateParser<PrimaryExpressionNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();
            if (IsBinaryOperator(CurrentToken)) {
                BinaryOperationParserContext binaryOperationParserContext = new BinaryOperationParserContext(0, expression);
                IParser<BinaryOperationNode> binaryOperationNodeParser = parserFactory.CreateParser<BinaryOperationNode>(binaryOperationParserContext);
                return binaryOperationNodeParser.Parse();
            }

            IParser<PrimaryExpressionNode> primaryExpressionParser = parserFactory.CreateParser<PrimaryExpressionNode>();
            return primaryExpressionParser.Parse();
        }
    }
    public class PrimaryExpressionParser : BaseParser<PrimaryExpressionNode> {
        public PrimaryExpressionParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory) : base(lexer, errorReporter, parserFactory) {
        }

        public override PrimaryExpressionNode Parse() {
            PrimaryExpressionNode expression;
            if (IsUnaryOperator(CurrentToken) || (IsUnaryOperator(PeekNextToken()) && IsPostfixUnaryOperator(PeekNextToken()))) {
                //expression = parserFactory.CreateParser<UnaryOperationNode>().Parse();
                expression = null;
            } else if (CurrentToken.Type == TokenType.IDENTIFIER) {
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

    public class UnaryOperationParser : BaseParser<UnaryOperationNode> {
        public UnaryOperationParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override UnaryOperationNode Parse() {
            Token operatorToken;
            UnaryOperatorPosition position;
            ExpressionNode operand;

            if (IsUnaryOperator(CurrentToken)) {
                operatorToken = CurrentToken;
                position = UnaryOperatorPosition.Prefix;
                Eat(CurrentToken.Type);
                operand = parserFactory.CreateParser<ExpressionNode>().Parse();
            } else {
                IdentifierName identifierName = ParseIdentifierName();
                operand = new VariableAccessNode(identifierName);

                if (IsPostfixUnaryOperator(CurrentToken)) {
                    operatorToken = CurrentToken;
                    position = UnaryOperatorPosition.Postfix;
                    Eat(operatorToken.Type);
                } else {
                    throw new ParsingException("Expected a unary operator.");
                }
            }

            if (operand == null) {
                throw new InvalidOperationException("Unary operation missing operand.");
            }

            return new UnaryOperationNode(operatorToken, operand, position);
        }
    }


    public class BinaryOperationParser : BaseParserWithContext<BinaryOperationNode, BinaryOperationParserContext> {
        public BinaryOperationParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory, BinaryOperationParserContext context)
            : base(lexer, errorReporter, parserFactory, context) {
        }

        public override BinaryOperationNode Parse() {
            ExpressionNode left = context.Left;
            Token operatorToken = CurrentToken;
            Eat(operatorToken.Type);
            ExpressionNode right = parserFactory.CreateParser<PrimaryExpressionNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();

            if (IsBinaryOperator(CurrentToken)) {
                int currentOperatorPrecedence = GetPrecedence(operatorToken.Type);
                int nextOperatorPrecedence = GetPrecedence(CurrentToken.Type);
                if (currentOperatorPrecedence > nextOperatorPrecedence) {
                    BinaryOperationNode binaryOperationNode = new BinaryOperationNode(left, operatorToken, right);
                    BinaryOperationParserContext binaryOperationParserContext = new BinaryOperationParserContext(0, binaryOperationNode);
                    IParser<BinaryOperationNode> binaryOperationNodeParser = parserFactory.CreateParser<BinaryOperationNode>(binaryOperationParserContext);
                    return binaryOperationNodeParser.Parse();
                } else {
                    BinaryOperationParserContext binaryOperationParserContext = new BinaryOperationParserContext(0, right);
                    IParser<BinaryOperationNode> binaryOperationNodeParser = parserFactory.CreateParser<BinaryOperationNode>(binaryOperationParserContext);


                    return new BinaryOperationNode(left, operatorToken, binaryOperationNodeParser.Parse());
                }
            }
            return new BinaryOperationNode(left, operatorToken, right);
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
