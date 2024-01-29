using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.ArcanaParser {
    public class Parser {
        private readonly ILexer _lexer;
        private Token _currentToken;
        private Token? _peekedToken = null;

        public Parser(ILexer lexer) {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken(); // Initialize with the first token
            SkipComments(); // Skip any comments
        }

        private void Eat(TokenType tokenType) {
            if (_currentToken.Type == tokenType) {
                if (_peekedToken != null) {
                    _currentToken = _peekedToken.Value;
                    _peekedToken = null;
                } else {
                    _currentToken = _lexer.GetNextToken();
                }
                SkipComments();
            } else {
                throw new Exception($"Syntax error: Expected {tokenType}, got {_currentToken.Type} at Line: {_currentToken.LineNumber} Pos: {_currentToken.Position}");
            }
        }
        private Token PeekNextToken() {
            if (_peekedToken == null) {
                _peekedToken = _lexer.GetNextToken();
            }
            return _peekedToken.Value;
        }

        private void SkipComments() {
            while (_currentToken.Type == TokenType.COMMENT) {
                Eat(TokenType.COMMENT);
            }
        }

        public ASTNode Parse() {
            ProgramNode rootNode = new ProgramNode();

            // Parse imports first
            rootNode.Imports = ParseImportDeclarations();

            while (_currentToken.Type != TokenType.EOF) {
                switch (_currentToken.Type) {
                    case TokenType.NAMESPACE:
                        rootNode.Declarations.Add(ParseNamespaceDeclaration());
                        break;
                    case TokenType.CLASS:
                        rootNode.Declarations.Add(ParseClassDeclaration());
                        break;
                    default:
                        throw new Exception($"Unexpected token: {_currentToken.Type}");
                }
            }

            return rootNode;
        }

        private NamespaceDeclarationNode ParseNamespaceDeclaration() {
            Eat(TokenType.NAMESPACE);
            QualifiedName namespaceName = ParseQualifiedName();

            Eat(TokenType.OPEN_BRACE);

            List<ASTNode> classDeclarations = new List<ASTNode>();
            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                if (_currentToken.Type == TokenType.CLASS) {
                    classDeclarations.Add(ParseClassDeclaration());
                } else {
                    // Handle error or unexpected token
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            return new NamespaceDeclarationNode(namespaceName, classDeclarations);
        }


        private ClassDeclarationNode ParseClassDeclaration() {
            Eat(TokenType.CLASS);
            string className = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParentTypeNode> parentTypes = ParseParentTypes();

            Eat(TokenType.OPEN_BRACE);

            List<FieldDeclarationNode> fields = new List<FieldDeclarationNode>();
            List<MethodDeclarationNode> methods = new List<MethodDeclarationNode>();

            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                string? accessModifier = ParseAccessModifier();

                if (IsMethodDeclaration()) {
                    methods.Add(ParseMethodDeclaration(accessModifier));
                } else {
                    fields.Add(ParseFieldDeclaration(accessModifier));
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            return new ClassDeclarationNode(className, parentTypes, fields, methods);
        }

        private bool IsMethodDeclaration() {
            // Peek at the next token without consuming it.
            var nextToken = PeekNextToken();

            // Check if the current token indicates a method.
            // Assuming 'func' keyword is used to indicate a method.
            if (_currentToken.Type == TokenType.FUNC ||
                (_currentToken.Type == TokenType.IDENTIFIER && nextToken.Type == TokenType.FUNC)) {
                return true;
            }

            // Otherwise, it's likely a field (or a syntax error, which will be caught later).
            return false;
        }

        private string? ParseAccessModifier() {
            if (_currentToken.Type == TokenType.PUBLIC || _currentToken.Type == TokenType.PRIVATE) {
                string modifier = _currentToken.Value;
                Eat(_currentToken.Type);
                return modifier;
            }
            return null; // No access modifier present
        }

        private List<ParentTypeNode> ParseParentTypes() {
            List<ParentTypeNode> parentTypes = new List<ParentTypeNode>();
            if (_currentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                parentTypes.Add(ParseParentType());
                while (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    parentTypes.Add(ParseParentType());
                }
            }

            return parentTypes;
        }
        private ParentTypeNode ParseParentType() {
            string parentTypeName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);
            return new ParentTypeNode(parentTypeName);
        }

        private FieldDeclarationNode ParseFieldDeclaration(string? accessModifier) {
            TypeNode fieldType = ParseType();

            string fieldName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            ASTNode? initialValue = null;
            if (_currentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                initialValue = ParseExpression();
            }

            return new FieldDeclarationNode(fieldType, fieldName, initialValue);
        }

        private MethodDeclarationNode ParseMethodDeclaration(string? accessModifier) {
            // Assuming the next token is 'func'
            Eat(TokenType.FUNC);

            // Parse the method name
            string methodName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            // Parse the parameter list
            Eat(TokenType.OPEN_PARENTHESIS);
            List<ParameterNode> parameters = new List<ParameterNode>();
            while (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                parameters.Add(ParseParameter());
                if (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                }
            }
            Eat(TokenType.CLOSE_PARENTHESIS);

            // Parse the return types
            List<TypeNode> returnTypes = new List<TypeNode>();
            if (_currentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                // Parse the first return type
                returnTypes.Add(ParseType());
                while (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);

                    // Break the loop if the next token is an OPEN_BRACE (start of the method body)
                    // This check prevents consuming the OPEN_BRACE as part of the type parsing
                    if (_currentToken.Type == TokenType.OPEN_BRACE) {
                        break;
                    }

                    returnTypes.Add(ParseType());
                }
            }
            // For now, we'll assume the method body is empty and just consume the curly braces
            Eat(TokenType.OPEN_BRACE);
            Eat(TokenType.CLOSE_BRACE);

            // Return the method declaration node
            return new MethodDeclarationNode(methodName, accessModifier, returnTypes, parameters, null);
        }

        private ParameterNode ParseParameter() {
            // Assuming a parameter is declared as 'type name'
            TypeNode parameterType = ParseType();

            string parameterName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            return new ParameterNode(parameterType, parameterName);
        }


        private TypeNode ParseType() {
            string typeName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            bool isNullable = false;
            if (_currentToken.Type == TokenType.QUESTION_MARK) {
                Eat(TokenType.QUESTION_MARK);
                isNullable = true;
            }

            return new TypeNode(typeName, isNullable);
        }

        private List<ImportDeclarationNode> ParseImportDeclarations() {
            List<ImportDeclarationNode> imports = new List<ImportDeclarationNode>();
            while (_currentToken.Type != TokenType.EOF && _currentToken.Type == TokenType.IMPORT) {
                Eat(TokenType.IMPORT);
                QualifiedName qualifiedName = ParseQualifiedName();
                imports.Add(new ImportDeclarationNode(qualifiedName));
            }
            return imports;
        }

        private ASTNode ParseExpression() {
            if (_currentToken.Type == TokenType.NULL) {
                Eat(TokenType.NULL);
                return new NullLiteralNode();
            }
            if (_currentToken.Type == TokenType.NUMBER || _currentToken.Type == TokenType.STRING) {
                LiteralNode literal = new LiteralNode(_currentToken.Value);
                Eat(_currentToken.Type);
                return literal;
            }

            throw new NotImplementedException("Expression parsing not fully implemented.");
        }

        private QualifiedName ParseQualifiedName() {
            List<string> parts = new List<string>();
            parts.Add(_currentToken.Value);
            Eat(TokenType.IDENTIFIER);

            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT);
                parts.Add(_currentToken.Value);
                Eat(TokenType.IDENTIFIER);
            }

            return new QualifiedName(parts);
        }
    }
}
