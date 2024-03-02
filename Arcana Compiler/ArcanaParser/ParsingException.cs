using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser {
    public class ParsingException : Exception {
        public ParsingException(string message) : base(message) { }
    }

    public class UnexpectedTokenException : ParsingException {
        public UnexpectedTokenException(Token token)
            : base($"Unexpected token '{token.Value}' of type {token.Type} at line {token.LineNumber}, position {token.Position}. Line: '{token.LineText.Trim()}'.") { }
    }

    public class SyntaxErrorException : ParsingException {
        public SyntaxErrorException(TokenType expected, Token found)
            : base($"Expected token of type {expected}, but found '{found.Value}' of type {found.Type} at line {found.LineNumber}, position {found.Position}. Line: '{found.LineText.Trim()}'.") { }
        public SyntaxErrorException(string expected, Token found)
           : base($"Expected {expected}, but found '{found.Value}' of type {found.Type} at line {found.LineNumber}, position {found.Position}. Line: '{found.LineText.Trim()}'.") { }
    }
}
