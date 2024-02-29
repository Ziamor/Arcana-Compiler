using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using static Arcana_Compiler.Common.ErrorReporter;

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

    public class Parser : IParserOld {
        private ILexer? _lexer;
        private readonly ErrorReporter _errorReporter = new ErrorReporter();
        private Token _currentToken;

        private void Eat(TokenType tokenType) {
            if (_lexer == null) {
                throw new InvalidOperationException("Lexer has not been initialized.");
            }
            if (_currentToken.Type == tokenType) {
                _currentToken = _lexer.GetNextToken();
                SkipComments();
            } else if (_currentToken.Type == TokenType.ERROR) {
                ReportError($"Invalid token encountered: '{_currentToken.Value}'", _currentToken.LineNumber, _currentToken.Position, ErrorSeverity.Error);
                _currentToken = _lexer.GetNextToken();
            } else {
                ReportError($"Expected token of type {tokenType}, but found '{_currentToken.Value}'", _currentToken.LineNumber, _currentToken.Position, ErrorSeverity.Error);
                RecoverOrInsertDummyToken(tokenType);
            }
        }

        private void ReportError(string message, int lineNumber, int position, ErrorSeverity severity) {
            _errorReporter.ReportError(new ParseError(message, lineNumber, position, severity));

            if (severity == ErrorSeverity.Fatal) {
                throw new ParsingException(message);
            }
        }

        private void RecoverOrInsertDummyToken(TokenType expectedTokenType) {
            if (CanInsertDummyToken(expectedTokenType)) {
                InsertDummyToken(expectedTokenType);
                return;
            }

            if (!Recover()) {
                throw new ParsingException($"Expected token of type {{tokenType}}, but found '{{_currentToken.Value}}'");
            }
        }

        private bool CanInsertDummyToken(TokenType expectedTokenType) {
            switch (expectedTokenType) {
                case TokenType.SEMICOLON:
                case TokenType.OPEN_BRACE:
                case TokenType.CLOSE_BRACE:
                case TokenType.OPEN_PARENTHESIS:
                case TokenType.CLOSE_PARENTHESIS:
                case TokenType.OPEN_BRACKET: 
                case TokenType.CLOSE_BRACKET:
                    return true;
                default:
                    return false;
            }
        }

        private void InsertDummyToken(TokenType tokenType) {
            // Create a dummy token of the expected type
            Token dummyToken = new Token(tokenType, "", _currentToken.LineNumber, _currentToken.Position, _currentToken.LineText);

            // Report the insertion for clarity
            ReportError($"Inserted dummy token of type {tokenType} to recover from error.", dummyToken.LineNumber, dummyToken.Position, ErrorSeverity.Error);

            if (_lexer == null) {
                throw new InvalidOperationException("Lexer has not been initialized.");
            }

            // Adjust the parser state as if the expected token was encountered
            _currentToken = _lexer.GetNextToken(); // Move past the inserted dummy token
        }


        private bool Recover() {
            if (_lexer == null) {
                throw new InvalidOperationException("Lexer has not been initialized.");
            }

            TokenType originalTokenType = _currentToken.Type;
            int originalLineNumber = _currentToken.LineNumber;
            int originalPosition = _currentToken.Position;

            _currentToken = _lexer.GetNextToken();
            while (_currentToken.Type != TokenType.EOF && !IsRecoveryPoint(_currentToken)) {
                _currentToken = _lexer.GetNextToken();
            }

            if (_currentToken.Type == TokenType.EOF) {
                ReportError($"Unexpected end of file during recovery for token of type {originalTokenType}.", originalLineNumber, originalPosition, ErrorSeverity.Fatal);
            }

            return _currentToken.Type != TokenType.EOF;
        }

        private bool IsRecoveryPoint(Token token) {
            switch (token.Type) {
                case TokenType.FUNC:
                case TokenType.CLASS:
                case TokenType.IF:
                case TokenType.FOR:
                case TokenType.WHILE:
                case TokenType.DO:
                case TokenType.NAMESPACE:
                case TokenType.INTERFACE:
                case TokenType.PUBLIC:
                case TokenType.PRIVATE:
                case TokenType.PROTECTED:
                    return true;
                default:
                    return false;
            }
        }


        private Token PeekNextToken(int depth = 1) {
            if (_lexer == null) {
                throw new InvalidOperationException("Lexer has not been initialized.");
            }
            return _lexer.PeekToken(depth);
        }

        private void SkipComments() {
            while (_currentToken.Type == TokenType.COMMENT) {
                Eat(TokenType.COMMENT);
            }
        }

        public ProgramNode Parse(ILexer lexer, out ErrorReporter errorReporter) {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken(); // Initialize with the first token
            SkipComments(); // Skip any comments

            ProgramNode rootNode = new ProgramNode();
            rootNode.Imports = ParseImportDeclarations();

            List<ClassDeclarationNode> defaultNamespaceClasses = new List<ClassDeclarationNode>();
            List<InterfaceDeclarationNode> defaultNamespaceInterfaces = new List<InterfaceDeclarationNode>();

            while (_currentToken.Type != TokenType.EOF) {
                switch (_currentToken.Type) {
                    case TokenType.NAMESPACE:
                        ParseNamespaceDeclaration(rootNode);
                        break;
                    case TokenType.CLASS:
                        defaultNamespaceClasses.Add(ParseClassDeclaration());
                        break;
                    case TokenType.INTERFACE:
                        defaultNamespaceInterfaces.Add(ParseInterfaceDeclaration());
                        break;
                    case TokenType type when IsAccessModifier(type):
                        defaultNamespaceClasses.Add(ParseClassDeclaration());
                        break;
                    default:
                        throw new UnexpectedTokenException(_currentToken);
                }
            }

            if (defaultNamespaceClasses.Any()) {
                rootNode.NamespaceDeclarations.Add(new NamespaceDeclarationNode(new IdentifierName("default"), defaultNamespaceClasses, defaultNamespaceInterfaces));
            }

            errorReporter = _errorReporter;
            return rootNode;
        }

        private void ParseNamespaceDeclaration(ProgramNode rootNode) {
            Eat(TokenType.NAMESPACE);
            IdentifierName namespaceName = ParseIdentifierName();
            Eat(TokenType.OPEN_BRACE);

            List<ClassDeclarationNode> classDeclarations = new List<ClassDeclarationNode>();
            List<InterfaceDeclarationNode> interfaceDeclarations = new List<InterfaceDeclarationNode>(); // Added for interfaces

            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                switch (_currentToken.Type) {
                    case TokenType.CLASS:
                    case TokenType type when IsAccessModifier(type):
                        classDeclarations.Add(ParseClassDeclaration(namespaceName));
                        break;
                    case TokenType.INTERFACE:
                        interfaceDeclarations.Add(ParseInterfaceDeclaration());
                        break;
                    default:
                        throw new UnexpectedTokenException(_currentToken);
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            rootNode.NamespaceDeclarations.Add(new NamespaceDeclarationNode(namespaceName, classDeclarations, interfaceDeclarations));
        }


        private ClassDeclarationNode ParseClassDeclaration(IdentifierName? currentNamespace = null) {
            string? classAccessModifier = TryParseAccessModifier();
            List<ClassModifierNode> classModifiers = ParseClassModifiers();

            Eat(TokenType.CLASS);
            string className = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParentTypeNode> parentTypes = ParseParentTypes();

            Eat(TokenType.OPEN_BRACE);

            List<FieldDeclarationNode> fields = new List<FieldDeclarationNode>();
            List<MethodDeclarationNode> methods = new List<MethodDeclarationNode>();
            List<ClassDeclarationNode> nestedClasses = new List<ClassDeclarationNode>();

            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                string? accessModifier = TryParseAccessModifier();

                if (_currentToken.Type == TokenType.CLASS) {
                    nestedClasses.Add(ParseClassDeclaration(currentNamespace)); // Recursively parse nested class
                } else if (IsMethodDeclaration()) {
                    methods.Add(ParseMethodDeclaration(accessModifier));
                } else {
                    fields.Add(ParseFieldDeclaration(accessModifier));
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            if (currentNamespace == null) {
                currentNamespace = IdentifierName.DefaultNameSpace;
            }

            // Concatenate the class name to the namespace
            currentNamespace += className;
            return new ClassDeclarationNode(currentNamespace, classAccessModifier, classModifiers, parentTypes, fields, methods, nestedClasses);
        }

        private List<ClassModifierNode> ParseClassModifiers() {
            List<ClassModifierNode> modifiers = new List<ClassModifierNode>();
            while (IsClassModifier(_currentToken.Type)) {
                modifiers.Add(new ClassModifierNode(_currentToken.Value));
                Eat(_currentToken.Type);
            }
            return modifiers;
        }

        private bool IsClassModifier(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.STATIC:
                case TokenType.ABSTRACT:
                case TokenType.FINAL:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsMethodDeclaration() {
            // Check if the current token is 'func'.
            if (_currentToken.Type == TokenType.FUNC) {
                return true;
            }

            // Check up to the next 3 tokens ahead for a 'func' keyword, allowing for modifiers before it.
            for (int i = 1; i <= 3; i++) {
                Token nextToken = PeekNextToken(i);
                if (nextToken.Type == TokenType.FUNC) {
                    return true;
                }
                // If the token is neither an access modifier nor a method modifier, stop checking further.
                if (!IsAccessModifier(nextToken.Type) && !IsMethodModifier(nextToken.Type)) {
                    break;
                }
            }

            return false; // If 'func' is not found within the allowed range, it's not a method declaration.
        }

        private bool IsMethodModifier(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.STATIC:
                case TokenType.ABSTRACT:
                case TokenType.VIRTUAL:
                case TokenType.OVERRIDE:
                case TokenType.FINAL:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsAccessModifier(TokenType tokenType) {
            return tokenType == TokenType.PUBLIC || tokenType == TokenType.PRIVATE || tokenType == TokenType.PROTECTED;
        }


        /// <summary>
        /// Try to parse an access modifer, will return null if none is found.
        /// </summary>
        /// <returns>A string if an access modifer is found, null otherwise.</returns>
        private string? TryParseAccessModifier() {
            if (IsAccessModifier(_currentToken.Type)) {
                string modifier = _currentToken.Value;
                Eat(_currentToken.Type);
                return modifier;
            }
            return null; // No access modifier present
        }
        private InterfaceDeclarationNode ParseInterfaceDeclaration() {
            Eat(TokenType.INTERFACE);
            string interfaceName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<MethodSignatureNode> methods = new List<MethodSignatureNode>();

            Eat(TokenType.OPEN_BRACE);
            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                methods.Add(ParseMethodSignature());
            }
            Eat(TokenType.CLOSE_BRACE);

            return new InterfaceDeclarationNode(new IdentifierName(interfaceName), methods);
        }

        private MethodSignatureNode ParseMethodSignature() {
            string? accessModifier = TryParseAccessModifier();
            Eat(TokenType.FUNC);
            string methodName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParameterNode> parameters = ParseParameters();

            List<TypeNode> returnTypes = new List<TypeNode>();
            if (_currentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                returnTypes.Add(ParseType());
                while (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    if (_currentToken.Type == TokenType.FUNC || IsAccessModifier(_currentToken.Type)) {
                        break;
                    }

                    returnTypes.Add(ParseType());
                }
            }

            return new MethodSignatureNode(methodName, parameters, returnTypes);
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
            List<FieldModifierNode> fieldModifiers = ParseFieldModifiers(); // Implement this method

            TypeNode fieldType = ParseType();
            string fieldName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            ASTNode? initialValue = null;
            if (_currentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                initialValue = ParseExpression();
            }

            return new FieldDeclarationNode(fieldType, fieldName, accessModifier, fieldModifiers, initialValue);
        }

        private List<FieldModifierNode> ParseFieldModifiers() {
            List<FieldModifierNode> modifiers = new List<FieldModifierNode>();
            while (IsFieldModifier(_currentToken.Type)) {
                modifiers.Add(new FieldModifierNode(_currentToken.Value));
                Eat(_currentToken.Type);
            }
            return modifiers;
        }

        private bool IsFieldModifier(TokenType tokenType) {
            switch (tokenType) {
                case TokenType.STATIC:
                case TokenType.CONST:
                    return true;
                default:
                    return false;
            }
        }

        private MethodDeclarationNode ParseMethodDeclaration(string? accessModifier) {
            List<MethodModifierNode> methodModifiers = ParseMethodModifiers();

            Eat(TokenType.FUNC);

            // Parse the method name
            string methodName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            // Parse the parameter list
            List<ParameterNode> parameters = ParseParameters();

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

            Eat(TokenType.OPEN_BRACE);
            List<ASTNode> methodBody = new List<ASTNode>();
            while (_currentToken.Type != TokenType.CLOSE_BRACE) {
                if (_currentToken.Type == TokenType.RETURN) {
                    methodBody.Add(ParseReturnStatement());
                    break;
                }
                methodBody.Add(ParseStatement());
            }
            Eat(TokenType.CLOSE_BRACE);

            // Return the method declaration node
            return new MethodDeclarationNode(methodName, accessModifier, methodModifiers, returnTypes, parameters, methodBody);
        }

        private List<MethodModifierNode> ParseMethodModifiers() {
            List<MethodModifierNode> modifiers = new List<MethodModifierNode>();
            while (IsMethodModifier(_currentToken.Type)) {
                modifiers.Add(new MethodModifierNode(_currentToken.Value));
                Eat(_currentToken.Type);
            }
            return modifiers;
        }

        private List<ParameterNode> ParseParameters() {
            Eat(TokenType.OPEN_PARENTHESIS);
            List<ParameterNode> parameters = new List<ParameterNode>();
            while (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                parameters.Add(ParseParameter());
                if (_currentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                }
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return parameters;
        }
        private ParameterNode ParseParameter() {
            TypeNode parameterType = ParseType();

            string parameterName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            return new ParameterNode(parameterType, parameterName);
        }
        private ReturnStatementNode ParseReturnStatement() {
            Eat(TokenType.RETURN);
            List<ASTNode> returnExpressions = new List<ASTNode>();

            returnExpressions.Add(ParseExpression()); // Parse the first expression
            while (_currentToken.Type == TokenType.COMMA) {
                Eat(TokenType.COMMA); // Eat the comma to move to the next expression
                returnExpressions.Add(ParseExpression()); // Parse subsequent expressions
            }

            return new ReturnStatementNode(returnExpressions);
        }
        private StatementNode ParseStatement() {
            if (_currentToken.Type == TokenType.IDENTIFIER) {
                // Peek next token to decide between declaration, assignment, or method call
                Token nextToken = PeekNextToken();
                switch (nextToken.Type) {
                    case TokenType.IDENTIFIER:
                        return ParseVariableDeclaration();
                    case TokenType.DOT: {
                            ExpressionNode expression = ParseIdentifierOrMethodCall();
                            return new ExpressionStatementNode(expression);
                        }
                    case TokenType.ASSIGN:
                        return ParseVariableAssignment();
                    case TokenType.OPEN_PARENTHESIS: {
                            ExpressionNode expression = ParseMethodCall(ParseIdentifierName());
                            return new ExpressionStatementNode(expression);
                        }
                    default:
                        ReportError($"Unexpected token '{nextToken.Value}' encountered.", nextToken.LineNumber, nextToken.Position, ErrorSeverity.Error);
                        Recover(); // Attempt to recover
                        return new ErrorStatementNode();
                }
            } else {
                switch (_currentToken.Type) {
                    case TokenType.IF:
                        return ParseIfStatement();
                    case TokenType.THIS:
                        return ParseThisAssignment();
                    case TokenType.FOR:
                        return ParseForLoop();
                    default:
                        ReportError($"Unexpected token '{_currentToken.Value}' encountered.", _currentToken.LineNumber, _currentToken.Position, ErrorSeverity.Error);
                        Recover(); // Attempt to recover
                        return new ErrorStatementNode();
                }
            }
        }

        private StatementNode ParseForLoop() {
            Eat(TokenType.FOR);
            Eat(TokenType.OPEN_PARENTHESIS);

            Token nextToken = PeekNextToken();
            if (nextToken.Type == TokenType.IN) {
                return ParseForEachLoop();
            } else {
                return ParseTraditionalForLoop();
            }
        }

        private ForEachLoopNode ParseForEachLoop() {
            string variableName = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);
            Eat(TokenType.IN);

            ASTNode collection = ParseExpression();

            Eat(TokenType.CLOSE_PARENTHESIS);

            Eat(TokenType.OPEN_BRACE);
            List<ASTNode> body = ParseBlockOrStatement();
            Eat(TokenType.CLOSE_BRACE);

            VariableDeclarationNode variableDeclaration = new VariableDeclarationNode(new TypeNode("var", false), variableName, null);
            return new ForEachLoopNode(variableDeclaration, collection, body);
        }

        private ForLoopNode ParseTraditionalForLoop() {
            ASTNode initialization = ParseStatement();
            Eat(TokenType.SEMICOLON);

            ASTNode condition = ParseExpression();
            Eat(TokenType.SEMICOLON);

            ASTNode increment = ParseExpression();

            Eat(TokenType.CLOSE_PARENTHESIS);

            Eat(TokenType.OPEN_BRACE);
            List<ASTNode> body = ParseBlockOrStatement();
            Eat(TokenType.CLOSE_BRACE);

            return new ForLoopNode(initialization, condition, increment, body);
        }

        private StatementNode ParseThisAssignment() {
            Eat(TokenType.THIS);
            Eat(TokenType.DOT);
            IdentifierName identifierName = ParseIdentifierName();

            // Expect an assignment operator next
            if (_currentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                ASTNode valueExpression = ParseExpression();
                return new ThisAssignmentNode(identifierName, valueExpression);
            } else {
                throw new UnexpectedTokenException(_currentToken);
            }
        }
        private StatementNode ParseVariableDeclaration() {
            List<(TypeNode Type, string Name)> tempDeclarations = new List<(TypeNode, string)>();
            bool expectComma;

            // First, parse the types and names of the variables to be declared
            do {
                TypeNode variableType = ParseType();
                string variableName = _currentToken.Value;
                Eat(TokenType.IDENTIFIER);
                tempDeclarations.Add((variableType, variableName));

                expectComma = _currentToken.Type == TokenType.COMMA;
                if (expectComma) {
                    Eat(TokenType.COMMA);
                }
            } while (expectComma);

            ASTNode? initialValue = null;
            if (_currentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                initialValue = ParseExpression(); // This parses the right-hand side, which could be a function call or another expression
            }

            // Now, create the variable declaration nodes with the initial value
            if (tempDeclarations.Count == 1) {
                // If there's only one variable, create a single VariableDeclarationNode
                var declaration = tempDeclarations[0];
                return new VariableDeclarationNode(declaration.Type, declaration.Name, initialValue);
            } else {
                // For multiple variables, prepare for a destructuring assignment
                List<VariableDeclarationNode> variableDeclarations = tempDeclarations
                    .Select(declaration => new VariableDeclarationNode(declaration.Type, declaration.Name, null))
                    .ToList();

                return new DestructuringAssignmentNode(variableDeclarations, initialValue);
            }
        }


        private StatementNode ParseVariableAssignment() {
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
                IdentifierName qualifiedName = ParseIdentifierName();
                imports.Add(new ImportDeclarationNode(qualifiedName));
            }
            return imports;
        }

        private ExpressionNode ParseExpression(int parentPrecedence = 0) {
            ExpressionNode node;
            // Initial parsing as before
            if (IsUnaryOperator(_currentToken)) {
                node = ParseUnaryOperation();
            } else {
                node = ParsePrimaryExpression();
            }

            // Handling binary operations as before
            while (IsBinaryOperator(_currentToken) && GetPrecedence(_currentToken.Type) > parentPrecedence) {
                Token operatorToken = _currentToken;
                Eat(operatorToken.Type);

                int precedence = GetPrecedence(operatorToken.Type);
                ExpressionNode right = ParseExpression(precedence);
                node = new BinaryOperationNode(node, operatorToken, right);
            }

            // New: Handling chained calls (e.g., .method() or .property)
            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT); // Consume the dot, move to next token (method name or property name)

                // Now determine if it's a method call or a property access
                if (_currentToken.Type == TokenType.IDENTIFIER) {
                    Token nextToken = PeekNextToken();
                    if (nextToken.Type == TokenType.OPEN_PARENTHESIS) {
                        // It's a method call
                        IdentifierName identifierName = ParseIdentifierName();
                        MethodCallNode methodCallNode = ParseMethodCall(identifierName);
                        node = new ChainedMethodCallNode(node, methodCallNode);
                    } else {
                        // TODO, probably a property
                    }
                } else {
                    throw new SyntaxErrorException("Expected identifier", _currentToken);
                }
            }

            return node;
        }


        /// <summary>
        /// Parses a primary expression, which includes literals (numbers and strings), the 'null' keyword,
        /// identifiers (which could be variables or method calls), and expressions enclosed in parentheses.
        /// This function serves as the base case for the recursive descent parsing of expressions,
        /// handling the most atomic elements that do not contain other expressions.
        /// </summary>
        /// <returns>An ExpressionNode representing the parsed primary expression.</returns>
        /// <exception cref="UnexpectedTokenException">Thrown when an unexpected token is encountered.</exception>
        private ExpressionNode ParsePrimaryExpression() {
            switch (_currentToken.Type) {
                case TokenType.NUMBER:
                    return ParseNumberLiteral();
                case TokenType.STRING:
                    return ParseStringLiteral();
                case TokenType.NULL:
                    Eat(TokenType.NULL);
                    return new NullLiteralNode();
                case TokenType.THIS:
                    return ParseThisExpression();
                case TokenType.IDENTIFIER:
                    var identifierOrMethodCall = ParseIdentifierOrMethodCall();
                    if (_currentToken.Type == TokenType.OPEN_BRACKET) {
                        return ParseArrayAccess(identifierOrMethodCall);
                    }
                    return identifierOrMethodCall;
                case TokenType.OPEN_PARENTHESIS:
                    Eat(TokenType.OPEN_PARENTHESIS);
                    ExpressionNode expression = ParseExpression();
                    Eat(TokenType.CLOSE_PARENTHESIS);
                    return expression;
                case TokenType.NEW:
                    Eat(TokenType.NEW);
                    IdentifierName className = ParseIdentifierName();
                    List<ASTNode> constructorArguments = new List<ASTNode>();
                    if (_currentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                        constructorArguments = ParseArguments();
                    }
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

        private ExpressionNode ParseArrayAccess(ExpressionNode array) {
            Eat(TokenType.OPEN_BRACKET);
            ExpressionNode index = ParseExpression();
            Eat(TokenType.CLOSE_BRACKET);

            return new ArrayAccessNode(array, index);
        }

        private ExpressionNode ParseIdentifierOrMethodCall() {
            IdentifierName qualifiedName = ParseIdentifierName();
            ExpressionNode currentNode;
            if (_currentToken.Type == TokenType.OPEN_PARENTHESIS) {
                currentNode = ParseMethodCall(qualifiedName);
            } else {
                currentNode = new VariableAccessNode(qualifiedName);

                if (IsPostfixUnaryOperator(_currentToken)) {
                    currentNode = ParseUnaryOperation(currentNode);
                }
            }

            // Handle chaining after the initial method call or variable access
            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT); // Consume the dot
                currentNode = ParseChainedCall(currentNode);
            }

            return currentNode;
        }

        private ExpressionNode ParseChainedCall(ASTNode previousNode) {
            string nextIdentifier = _currentToken.Value;
            Eat(TokenType.IDENTIFIER);

            if (_currentToken.Type == TokenType.OPEN_PARENTHESIS) {
                // Reuse the existing method to parse method calls and create a MethodCallNode
                IdentifierName qualifiedName = new IdentifierName(new List<string> { nextIdentifier });
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


        private ExpressionNode ParseUnaryOperation(ExpressionNode? operand = null) {
            Token operatorToken = _currentToken;
            bool isPrefix = operand == null;
            UnaryOperatorPosition position = isPrefix ? UnaryOperatorPosition.Prefix : UnaryOperatorPosition.Postfix;

            if (isPrefix) {
                Eat(operatorToken.Type);
                operand = ParseExpression();

                if (operand is UnaryOperationNode unaryOperand) {
                    // e.g --i++
                    throw new ParsingException($"Chaining unary operators is not allowed at line {operatorToken.LineNumber}, position {operatorToken.Position}.");
                }
            } else {
                // For postfix, since operand is already parsed, simply check if the next token is a unary operator, e.g --i++
                if (IsPostfixUnaryOperator(PeekNextToken())) {
                    throw new ParsingException($"Chaining unary operators is not allowed at line {operatorToken.LineNumber}, position {operatorToken.Position}.");
                }

                Eat(operatorToken.Type);
            }

            if (operand == null) {
                throw new InvalidOperationException("Unary operation missing operand.");
            }

            return new UnaryOperationNode(operatorToken, operand, position);
        }

        private ExpressionNode ParseBinaryExpression(int parentPrecedence = 0) {
            ExpressionNode left = ParsePrimaryExpression(); // Parse the left-hand side

            while (true) {
                int precedence = GetPrecedence(_currentToken.Type);
                if (precedence <= parentPrecedence) {
                    break;
                }

                Token operatorToken = _currentToken;
                Eat(_currentToken.Type);

                // Parse the right-hand side with higher precedence
                ExpressionNode right = ParseBinaryExpression(precedence);

                left = new BinaryOperationNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParseThisExpression() {
            Eat(TokenType.THIS);
            ThisExpressionNode thisNode = new ThisExpressionNode(null);

            // Handle property or method access after 'this', similar to how you handle chained calls.
            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT); // Consume the dot
                Console.WriteLine("TODO");
                //thisNode = ParseChainedCall(thisNode);
            }

            return thisNode;
        }

        private bool IsUnaryOperator(Token token) {
            return token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT ||
                   token.Type == TokenType.MINUS || token.Type == TokenType.NOT;
        }

        private bool IsPostfixUnaryOperator(Token token) {
            return token.Type == TokenType.INCREMENT || token.Type == TokenType.DECREMENT;
        }

        private bool IsBinaryOperator(Token token) {
            return token.Type == TokenType.PLUS || token.Type == TokenType.MINUS ||
                   token.Type == TokenType.MULTIPLY || token.Type == TokenType.DIVIDE ||
                   token.Type == TokenType.EQUALS || token.Type == TokenType.LESS_THAN ||
                   token.Type == TokenType.GREATER_THAN || token.Type == TokenType.LESS_THAN_OR_EQUAL ||
                   token.Type == TokenType.GREATER_THAN_OR_EQUAL;
        }

        private int GetPrecedence(TokenType tokenType) {
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

        private MethodCallNode ParseMethodCall(IdentifierName identifierName) {
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
            return new MethodCallNode(identifierName, arguments);
        }

        private IdentifierName ParseIdentifierName() {
            List<string> parts = new List<string>();
            parts.Add(_currentToken.Value);
            Eat(TokenType.IDENTIFIER);

            while (_currentToken.Type == TokenType.DOT) {
                Eat(TokenType.DOT);
                if (_currentToken.Type == TokenType.IDENTIFIER || _currentToken.Type == TokenType.MULTIPLY) {
                    parts.Add(_currentToken.Value);
                    Eat(_currentToken.Type);
                }
            }

            return new IdentifierName(parts);
        }
    }
}
