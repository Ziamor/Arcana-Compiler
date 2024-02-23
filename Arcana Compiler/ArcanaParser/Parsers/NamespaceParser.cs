using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Factory; // Ensure correct namespace import
using System.Collections.Generic;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class NamespaceParser : BaseParser<NamespaceDeclarationNode> {
        public NamespaceParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override NamespaceDeclarationNode Parse() {
            if (CurrentToken.Type != TokenType.NAMESPACE) {
                Error("Expected 'namespace' declaration.");
            }

            Eat(TokenType.NAMESPACE);
            var namespaceName = ParseIdentifierName();
            Eat(TokenType.OPEN_BRACE);

            var classes = new List<ClassDeclarationNode>();
            var interfaces = new List<InterfaceDeclarationNode>();

            while (CurrentToken.Type != TokenType.CLOSE_BRACE && CurrentToken.Type != TokenType.EOF) {
                bool isClassOrInterfaceAhead = IsClassOrInterfaceAhead();

                if (isClassOrInterfaceAhead) {
                    if (PeekNextRelevantToken().Type == TokenType.CLASS) {
                        var classParser = parserFactory.CreateParser<ClassDeclarationNode>();
                        classes.Add(classParser.Parse());
                    } else if (PeekNextRelevantToken().Type == TokenType.INTERFACE) {
                        var interfaceParser = parserFactory.CreateParser<InterfaceDeclarationNode>();
                        interfaces.Add(interfaceParser.Parse());
                    }
                } else {
                    Error("Expected 'class' or 'interface' declaration.");
                }
                CurrentToken = Lexer.GetCurrentToken();
            }

            Eat(TokenType.CLOSE_BRACE);
            return new NamespaceDeclarationNode(namespaceName, classes, interfaces);
        }

        private bool IsClassOrInterfaceAhead() {
            // Skip through any possible access or class modifiers
            for (int i = 1; i <= 3; i++) {
                var token = PeekNextToken(i);
                if (token.Type == TokenType.CLASS || token.Type == TokenType.INTERFACE) {
                    return true;
                }
            }
            return false;
        }

        private Token PeekNextRelevantToken() {
            for (int i = 1; i <= 3; i++) {
                var token = PeekNextToken(i);
                if (token.Type == TokenType.CLASS || token.Type == TokenType.INTERFACE) {
                    return token;
                }
            }
            return CurrentToken;
        }
    }
}
