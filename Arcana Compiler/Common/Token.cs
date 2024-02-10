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
        NOT, // !
        PLUS, // +
        MINUS, // -
        MULTIPLY, // *
        DIVIDE, // /
        ASSIGN, // =
        EQUALS, // ==
        NOT_EQUALS, // !=
        LESS_THEN, // <
        LESS_THEN_OR_EQUAL, // <=
        GREATER_THAN, // >
        GREATER_THAN_OR_EQUAL, // >=
        NULL_COALESCING, // ??
        TYPE_CASTING, // ::

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
        NEW,
        FUNC,
        IF,
        ELSE,
        PUBLIC,
        PRIVATE,
        NULL,
        THIS,
    }

    public struct Token {
        public readonly static Dictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>{
            { "import", TokenType.IMPORT },
            { "namespace", TokenType.NAMESPACE },
            { "class", TokenType.CLASS },
            { "new", TokenType.NEW },
            { "func", TokenType.FUNC },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "public", TokenType.PUBLIC },
            { "private", TokenType.PRIVATE },
            { "null", TokenType.NULL },
            { "this", TokenType.THIS },
        };
        public TokenType Type { get; }
        public string Value { get; }
        public int LineNumber { get; }
        public int Position { get; } // Column number

        public string LineText { get; }

        public Token(TokenType type, string? value, int lineNumber, int position, string lineText) {
            Type = type;
            Value = value ?? "null";
            LineNumber = lineNumber;
            Position = position;
            LineText = lineText;
        }

        public override string ToString() {
            return $"Token(Type: {Type}, Value: {Value}, Line: {LineNumber}, Pos: {Position})";
        }
    }
}
