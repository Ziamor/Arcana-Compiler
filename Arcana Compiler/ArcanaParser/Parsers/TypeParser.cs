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

            return new TypeNode(typeName, isNullable);
        }
    }
}
