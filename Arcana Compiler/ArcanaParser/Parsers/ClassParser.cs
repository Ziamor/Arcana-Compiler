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
                string? accessModifier = TryParseAccessModifier();

                if (CurrentToken.Type == TokenType.CLASS) {
                    var nestedClassParser = parserFactory.CreateParser<ClassDeclarationNode>();
                    nestedClasses.Add(nestedClassParser.Parse());
                } else if (IsMethodDeclaration()) {
                    methods.Add(ParseMethodDeclaration(accessModifier));
                } else {
                    fields.Add(ParseFieldDeclaration(accessModifier));
                }
                CurrentToken = Lexer.GetCurrentToken();
            }

            Eat(TokenType.CLOSE_BRACE);

            IdentifierName currentNamespace = IdentifierName.DefaultNameSpace;

            var fullClassName = currentNamespace + className;

            return new ClassDeclarationNode(fullClassName, classAccessModifier, classModifiers, parentTypes, fields, methods, nestedClasses);
        }

        private List<ParentTypeNode> ParseParentTypes() {
            return new List<ParentTypeNode>();
        }

        private MethodDeclarationNode ParseMethodDeclaration(string? accessModifier) {
            return null;
        }

        private FieldDeclarationNode ParseFieldDeclaration(string? accessModifier) {
            IParser<FieldDeclarationNode> fieldParser = parserFactory.CreateParser<FieldDeclarationNode>();
            return fieldParser.Parse();
        }
    }    
}
