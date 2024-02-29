using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;

namespace Arcana_Compiler.ArcanaParser.Parsers {

    public class ClassParser : BaseParser<ClassDeclarationNode> {
        public ClassParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override ClassDeclarationNode Parse() {
            string? classAccessModifier = TryParseAccessModifier();
            List<ClassModifierNode> classModifiers = ParseClassModifiers();

            Eat(TokenType.CLASS);
            string className = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParentTypeNode> parentTypes = ParseParentTypes();

            Eat(TokenType.OPEN_BRACE);

            List<FieldDeclarationNode> fields = new List<FieldDeclarationNode>();
            List<MethodDeclarationNode> methods = new List<MethodDeclarationNode>();
            List<ClassDeclarationNode> nestedClasses = new List<ClassDeclarationNode>();

            while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                if (CurrentToken.Type == TokenType.CLASS) {
                    nestedClasses.Add(ParseNestedClass());
                } else if (IsMethodDeclaration()) {
                    methods.Add(ParseMethodDeclaration());
                } else {
                    fields.Add(ParseFieldDeclaration());
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            IdentifierName currentNamespace = IdentifierName.DefaultNameSpace;

            var fullClassName = currentNamespace + className;

            return new ClassDeclarationNode(fullClassName, classAccessModifier, classModifiers, parentTypes, fields, methods, nestedClasses);
        }

        private List<ParentTypeNode> ParseParentTypes() {
            List<ParentTypeNode> parentTypes = new List<ParentTypeNode>();
            if (CurrentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                parentTypes.Add(new ParentTypeNode(CurrentToken.Value));
                Eat(TokenType.IDENTIFIER);
                while (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    parentTypes.Add(new ParentTypeNode(CurrentToken.Value));
                    Eat(TokenType.IDENTIFIER);
                }
            }
            return parentTypes;
        }

        private ClassDeclarationNode ParseNestedClass() {
            ClassDeclarationNode classDeclarationNode = parserFactory.CreateParser<ClassDeclarationNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();
            return classDeclarationNode;
        }

        private MethodDeclarationNode ParseMethodDeclaration() {
            MethodDeclarationNode methodDeclarationNode = parserFactory.CreateParser<MethodDeclarationNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();
            return methodDeclarationNode;
        }

        private FieldDeclarationNode ParseFieldDeclaration() {
            FieldDeclarationNode fieldDeclarationNode = parserFactory.CreateParser<FieldDeclarationNode>().Parse();
            CurrentToken = Lexer.GetCurrentToken();
            return fieldDeclarationNode;
        }
    }
}
