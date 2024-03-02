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

    public class TypeParser : BaseParser<TypeNode> {
        public TypeParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override TypeNode Parse() {
            string typeName = CurrentToken.Value;
            Eat(TokenType.IDENTIFIER);

            bool isNullable = false;
            if (CurrentToken.Type == TokenType.QUESTION_MARK) {
                Eat(TokenType.QUESTION_MARK);
                isNullable = true;
            }

            bool isArray = false;
            int arrayDimensions = 0;
            while (CurrentToken.Type == TokenType.OPEN_BRACKET) {
                Eat(TokenType.OPEN_BRACKET);
                if (CurrentToken.Type != TokenType.CLOSE_BRACKET) {
                    ReportError("Missing close bracket ']'", CurrentToken, ErrorReporter.ErrorSeverity.Error);
                }
                Eat(TokenType.CLOSE_BRACKET);
                isArray = true;
                arrayDimensions++;
            }

            return new TypeNode(typeName, isNullable, isArray, arrayDimensions);
        }
    }
}
