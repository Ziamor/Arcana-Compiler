using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.ArcanaParser {
    public abstract class BaseParser<T> : IParser<T> where T : ASTNode {
        protected ILexer Lexer;
        protected Token CurrentToken;
        public readonly ErrorReporter ErrorReporter;

        protected BaseParser(ILexer lexer, ErrorReporter errorReporter) {
            Lexer = lexer;
            ErrorReporter = errorReporter;
            CurrentToken = Lexer.GetCurrentToken();
        }

        protected void Eat(TokenType tokenType) {
            if (CurrentToken.Type == tokenType) {
                CurrentToken = Lexer.GetNextToken();
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

        protected void ReportError(string message, int lineNumber, int position, ErrorSeverity severity) {
            ErrorReporter.ReportError(new ParseError(message, lineNumber, position, severity));
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

        public abstract T Parse();
    }
}