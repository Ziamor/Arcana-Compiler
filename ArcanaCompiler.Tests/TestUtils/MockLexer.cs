using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.Common;

namespace ArcanaCompiler.Tests.TestUtils
{
    public class MockLexer : ILexer
    {
        private readonly Queue<Token> _tokens;

        public MockLexer(IEnumerable<Token> tokens)
        {
            _tokens = new Queue<Token>(tokens);
        }

        public Token GetNextToken()
        {
            if (_tokens.Count > 0)
            {
                return _tokens.Dequeue();
            }
            return new Token(TokenType.EOF, null, -1, -1); // Return EOF if no tokens left
        }

        public List<Token> Tokenize()
        {
            return _tokens.ToList();
        }
    }
}
