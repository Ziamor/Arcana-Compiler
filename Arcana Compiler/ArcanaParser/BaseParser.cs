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

        public abstract T Parse();
    }
}