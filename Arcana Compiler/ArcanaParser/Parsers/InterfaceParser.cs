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
    public class InterfaceParser : BaseParser<InterfaceDeclarationNode> {
        public InterfaceParser(ILexer lexer, ErrorReporter errorReporter, ParserFactory parserFactory)
            : base(lexer, errorReporter, parserFactory) {
        }

        public override InterfaceDeclarationNode Parse() {
            throw new NotImplementedException();
        }
    }
}
