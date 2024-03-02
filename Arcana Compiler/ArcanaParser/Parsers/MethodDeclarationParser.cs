using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class MethodDeclarationParser : BaseParser<MethodDeclarationNode> {
        public MethodDeclarationParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override MethodDeclarationNode Parse() {
            string? accessModifier = TryParseAccessModifier();

            List<MethodModifierNode> methodModifiers = ParseMethodModifiers();

            Eat(TokenType.FUNC);

            // Parse the method name
            string methodName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            // Parse the parameter list
            List<ParameterNode> parameters = ParseParameters();

            // Parse the return types
            List<TypeNode> returnTypes = new List<TypeNode>();
            if (CurrentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                // Parse the first return type
                returnTypes.Add(ParseType());
                while (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);

                    // Break the loop if the next token is an OPEN_BRACE (start of the method body)
                    // This check prevents consuming the OPEN_BRACE as part of the type parsing
                    if (CurrentToken.Type == TokenType.OPEN_BRACE) {
                        break;
                    }

                    returnTypes.Add(ParseType());
                }
            }

            Eat(TokenType.OPEN_BRACE);
            List<ASTNode> methodBody = new List<ASTNode>();
            while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                if (CurrentToken.Type == TokenType.RETURN) {
                    methodBody.Add(ParseReturnStatement());
                    break;
                }
                methodBody.Add(ParseStatement());
            }
            Eat(TokenType.CLOSE_BRACE);

            // Return the method declaration node
            return new MethodDeclarationNode(methodName, accessModifier, methodModifiers, returnTypes, parameters, methodBody);
        }

        private StatementNode ParseStatement() {
            StatementNode statementNode = parserFactory.CreateParser<StatementNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();

            return statementNode;
        }

        private ReturnStatementNode ParseReturnStatement() {
            Eat(TokenType.RETURN);
            List<ASTNode> returnExpressions =
            [
                ParseExpression(), // Parse the first expression
            ];
            while (CurrentToken.Type == TokenType.COMMA) {
                Eat(TokenType.COMMA); // Eat the comma to move to the next expression
                returnExpressions.Add(ParseExpression()); // Parse subsequent expressions
            }

            return new ReturnStatementNode(returnExpressions);
        }

        private List<MethodModifierNode> ParseMethodModifiers() {
            List<MethodModifierNode> modifiers = new List<MethodModifierNode>();
            while (IsMethodModifier(CurrentToken.Type)) {
                modifiers.Add(new MethodModifierNode(CurrentToken.Value));
                Eat(CurrentToken.Type);
            }
            return modifiers;
        }

        private List<ParameterNode> ParseParameters() {
            Eat(TokenType.OPEN_PARENTHESIS);
            List<ParameterNode> parameters = new List<ParameterNode>();
            while (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                parameters.Add(ParseParameter());
                if (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                }
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return parameters;
        }

        private ParameterNode ParseParameter() {
            TypeNode parameterType = ParseType();

            string parameterName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            return new ParameterNode(parameterType, parameterName);
        }
        private TypeNode ParseType() {
            TypeNode typeNode = parserFactory.CreateParser<TypeNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();

            return typeNode;
        }
        private ExpressionNode ParseExpression() {
            ExpressionNode expressionNode = parserFactory.CreateParser<ExpressionNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();

            return expressionNode;
        }
    }
}
