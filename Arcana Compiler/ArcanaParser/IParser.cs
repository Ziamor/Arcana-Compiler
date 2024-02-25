using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser {
    public interface IParser<T> where T : ASTNode {
        T Parse();
    }
}