using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Factory;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class ProgramParser : BaseParser<ProgramNode> {

        public ProgramParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override ProgramNode Parse() {
            ProgramNode rootNode = new ProgramNode();
            rootNode.Imports = ParseImportDeclarations();

            var defaultNamespaceClasses = new List<ClassDeclarationNode>();
            var defaultNamespaceInterfaces = new List<InterfaceDeclarationNode>();

            while (CurrentToken.Type != TokenType.EOF) {
                switch (CurrentToken.Type) {
                    case TokenType.NAMESPACE:
                        rootNode.NamespaceDeclarations.Add(ParseNamespace());
                        break;
                    default:
                        if (!IsClassOrInterfaceAhead()) {
                            throw new UnexpectedTokenException(CurrentToken);
                        }
                        ParseClassOrInterfaceInDefaultNamespace(defaultNamespaceClasses, defaultNamespaceInterfaces);
                        break;

                }
                CurrentToken = Lexer.GetCurrentToken();
            }

            if (defaultNamespaceClasses.Any() || defaultNamespaceInterfaces.Any()) {
                rootNode.NamespaceDeclarations.Add(new NamespaceDeclarationNode(new IdentifierName("default"), defaultNamespaceClasses, defaultNamespaceInterfaces));
            }

            return rootNode;
        }

        private List<ImportDeclarationNode> ParseImportDeclarations() {
            var imports = new List<ImportDeclarationNode>();
            while (CurrentToken.Type == TokenType.IMPORT) {
                Eat(TokenType.IMPORT);
                var qualifiedName = ParseIdentifierName();
                imports.Add(new ImportDeclarationNode(qualifiedName));
            }
            return imports;
        }

        private void ParseClassOrInterfaceInDefaultNamespace(List<ClassDeclarationNode> defaultNamespaceClasses, List<InterfaceDeclarationNode> defaultNamespaceInterfaces) {
            var nextToken = PeekNextRelevantToken();
            if (nextToken.Type == TokenType.CLASS) {
                var classParser = parserFactory.CreateParser<ClassDeclarationNode>();
                defaultNamespaceClasses.Add(classParser.Parse());
            } else if (nextToken.Type == TokenType.INTERFACE) {
                var interfaceParser = parserFactory.CreateParser<InterfaceDeclarationNode>();
                defaultNamespaceInterfaces.Add(interfaceParser.Parse());
            } else {
                throw new UnexpectedTokenException(CurrentToken);
            }
        }

        private NamespaceDeclarationNode ParseNamespace() {
            if (CurrentToken.Type != TokenType.NAMESPACE) {
                Error("Expected 'namespace' declaration.");
            }

            Eat(TokenType.NAMESPACE);
            var namespaceName = ParseIdentifierName();
            Eat(TokenType.OPEN_BRACE);


            List<ClassDeclarationNode> classes;
            List<InterfaceDeclarationNode> interfaces;

            ParseClassOrInterfaces(out classes, out interfaces);
            Eat(TokenType.CLOSE_BRACE);
            return new NamespaceDeclarationNode(namespaceName, classes, interfaces);
        }

        private void ParseClassOrInterfaces(out List<ClassDeclarationNode> classes, out List<InterfaceDeclarationNode> interfaces) {
            classes = new List<ClassDeclarationNode>();
            interfaces = new List<InterfaceDeclarationNode>();

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
        }

        private bool IsClassOrInterfaceAhead() {
            if (CurrentToken.Type == TokenType.CLASS || CurrentToken.Type == TokenType.INTERFACE) {
                return true;
            }

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