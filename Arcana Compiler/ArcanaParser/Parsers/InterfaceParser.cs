using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class InterfaceParser : BaseParser<InterfaceDeclarationNode> {
        public InterfaceParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override InterfaceDeclarationNode Parse() {
            string? interfaceAccessModifier = TryParseAccessModifier();
            List<ClassModifierNode> interfaceModifiers = ParseClassModifiers();

            Eat(TokenType.INTERFACE);
            string interfaceName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParentTypeNode> parentInterfaces = ParseParentInterfaces();

            Eat(TokenType.OPEN_BRACE);

            List<MethodSignatureNode> methods = new List<MethodSignatureNode>();

            while (CurrentToken.Type != TokenType.CLOSE_BRACE) {
                if (IsMethodDeclaration()) {
                    methods.Add(ParseMethodSignature());
                }
            }

            Eat(TokenType.CLOSE_BRACE);

            IdentifierName currentNamespace = IdentifierName.DefaultNameSpace;
            IdentifierName fullInterfaceName = currentNamespace + interfaceName;

            return new InterfaceDeclarationNode(fullInterfaceName, interfaceAccessModifier, interfaceModifiers, methods);
        }

        private List<ParentTypeNode> ParseParentInterfaces() {
            return new List<ParentTypeNode>();
        }

        private MethodSignatureNode ParseMethodSignature() {
            return ParseNode<MethodSignatureNode>();
        }
    }
}
