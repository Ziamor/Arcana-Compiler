using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcanaCompiler.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void TestTokenizeSimpleVariableDeclaration()
        {
            var lexer = new Lexer("int count = 10");
            var expectedTokens = new List<Token> {
                new Token(TokenType.IDENTIFIER, "int", 1, 1),
                new Token(TokenType.IDENTIFIER, "count", 1, 4),
                new Token(TokenType.ASSIGN, "=", 1, 10),
                new Token(TokenType.NUMBER, "10", 1, 12),
                new Token(TokenType.EOF, null, 1, 15)
            };

            var actualTokens = lexer.Tokenize();
            Assert.That(actualTokens, Is.EqualTo(expectedTokens));
        }
    }
}
