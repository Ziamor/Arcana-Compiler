using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Parsers {
    public class FieldParser : BaseParser<FieldDeclarationNode> {
        public FieldParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override FieldDeclarationNode Parse() {
            string? accessModifier = TryParseAccessModifier();
            List<FieldModifierNode> fieldModifiers = ParseFieldModifiers();
            TypeNode fieldType = ParseType();
            string fieldName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            ExpressionNode? initialValue = null;
            if (CurrentToken.Type == TokenType.ASSIGN) {
                Eat(TokenType.ASSIGN);
                initialValue = ParseExpression();
            }

            return new FieldDeclarationNode(fieldType, fieldName, accessModifier, fieldModifiers, initialValue);
        }

        private List<FieldModifierNode> ParseFieldModifiers() {
            List<FieldModifierNode> modifiers = new List<FieldModifierNode>();
            while (IsFieldModifier(CurrentToken.Type)) {
                modifiers.Add(new FieldModifierNode(CurrentToken.Value));
                Eat(CurrentToken.Type);
            }
            return modifiers;
        }

        private TypeNode ParseType() {
            return ParseNode<TypeNode>();
        }

        private ExpressionNode ParseExpression() {
            return ParseNode<ExpressionNode>();
        }
    }
}
