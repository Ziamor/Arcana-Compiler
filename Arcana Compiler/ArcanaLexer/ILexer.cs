using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaLexer {
    public interface ILexer {
        void Initialize(string input);
        Token GetNextToken();
        List<Token> Tokenize();
        Token PeekToken(int depth);
    }
}
