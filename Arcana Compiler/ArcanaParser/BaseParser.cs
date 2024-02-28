using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System.Xml.Linq;
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.ArcanaParser {
    public class DefaultParserContext<TNode> : IParserContext<TNode> where TNode : ASTNode {
    }

    public abstract class BaseParser<TNode> : IParser<TNode> where TNode : ASTNode {
        protected ILexer Lexer;
        protected Token CurrentToken;
        public readonly ErrorReporter errorReporter;
        protected readonly ParserFactory parserFactory;

        protected BaseParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory) {
            Lexer = lexer;
            this.errorReporter = errorReporter;
            this.parserFactory = parserFactory;

            CurrentToken = Lexer.GetCurrentToken();
            SkipComments();
        }

        protected void Eat(TokenType tokenType) {
            if (CurrentToken.Type == tokenType) {
                CurrentToken = Lexer.GetNextToken();
                SkipComments();
            } else {
                Error(string.Format("Expected token of type {0}, but found '{1}'", tokenType, CurrentToken.Value));
            }
        }

        protected void Error(string message) {
            throw new ParsingException(message);
        }

        protected Token PeekNextToken(int depth = 1) {
            return Lexer.PeekToken(depth);
        }

        protected void SkipComments() {
            while (CurrentToken.Type == TokenType.COMMENT) {
                Eat(TokenType.COMMENT);
            }
        }

        protected void ReportError(string message, int lineNumber, int position, ErrorSeverity severity) {
            errorReporter.ReportError(new ParseError(message, lineNumber, position, severity));
            if (severity == ErrorSeverity.Fatal) {
                throw new ParsingException(message);
            }
        }

        protected IdentifierName ParseIdentifierName() {
            List<string> parts = [CurrentToken.Value];
            Eat(TokenType.IDENTIFIER);

            while (CurrentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT);
                if (CurrentToken.Type == TokenType.IDENTIFIER || CurrentToken.Type == TokenType.MULTIPLY) {
                    parts.Add(CurrentToken.Value);
                    Eat(CurrentToken.Type);
                }
            }

            return new IdentifierName(parts);
        }

        protected bool IsLiteral(Token token) {
            switch (token.Type) {
                case TokenType.NUMBER:
                case TokenType.STRING:
                case TokenType.NULL:
                    return true;
                default:
                    return false;
            }
        }


        protected bool IsClassModifier(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.STATIC:
                case TokenType.ABSTRACT:
                case TokenType.FINAL:
                    return true;
                default:
                    return false;
            }
        }

        protected List<ClassModifierNode> ParseClassModifiers() {
            List<ClassModifierNode> modifiers = new List<ClassModifierNode>();
            while (IsClassModifier(CurrentToken.Type)) {
                modifiers.Add(new ClassModifierNode(CurrentToken.Value));
                Eat(CurrentToken.Type);
            }
            return modifiers;
        }

        protected bool IsFieldModifier(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.STATIC:
                case TokenType.CONST:
                    return true;
                default:
                    return false;
            }
        }

        protected bool IsAccessModifier(TokenType tokenType) {
            return tokenType == TokenType.PUBLIC || tokenType == TokenType.PRIVATE || tokenType == TokenType.PROTECTED;
        }


        /// <summary>
        /// Try to parse an access modifer, will return null if none is found.
        /// </summary>
        /// <returns>A string if an access modifer is found, null otherwise.</returns>
        protected string? TryParseAccessModifier() {
            if (IsAccessModifier(CurrentToken.Type)) {
                string modifier = CurrentToken.Value;
                Eat(CurrentToken.Type);
                return modifier;
            }
            return null; // No access modifier present
        }


        protected bool IsUnaryOperator(Token token) {
            return token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT ||
                   token.Type == TokenType.MINUS || token.Type == TokenType.NOT;
        }

        protected bool IsPostfixUnaryOperator(Token token) {
            return token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT;
        }

        protected bool IsBinaryOperator(Token token) {
            return token.Type == TokenType.PLUS || token.Type == TokenType.MINUS ||
                   token.Type == TokenType.MULTIPLY || token.Type == TokenType.DIVIDE ||
                   token.Type == TokenType.EQUALS || token.Type == TokenType.LESS_THAN ||
                   token.Type == TokenType.GREATER_THAN || token.Type == TokenType.LESS_THAN_OR_EQUAL ||
                   token.Type == TokenType.GREATER_THAN_OR_EQUAL;
        }

        protected int GetPrecedence(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    return 1;
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                    return 2;
                case TokenType.EQUALS:
                case TokenType.LESS_THAN:
                case TokenType.GREATER_THAN:
                case TokenType.LESS_THAN_OR_EQUAL:
                case TokenType.GREATER_THAN_OR_EQUAL:
                    return 3;
                default:
                    return 0;
            }
        }
        public abstract TNode Parse();
    }
}