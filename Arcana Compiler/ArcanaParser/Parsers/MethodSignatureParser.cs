using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaParser.Parsers {

    public class MethodSignatureParser : BaseParser<MethodSignatureNode> {
        public MethodSignatureParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override MethodSignatureNode Parse() {
            string? accessModifier = TryParseAccessModifier();
            Eat(TokenType.FUNC);
            string methodName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            List<ParameterNode> parameters = ParseParameters();

            List<TypeNode> returnTypes = new List<TypeNode>();
            if (CurrentToken.Type == TokenType.COLON) {
                Eat(TokenType.COLON);
                returnTypes.Add(ParseType());
                while (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                    if (CurrentToken.Type == TokenType.FUNC || IsAccessModifier(CurrentToken.Type)) {
                        break;
                    }

                    returnTypes.Add(ParseType());
                }
            }

            return new MethodSignatureNode(methodName, parameters, returnTypes);
        }

        private List<ParameterNode> ParseParameters() {
            Eat(TokenType.OPEN_PARENTHESIS);
            List<ParameterNode> parameters = new List<ParameterNode>();
            while (CurrentToken.Type != TokenType.CLOSE_PARENTHESIS) {
                parameters.Add(ParseParameter());
                if (CurrentToken.Type == TokenType.COMMA) {
                    Eat(TokenType.COMMA);
                }
            }
            Eat(TokenType.CLOSE_PARENTHESIS);
            return parameters;
        }

        private ParameterNode ParseParameter() {
            TypeNode parameterType = ParseType();

            string parameterName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            return new ParameterNode(parameterType, parameterName);
        }

        private TypeNode ParseType() {
            return ParseNode<TypeNode>();
        }
    }
}
