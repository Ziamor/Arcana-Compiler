using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaLexer {
    public interface ILexer {
        void Initialize(string input);
        Token GetNextToken();
        Token GetCurrentToken();
        List<Token> Tokenize();
        Token PeekToken(int depth);
    }
}
