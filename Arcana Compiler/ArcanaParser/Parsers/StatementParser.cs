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
                    /*case TokenType.IF:
                        return ParseIfStatement();*/
                    case TokenType.THIS:
                        return ParseThisAssignment();
                    case TokenType.FOR:
                        return ParseForLoop();
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

            ASTNode collection = ParseExpression();

            Eat(TokenType.CLOSE_PARENTHESIS);

            Eat(TokenType.OPEN_BRACE);
            List<ASTNode> body = ParseBlockOrStatement();
            Eat(TokenType.CLOSE_BRACE);

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

        private List<ASTNode> ParseBlockOrStatement() {
            if (CurrentToken.Type == TokenType.OPEN_BRACE) {
                Eat(TokenType.OPEN_BRACE);
                List<ASTNode> statements = new List<ASTNode>();
                while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                    statements.Add(ParseStatement());
                }
                Eat(TokenType.CLOSE_BRACE);
                return statements;
            } else {
                // If not a block, parse a single statement
                return new List<ASTNode> { ParseStatement() };
            }
        }
        /* private IfStatementNode ParseIfStatement() {
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
             while (CurrentToken.Type == TokenType.ELSE) {
                 Eat(TokenType.ELSE);
                 if (CurrentToken.Type == TokenType.IF) {
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
             if (CurrentToken.Type == TokenType.OPEN_BRACE) {
                 Eat(TokenType.OPEN_BRACE);
                 List<ASTNode> statements = new List<ASTNode>();
                 while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                     statements.Add(ParseStatement());
                 }
                 Eat(TokenType.CLOSE_BRACE);
                 return statements;
             } else {
                 // If not a block, parse a single statement
                 return new List<ASTNode> { ParseStatement() };
             }
         }*/

        private TypeNode ParseType() {
            return ParseNode<TypeNode>();
        }
    }
}
