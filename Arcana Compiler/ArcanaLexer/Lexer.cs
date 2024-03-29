﻿using System.Text;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaLexer {
    public class Lexer : ILexer {
        private string _input = "";
        private string[] _lines = [];
        private int _position;
        private char _currentChar;
        private int _currentLine;
        private int _currentLinePosition;
        private int _currentTokenLength;
        private Token? _currentToken = null;
        private readonly Queue<Token> _tokenCache = new Queue<Token>();

        public void Initialize(string input) {
            _input = input.Replace("\r\n", "\n");
            _lines = _input.Split('\n');
            _position = 0;
            _currentLine = 1;
            _currentLinePosition = 1;
            _currentTokenLength = 0;
            _currentChar = _input.Length > 0 ? _input[_position] : '\0';
            _tokenCache.Clear();

            _currentToken = null;
        }

        private void Advance() {
            _currentTokenLength++;

            if (_currentChar == '\n') {
                _currentLine++;
                _currentLinePosition = 0;
                _currentTokenLength = 0; // Reset token length at new line
            }

            _position++;
            _currentLinePosition++;

            if (_position >= _input.Length) {
                _currentChar = '\0'; // Indicates end of file
            } else {
                _currentChar = _input[_position];
            }
        }

        public Token PeekToken(int depth = 1) {
            // Ensure there are enough tokens in the cache
            while (_tokenCache.Count < depth && _currentChar != '\0') {
                _tokenCache.Enqueue(ParseNextToken());
            }

            if (_tokenCache.Count < depth) {
                return CreateToken(TokenType.EOF, null); // Not enough tokens available (EOF reached)
            }

            return _tokenCache.ElementAt(depth - 1); // Return the peeked token without removing it
        }

        private Token ParseNextToken() {
            _currentTokenLength = 0; // Reset token length for new token

            while (_currentChar != '\0') {
                if (char.IsWhiteSpace(_currentChar)) {
                    SkipWhitespace();
                    continue;
                }

                if (char.IsDigit(_currentChar))
                    return GetNumberToken();

                if (char.IsLetter(_currentChar))
                    return GetIdentifierOrKeywordToken();

                return GetComplexToken(); // Handles symbols and punctuation
            }

            return CreateToken(TokenType.EOF, null);
        }


        public Token GetNextToken() {
            if (_tokenCache.Count > 0) {
                _currentToken = _tokenCache.Dequeue();
            } else {
                _currentToken = ParseNextToken();
            }
            return _currentToken ?? throw new InvalidOperationException("No current token exists. Call GetNextToken first.");
        }

        public Token GetCurrentToken() {
            return _currentToken ?? GetNextToken();
        }

        private Token GetComplexToken() {
            switch (_currentChar) {
                case '+':
                    Advance();
                    if (_currentChar == '+') {
                        Advance();
                        return CreateToken(TokenType.INCREMENT, "++");
                    }
                    return CreateToken(TokenType.PLUS, "+");
                case '-':
                    Advance();
                    if (_currentChar == '-') {
                        Advance();
                        return CreateToken(TokenType.DECREMENT, "--");
                    }
                    return CreateToken(TokenType.MINUS, "-");
                case '*':
                    Advance();
                    return CreateToken(TokenType.MULTIPLY, "*");
                case '/':
                    Advance();
                    if (_currentChar == '/') {
                        Advance();
                        return CreateToken(TokenType.COMMENT, GetSingleLineComment());
                    } else if (_currentChar == '*') {
                        Advance();
                        return CreateToken(TokenType.COMMENT, GetMultiLineComment());
                    }
                    return CreateToken(TokenType.DIVIDE, "/");
                case '"':
                    return CreateToken(TokenType.STRING, GetString());
                case '<':
                    Advance();
                    if (_currentChar == '=') {
                        Advance();
                        return CreateToken(TokenType.LESS_THAN_OR_EQUAL, "<=");
                    }
                    return CreateToken(TokenType.LESS_THAN, "<");
                case '>':
                    Advance();
                    if (_currentChar == '=') {
                        Advance();
                        return CreateToken(TokenType.GREATER_THAN_OR_EQUAL, ">=");
                    }
                    return CreateToken(TokenType.GREATER_THAN, ">");
                case '?':
                    Advance();
                    if (_currentChar == '?') {
                        Advance();
                        return CreateToken(TokenType.NULL_COALESCING, "??");
                    }
                    return CreateToken(TokenType.QUESTION_MARK, "?");
                case '.':
                    Advance();
                    return CreateToken(TokenType.DOT, ".");
                case ',':
                    Advance();
                    return CreateToken(TokenType.COMMA, ",");
                case ':':
                    Advance();
                    if (_currentChar == ':') {
                        Advance();
                        return CreateToken(TokenType.TYPE_CASTING, "::");
                    }
                    return CreateToken(TokenType.COLON, ":");
                case ';':
                    Advance();
                    return CreateToken(TokenType.SEMICOLON, ";");
                case '{':
                    Advance();
                    return CreateToken(TokenType.OPEN_BRACE, "{");
                case '}':
                    Advance();
                    return CreateToken(TokenType.CLOSE_BRACE, "}");
                case '(':
                    Advance();
                    return CreateToken(TokenType.OPEN_PARENTHESIS, "(");
                case ')':
                    Advance();
                    return CreateToken(TokenType.CLOSE_PARENTHESIS, ")");
                case '[':
                    Advance();
                    return CreateToken(TokenType.OPEN_BRACKET, "[");
                case ']':
                    Advance();
                    return CreateToken(TokenType.CLOSE_BRACKET, "]");
                case '!':
                    Advance();
                    if (_currentChar == '=') {
                        Advance();
                        return CreateToken(TokenType.NOT_EQUALS, "!=");
                    }
                    return CreateToken(TokenType.NOT, "!");
                case '=':
                    Advance();
                    if (_currentChar == '=') {
                        Advance();
                        return CreateToken(TokenType.EQUALS, "==");
                    }
                    return CreateToken(TokenType.ASSIGN, "=");
                default:
                    var errorToken = CreateToken(TokenType.ERROR, _currentChar.ToString());
                    Advance();
                    return errorToken;
            }
        }

        private Token CreateToken(TokenType type, string? value) {
            int tokenStartPos = _currentLinePosition - _currentTokenLength;
            string lineText = _lines.Length > _currentLine - 1 ? _lines[_currentLine - 1] : string.Empty;
            return new Token(type, value, _currentLine, tokenStartPos, lineText);
        }

        /// <summary>
        /// Tokenize everything, useful for testing the lexer
        /// </summary>
        /// <returns></returns>
        public List<Token> Tokenize() {
            List<Token> tokens = new List<Token>();
            Token current = GetNextToken();

            while (current.Type != TokenType.EOF) {
                tokens.Add(current);
                current = GetNextToken();
            }

            tokens.Add(current); // Make sure to include the EOF token

            return tokens;
        }

        private void SkipWhitespace() {
            while (char.IsWhiteSpace(_currentChar))
                Advance();
        }

        private Token GetNumberToken() {
            var result = "";
            bool hasDecimalPoint = false;

            while (char.IsDigit(_currentChar) || !hasDecimalPoint && _currentChar == '.') {
                if (_currentChar == '.') {
                    hasDecimalPoint = true;
                }
                result += _currentChar;
                Advance();
            }
            return CreateToken(TokenType.NUMBER, result);
        }
        private Token GetIdentifierOrKeywordToken() {
            StringBuilder resultBuilder = new StringBuilder();
            while (char.IsLetterOrDigit(_currentChar)) {
                resultBuilder.Append(_currentChar);
                Advance();
            }

            string result = resultBuilder.ToString();

            if (Token.KeywordMap.ContainsKey(result)) {
                return CreateToken(Token.KeywordMap[result], result);
            }

            return CreateToken(TokenType.IDENTIFIER, result);
        }

        private string GetString() {
            // The result is the string without the quotes
            var result = "";
            Advance(); // Skip the first quote
            while (_currentChar != '\0' && _currentChar != '\n') {
                result += _currentChar;
                Advance();

                if (_currentChar == '"') {
                    Advance(); // Consume the closing quote
                    break;
                }
            }
            return result;
        }

        private string GetSingleLineComment() {
            var result = "//";
            Advance(); // skip the second '/'
            while (_currentChar != '\0' && _currentChar != '\n') {
                result += _currentChar;
                Advance();
            }
            return result;
        }

        private string GetMultiLineComment() {
            var result = "/*";
            Advance(); // already skipped '*' before
            while (_currentChar != '\0') {
                if (_currentChar == '*' && Peek() == '/') {
                    result += "*/";
                    Advance(); // skip '*'
                    Advance(); // skip '/'
                    break;
                }
                result += _currentChar;
                Advance();
            }
            return result;
        }
        private char Peek() {
            var peekPos = _position + 1;
            if (peekPos >= _input.Length) {
                return '\0';
            }
            return _input[peekPos];
        }

    }
}
