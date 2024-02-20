using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Factory; // Import the correct namespace
using System.Collections.Generic;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class NamespaceParser : BaseParser<NamespaceDeclarationNode> {
        private readonly ParserFactory _parserFactory;

        public NamespaceParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter) {
            _parserFactory = parserFactory;
        }

        public override NamespaceDeclarationNode Parse() {
            if (CurrentToken.Type != TokenType.NAMESPACE) {
                Error("Expected namespace declaration.");
            }

            Eat(TokenType.NAMESPACE);
            var namespaceName = ParseIdentifierName();
            Eat(TokenType.OPEN_BRACE);

            var classes = new List<ClassDeclarationNode>();
            var interfaces = new List<InterfaceDeclarationNode>();

            while (CurrentToken.Type != TokenType.CLOSE_BRACE && CurrentToken.Type != TokenType.EOF) {
                switch (CurrentToken.Type) {
                    case TokenType.CLASS:
                        var classParser = _parserFactory.CreateParser<ClassDeclarationNode>();
                        var classNode = classParser.Parse();
                        classes.Add(classNode);
                        break;
                    case TokenType.INTERFACE:
                        var interfaceParser = _parserFactory.CreateParser<InterfaceDeclarationNode>();
                        var interfaceNode = interfaceParser.Parse();
                        interfaces.Add(interfaceNode);
                        break;
                    default:
                        Eat(CurrentToken.Type); // Or perhaps you meant to report an unexpected token error?
                        break;
                }
                CurrentToken = Lexer.GetCurrentToken(); // Sync up current token
            }

            Eat(TokenType.CLOSE_BRACE);

            return new NamespaceDeclarationNode(namespaceName, classes, interfaces);
        }
    }
}
