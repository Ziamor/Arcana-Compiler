using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.Common;

namespace ArcanaCompiler.Tests {
    [TestFixture]
    public class LexerTests {
        [Test]
        public void TestTokenizeSimpleVariableDeclaration() {
            var lexer = new Lexer("int count = 10");
            var expectedTokens = new List<Token> {
                new Token(TokenType.IDENTIFIER, "int", 1, 1, "int count = 10"),
                new Token(TokenType.IDENTIFIER, "count", 1, 4, "int count = 10"),
                new Token(TokenType.ASSIGN, "=", 1, 10, "int count = 10"),
                new Token(TokenType.NUMBER, "10", 1, 12, "int count = 10"),
                new Token(TokenType.EOF, null, 1, 15, "int count = 10")
            };

            var actualTokens = lexer.Tokenize();
            Assert.That(actualTokens, Is.EqualTo(expectedTokens));
        }

        [Test]
        public void TestTokenizeImportStatements() {
            var lexer = new Lexer("import single.level\nimport multi.level.path");
            var expectedTokens = new List<Token> {
                new Token(TokenType.IMPORT, "import", 1, 1, "import single.level"),
                new Token(TokenType.IDENTIFIER, "single", 1, 7, "import single.level"),
                new Token(TokenType.DOT, ".", 1, 14, "import single.level"),
                new Token(TokenType.IDENTIFIER, "level", 1, 15, "import single.level"),
                new Token(TokenType.IMPORT, "import", 2, 1, "import multi.level.path"),
                new Token(TokenType.IDENTIFIER, "multi", 2, 7, "import multi.level.path"),
                new Token(TokenType.DOT, ".", 2, 13, "import multi.level.path"),
                new Token(TokenType.IDENTIFIER, "level", 2, 14, "import multi.level.path"),
                new Token(TokenType.DOT, ".", 2, 19, "import multi.level.path"),
                new Token(TokenType.IDENTIFIER, "path", 2, 20, "import multi.level.path"),
                new Token(TokenType.EOF, null, 2, 24, "import multi.level.path")
            };

            var actualTokens = lexer.Tokenize();
            Assert.That(actualTokens, Is.EqualTo(expectedTokens));
        }
    }
}
