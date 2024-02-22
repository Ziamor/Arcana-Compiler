using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Factory;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class ProgramParser : BaseParser<ProgramNode> {
        private readonly ParserFactory _parserFactory;

        public ProgramParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter) {
            _parserFactory = parserFactory;            
        }

        public override ProgramNode Parse() {
            ProgramNode rootNode = new ProgramNode();
            rootNode.Imports = ParseImportDeclarations();

            var defaultNamespaceClasses = new List<ClassDeclarationNode>();
            var defaultNamespaceInterfaces = new List<InterfaceDeclarationNode>();

            while (CurrentToken.Type != TokenType.EOF) {                
                switch (CurrentToken.Type) {
                    case TokenType.NAMESPACE:
                        ASTNode? namespaceParserResult = _parserFactory.CreateParser<NamespaceDeclarationNode>()?.Parse();
                        if (namespaceParserResult is NamespaceDeclarationNode namespaceNode) {
                            rootNode.NamespaceDeclarations.Add(namespaceNode);
                        }
                        break;
                    default:
                        // TODO recover logic
                        throw new UnexpectedTokenException(CurrentToken);
                }
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
    }
}