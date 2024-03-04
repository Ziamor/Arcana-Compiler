using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class StatementParser : BaseParser<StatementNode> {

        public StatementParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
               : base(lexer, errorReporter, parserFactory) {
        }

        public override StatementNode Parse() {
            if (CurrentToken.Type == TokenType.IDENTIFIER) {
                // Peek next token to decide between declaration, assignment, or method call
                Token nextToken = PeekNextToken();
                switch (nextToken.Type) {
                    case TokenType.IDENTIFIER:
                    case TokenType.OPEN_BRACKET:
                        return ParseVariableDeclaration();
                    /*case TokenType.DOT: {
                            ExpressionNode expression = ParseIdentifierOrMethodCall();
                            return new ExpressionStatementNode(expression);
                        }*/
                    case TokenType.ASSIGN:
                        return ParseVariableAssignment();
                    /*case TokenType.OPEN_PARENTHESIS: {
                            ExpressionNode expression = ParseMethodCall(ParseIdentifierName());
                            return new ExpressionStatementNode(expression);
                        }*/
                    default:
                        ReportError($"Unexpected token '{nextToken.Value}' encountered.", nextToken.LineNumber, nextToken.Position, ErrorSeverity.Error);
                        Recover(); // Attempt to recover
                        return new ErrorStatementNode();
                }
            } else {
                switch (CurrentToken.Type) {
                    case TokenType.IF:
                        return ParseIfStatement();
                    case TokenType.THIS:
                        return ParseThisAssignment();
                    case TokenType.FOR:
                        return ParseForLoop();
                    case TokenType.RETURN:
                        return ParseReturnStatement();
                    default:
                        ReportError($"Unexpected token '{CurrentToken.Value}' encountered.", CurrentToken.LineNumber, CurrentToken.Position, ErrorSeverity.Error);
                        Recover(); // Attempt to recover
                        return new ErrorStatementNode();
                }
            }
        }

        private ExpressionNode ParseExpression() {
            return ParseNode<ExpressionNode>();
        }
        private StatementNode ParseStatement() {
            return ParseNode<StatementNode>();
        }

        private StatementNode ParseThisAssignment() {
            Eat(TokenType.THIS);
            Eat(TokenType.DOT);
            IdentifierName identifierName = ParseIdentifierName();

            // Expect an assignment operator next
            if (CurrentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                ASTNode valueExpression = ParseExpression();
                return new ThisAssignmentNode(identifierName, valueExpression);
            } else {
                throw new UnexpectedTokenException(CurrentToken);
            }
        }
        private ReturnStatementNode ParseReturnStatement() {
            Eat(TokenType.RETURN);
            List<ExpressionNode> returnExpressions =
            [
                ParseExpression(), // Parse the first expression
            ];
            while (CurrentToken.Type == TokenType.COMMA) {
                Eat(TokenType.COMMA);
                returnExpressions.Add(ParseExpression()); // Parse subsequent expressions
            }

            return new ReturnStatementNode(returnExpressions);
        }

        private StatementNode ParseVariableDeclaration() {
            List<(TypeNode Type, string Name)> tempDeclarations = new List<(TypeNode, string)>();
            bool expectComma;

            // First, parse the types and names of the variables to be declared
            do {
                TypeNode variableType = ParseType();
                string variableName = CurrentToken.Value;
                Eat(TokenType.IDENTIFIER);
                tempDeclarations.Add((variableType, variableName));

                expectComma = CurrentToken.Type == TokenType.COMMA;
                if (expectComma) {
                    Eat(TokenType.COMMA);
                }
            } while (expectComma);

            ExpressionNode? initialValue = null;
            if (CurrentToken.Type == TokenType.ASSIGN) {
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
            string variableName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);
            Eat(TokenType.ASSIGN);
            ASTNode expression = ParseExpression();
            return new VariableAssignmentNode(variableName, expression);
        }

        private StatementNode ParseForLoop() {
            Eat(TokenType.FOR);
            Eat(TokenType.OPEN_PARENTHESIS);

            Token nextToken = PeekNextToken(2);
            if (nextToken.Type == TokenType.IN) {
                return ParseForEachLoop();
            } else {
                return ParseTraditionalForLoop();
            }
        }

        private ForEachLoopNode ParseForEachLoop() {
            TypeNode type = ParseType();
            string variableName = CurrentToken.Value;
            VariableDeclarationNode variableDeclaration = new VariableDeclarationNode(type, variableName);
            Eat(TokenType.IDENTIFIER);
            Eat(TokenType.IN);

            ExpressionNode collection = ParseExpression();

            Eat(TokenType.CLOSE_PARENTHESIS);

            Eat(TokenType.OPEN_BRACE);
            List<StatementNode> body = ParseBlockOrStatement();
            Eat(TokenType.CLOSE_BRACE);

            return new ForEachLoopNode(variableDeclaration, collection, body);
        }

        private ForLoopNode ParseTraditionalForLoop() {
            StatementNode initialization = ParseStatement();
            Eat(TokenType.SEMICOLON);

            ExpressionNode condition = ParseExpression();
            Eat(TokenType.SEMICOLON);

            ExpressionNode increment = ParseExpression();

            Eat(TokenType.CLOSE_PARENTHESIS);

            Eat(TokenType.OPEN_BRACE);
            List<StatementNode> body = ParseBlockOrStatement();
            Eat(TokenType.CLOSE_BRACE);

            return new ForLoopNode(initialization, condition, increment, body);
        }

        private List<StatementNode> ParseBlockOrStatement() {
            if (CurrentToken.Type == TokenType.OPEN_BRACE) {
                Eat(TokenType.OPEN_BRACE);
                List<StatementNode> statements = new List<StatementNode>();
                while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                    statements.Add(ParseStatement());
                }
                Eat(TokenType.CLOSE_BRACE);
                return statements;
            } else {
                // If not a block, parse a single statement
                return new List<StatementNode> { ParseStatement() };
            }
        }

        private IfStatementNode ParseIfStatement() {
            List<(ExpressionNode Condition, List<StatementNode> Statements)> conditionsAndStatements = new List<(ExpressionNode Condition, List<StatementNode> Statements)>();
            List<StatementNode>? elseStatements = null;

            // Parse the initial 'if' condition and its block
            Eat(TokenType.IF);
            Eat(TokenType.OPEN_PARENTHESIS);
            ExpressionNode initialCondition = ParseExpression();
            Eat(TokenType.CLOSE_PARENTHESIS);
            List<StatementNode> initialThenStatements = ParseBlockOrStatement();

            conditionsAndStatements.Add((initialCondition, initialThenStatements));

            // Parse any 'else if' branches
            while (CurrentToken.Type == TokenType.ELSE) {
                Eat(TokenType.ELSE);
                if (CurrentToken.Type == TokenType.IF) {
                    Eat(TokenType.IF);
                    Eat(TokenType.OPEN_PARENTHESIS);
                    ExpressionNode elseIfCondition = ParseExpression();
                    Eat(TokenType.CLOSE_PARENTHESIS);
                    List<StatementNode> elseIfStatements = ParseBlockOrStatement();

                    conditionsAndStatements.Add((elseIfCondition, elseIfStatements));
                } else {
                    elseStatements = ParseBlockOrStatement();
                    break; // After 'else', no more 'else if' or 'else' should be parsed
                }
            }

            return new IfStatementNode(conditionsAndStatements, elseStatements);
        }

        private TypeNode ParseType() {
            return ParseNode<TypeNode>();
        }
    }
}
