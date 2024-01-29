using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaLexer {
    public interface ILexer {
        Token GetNextToken();
        List<Token> Tokenize();
    }
}
