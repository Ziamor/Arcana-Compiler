using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.ArcanaParser
{
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
                throw new SyntaxErrorException(tokenType, _currentToken);
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

        public ProgramNode Parse() {
            ProgramNode rootNode = new ProgramNode();

            // Parse imports first
            rootNode.Imports = ParseImportDeclarations();

            while (_currentToken.Type != TokenType.EOF) {
                switch (_currentToken.Type) {
                    case TokenType.NAMESPACE:
                        ParseNamespaceDeclaration(rootNode);
                        break;
                    case TokenType.CLASS:
                        rootNode.ClassDeclarations.Add(ParseClassDeclaration(null));
                        break;
                    default:
                        throw new UnexpectedTokenException(_currentToken);
                }
            }

            return rootNode;
        }

        private void ParseNamespaceDeclaration(ProgramNode rootNode) {
            Eat(TokenType.NAMESPACE);
            QualifiedName namespaceName = ParseQualifiedName();
            Eat(TokenType.OPEN_BRACE);

            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                if (_currentToken.Type == TokenType.CLASS) {
                    rootNode.ClassDeclarations.Add(ParseClassDeclaration(namespaceName));
                } else {
                    // Handle error or unexpected token
                }
            }

            Eat(TokenType.CLOSE_BRACE);
        }

        private ClassDeclarationNode ParseClassDeclaration(QualifiedName? currentNamespace) {
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

            return new ClassDeclarationNode(className, currentNamespace, parentTypes, fields, methods);
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
            List<ASTNode> methodBody = new List<ASTNode>();
            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                methodBody.Add(ParseStatement());
            }
            Eat(TokenType.CLOSE_BRACE);

            // Return the method declaration node
            return new MethodDeclarationNode(methodName, accessModifier, returnTypes, parameters, methodBody);
        }

        private ParameterNode ParseParameter() {
            // Assuming a parameter is declared as 'type name'
            TypeNode parameterType = ParseType();

            string parameterName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            return new ParameterNode(parameterType, parameterName);
        }

        private ASTNode ParseStatement() {
            if (_currentToken.Type == TokenType.IDENTIFIER) {
                // Peek next token to decide between declaration, assignment, or method call
                Token nextToken = PeekNextToken();
                switch (nextToken.Type) {
                    case TokenType.IDENTIFIER:
                        return ParseVariableDeclaration();
                    case TokenType.ASSIGN:
                        return ParseVariableAssignment();
                    case TokenType.OPEN_PARENTHESIS:
                        return ParseMethodCall(ParseQualifiedName());
                    default:
                        throw new NotImplementedException("Unrecognized statement pattern.");
                }
            } else {
                switch (_currentToken.Type) {
                    case TokenType.IF:
                        return ParseIfStatement();
                    default:
                        throw new UnexpectedTokenException(_currentToken);
                }
            }
        }

        private VariableDeclarationNode ParseVariableDeclaration() {
            TypeNode variableType = ParseType();

            string variableName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            ASTNode? initialValue = null;
            if (_currentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                initialValue = ParseExpression();
            }

            return new VariableDeclarationNode(variableType, variableName, initialValue);
        }

        private ASTNode ParseVariableAssignment() {
            string variableName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);
            Eat(TokenType.ASSIGN);
            ASTNode expression = ParseExpression();
            return new VariableAssignmentNode(variableName, expression);
        }

        private IfStatementNode ParseIfStatement() {
            List<(ASTNode Condition, List<ASTNode> Statements)> conditionsAndStatements = new List<(ASTNode Condition, List<ASTNode> Statements)>();
            List<ASTNode>? elseStatements = null;

            // Parse the initial 'if' condition and its block
            Eat(TokenType.IF);
            Eat(TokenType.OPEN_PARENTHESIS);
            ASTNode initialCondition = ParseExpression();
            Eat(TokenType.CLOSE_PARENTHESIS);
            List<ASTNode> initialThenStatements = ParseBlockOrStatement();

            conditionsAndStatements.Add((initialCondition, initialThenStatements));

            // Parse any 'else if' branches
            while (_currentToken.Type == TokenType.ELSE) {
                Eat(TokenType.ELSE);
                if (_currentToken.Type == TokenType.IF) {
                    Eat(TokenType.IF);
                    Eat(TokenType.OPEN_PARENTHESIS);
                    ASTNode elseIfCondition = ParseExpression();
                    Eat(TokenType.CLOSE_PARENTHESIS);
                    List<ASTNode> elseIfStatements = ParseBlockOrStatement();

                    conditionsAndStatements.Add((elseIfCondition, elseIfStatements));
                } else {
                    elseStatements = ParseBlockOrStatement();
                    break; // After 'else', no more 'else if' or 'else' should be parsed
                }
            }

            return new IfStatementNode(conditionsAndStatements, elseStatements);
        }

        private List<ASTNode> ParseBlockOrStatement() {
            if (_currentToken.Type == TokenType.OPEN_BRACE) {
                Eat(TokenType.OPEN_BRACE);
                List<ASTNode> statements = new List<ASTNode>();
                while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                    statements.Add(ParseStatement());
                }
                Eat(TokenType.CLOSE_BRACE);
                return statements;
            } else {
                // If not a block, parse a single statement
                return new List<ASTNode> { ParseStatement() };
            }
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

        private ASTNode ParseExpression(int parentPrecedence = 0) {
            ASTNode node;
            // Handle unary operations first
            if (_currentToken.Type == TokenType.MINUS || _currentToken.Type == TokenType.NOT) {
                node = ParseUnaryOperation();
            } else {
                node = ParsePrimaryExpression();
            }

            // Now handle binary operations
            while (IsBinaryOperator(_currentToken) && GetPrecedence(_currentToken.Type) > parentPrecedence) {
                Token operatorToken = _currentToken;
                if (!IsBinaryOperator(_currentToken)) {
                    throw new SyntaxErrorException("binary operator", _currentToken);
                }

                Eat(operatorToken.Type);
                
                int precedence = GetPrecedence(operatorToken.Type);
                ASTNode right = ParseExpression(precedence);
                node = new BinaryOperationNode(node, operatorToken, right);
            }
            return node;
        }

        /// <summary>
        /// Parses a primary expression, which includes literals (numbers and strings), the 'null' keyword,
        /// identifiers (which could be variables or method calls), and expressions enclosed in parentheses.
        /// This function serves as the base case for the recursive descent parsing of expressions,
        /// handling the most atomic elements that do not contain other expressions.
        /// </summary>
        /// <returns>An ASTNode representing the parsed primary expression.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token is encountered.</exception>
        private ASTNode ParsePrimaryExpression() {
            switch (_currentToken.Type) {
                case TokenType.NUMBER:
                    return ParseNumberLiteral();
                case TokenType.STRING:
                    return ParseStringLiteral();
                case TokenType.NULL:
                    Eat(TokenType.NULL);
                    return new NullLiteralNode();
                case TokenType.IDENTIFIER:
                    return ParseIdentifierOrMethodCall();
                case TokenType.OPEN_PARENTHESIS:
                    Eat(TokenType.OPEN_PARENTHESIS);
                    ASTNode expression = ParseExpression();
                    Eat(TokenType.CLOSE_PARENTHESIS);
                    return expression;
                case TokenType.NEW:
                    Eat(TokenType.NEW);
                    QualifiedName className = ParseQualifiedName();
                    List<ASTNode> constructorArguments = new List<ASTNode>();
                    Eat(TokenType.OPEN_PARENTHESIS);
                    if (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                        constructorArguments = ParseArguments();
                    }
                    Eat(TokenType.CLOSE_PARENTHESIS);
                    return new ObjectInstantiationNode(className, constructorArguments);
                default:
                    throw new UnexpectedTokenException(_currentToken); ;
            }
        }

        private LiteralNode ParseNumberLiteral() {
            string tokenValue = _currentToken.Value;
            object value;

            if (tokenValue.Contains(".")) {
                // If the value contains a decimal point, treat it as a float
                if (float.TryParse(tokenValue, out float floatValue)) {
                    value = floatValue;
                } else {
                    throw new ParsingException($"Invalid float literal '{tokenValue}' at line {_currentToken.LineNumber}, position {_currentToken.Position}.");
                }
            } else {
                // If there's no decimal point, treat it as an integer
                if (int.TryParse(tokenValue, out int intValue)) {
                    value = intValue;
                } else {
                    throw new ParsingException($"Invalid integer literal '{tokenValue}' at line {_currentToken.LineNumber}, position {_currentToken.Position}.");
                }
            }

            LiteralNode node = new LiteralNode(value);
            Eat(TokenType.NUMBER);
            return node;
        }


        private LiteralNode ParseStringLiteral() {
            LiteralNode node = new LiteralNode(_currentToken.Value);
            Eat(TokenType.STRING);
            return node;
        }

        private ASTNode ParseIdentifierOrMethodCall() {
            QualifiedName qualifiedName = ParseQualifiedName();
            ASTNode currentNode;
            if (_currentToken.Type == TokenType.OPEN_PARENTHESIS) {
                currentNode = ParseMethodCall(qualifiedName);
            } else {
                currentNode = new VariableAccessNode(qualifiedName);
            }

            // Handle chaining after the initial method call or variable access
            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT); // Consume the dot
                currentNode = ParseChainedCall(currentNode);
            }

            return currentNode;
        }

        private ASTNode ParseChainedCall(ASTNode previousNode) {
            // Assuming the next token is an identifier (either a method name or a property name)
            string nextIdentifier = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            if (_currentToken.Type == TokenType.OPEN_PARENTHESIS) {
                // Reuse the existing method to parse method calls and create a MethodCallNode
                QualifiedName qualifiedName = new QualifiedName(new List<string> { nextIdentifier });
                MethodCallNode methodCall = ParseMethodCall(qualifiedName);

                // Create a ChainedMethodCallNode with the previous node and the parsed MethodCallNode
                return new ChainedMethodCallNode(previousNode, methodCall);
            } else {
                // If it's not a method call, treat it as a property access and create a chained property access node
                return new ChainedPropertyAccessNode(previousNode, nextIdentifier);
            }
        }


        private List<ASTNode> ParseArguments() {
            List<ASTNode> arguments = new List<ASTNode>();
            Eat(TokenType.OPEN_PARENTHESIS);
            if (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                arguments.Add(ParseExpression());
                while (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    arguments.Add(ParseExpression());
                }
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return arguments;
        }


        private UnaryOperationNode ParseUnaryOperation() {
            Token operatorToken = _currentToken;
            Eat(_currentToken.Type);
            ASTNode operand = ParseExpression();
            return new UnaryOperationNode(operatorToken, operand);
        }

        private ASTNode ParseBinaryExpression(int parentPrecedence = 0) {
            ASTNode left = ParsePrimaryExpression(); // Parse the left-hand side

            while (true) {
                int precedence = GetPrecedence(_currentToken.Type);
                if (precedence <= parentPrecedence) {
                    break;
                }

                Token operatorToken = _currentToken;
                Eat(_currentToken.Type);

                // Parse the right-hand side with higher precedence
                ASTNode right = ParseBinaryExpression(precedence);

                left = new BinaryOperationNode(left, operatorToken, right);
            }

            return left;
        }
        private bool IsBinaryOperator(Token token) {
            return token.Type == TokenType.PLUS || token.Type == TokenType.MINUS ||
                   token.Type == TokenType.MULTIPLY || token.Type == TokenType.DIVIDE;
        }

        private int GetPrecedence(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    return 1;
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                    return 2;
                default:
                    return 0;
            }
        }

        private MethodCallNode ParseMethodCall(QualifiedName qualifiedName) {
            string methodName = qualifiedName.Identifier;
            Eat(TokenType.OPEN_PARENTHESIS);
            List<ASTNode> arguments = new List<ASTNode>();

            if (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                arguments.Add(ParseExpression());
                while (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    arguments.Add(ParseExpression());
                }
            }
            if (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                throw new SyntaxErrorException(TokenType.CLOSE_PARENTHESIS, _currentToken);
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return new MethodCallNode(methodName, arguments);
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
