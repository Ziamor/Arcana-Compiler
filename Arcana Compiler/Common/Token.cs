namespace Arcana_Compiler.Common {
    public enum TokenType {
        EOF, // End of File
        NUMBER, // A numeric value
        IDENTIFIER, // Variable names
        DOT, // .
        COMMA, // ,
        QUESTION_MARK, // ?
        COLON, // :

        /*Operators*/
        PLUS, // '+'
        MINUS, // '-'
        MULTIPLY, // '*'
        DIVIDE, // '/'
        ASSIGN, // '='
        EQUALS, // ==
        LESS_THEN, // <
        LESS_THEN_OR_EQUAL, // <=
        GREATER_THAN, // >
        GREATER_THAN_OR_EQUAL, // >=
        NULL_COALESCING, // ??

        STRING, // "String data"
        OPEN_PARENTHESIS, // (
        CLOSE_PARENTHESIS, // )
        OPEN_BRACE, // {
        CLOSE_BRACE, // }
        COMMENT,

        /*Keywords*/
        IMPORT,
        NAMESPACE,
        CLASS,
        EXTENDS,
        FUNC,
        PUBLIC,
        PRIVATE,
        NULL,
    }

    public struct Token {
        public readonly static Dictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>{
            { "import", TokenType.IMPORT },
            { "namespace", TokenType.NAMESPACE },
            { "class", TokenType.CLASS },
            { "extends", TokenType.EXTENDS },
            { "func", TokenType.FUNC },
            { "public", TokenType.PUBLIC },
            { "private", TokenType.PRIVATE },
            { "null", TokenType.NULL },
        };
        public TokenType Type { get; }
        public string Value { get; }
        public int LineNumber { get; }
        public int Position { get; } // Column number

        public Token(TokenType type, string? value, int lineNumber, int position) {
            Type = type;
            Value = value ?? "null";
            LineNumber = lineNumber;
            Position = position;
        }

        public override string ToString() {
            return $"Token(Type: {Type}, Value: {Value}, Line: {LineNumber}, Pos: {Position})";
        }
    }
}
