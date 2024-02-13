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
        LESS_THAN, // <
        LESS_THAN_OR_EQUAL, // <=
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
        INTERFACE,
        STRUCT,
        NEW,
        FUNC,
        IF,
        ELSE,
        PUBLIC,
        PRIVATE,
        NULL,
        THIS,
        RETURN,
        STATIC,
        ABSTRACT,
        VIRTUAL,
        FINAL,
        CONST,
        OVERRIDE,
    }

    public struct Token {
        public readonly static Dictionary<string, TokenType> KeywordMap = new Dictionary<string, TokenType>{
            { "import", TokenType.IMPORT },
            { "namespace", TokenType.NAMESPACE },
            { "class", TokenType.CLASS },
            { "interface", TokenType.INTERFACE },
            { "struct", TokenType.STRUCT },
            { "new", TokenType.NEW },
            { "func", TokenType.FUNC },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "public", TokenType.PUBLIC },
            { "private", TokenType.PRIVATE },
            { "null", TokenType.NULL },
            { "this", TokenType.THIS },
            { "return", TokenType.RETURN },
            { "static", TokenType.STATIC },
            { "abstract", TokenType.ABSTRACT },
            { "virtual", TokenType.VIRTUAL },
            { "final", TokenType.FINAL },
            { "const", TokenType.CONST },
            { "override", TokenType.OVERRIDE },
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
