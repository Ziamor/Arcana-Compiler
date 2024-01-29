using NUnit.Framework;
using System.Collections.Generic;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.Common;
using ArcanaCompiler.Tests.TestUtils;
using Arcana_Compiler.ArcanaParser.Nodes;

namespace ArcanaCompiler.Tests
{
    [TestFixture]
    public class ParserTests {
        [Test]
        public void TestParseSimpleVariableDeclaration() {
            var tokens = new List<Token> {
                new Token(TokenType.IDENTIFIER, "int", 1, 1),
                new Token(TokenType.IDENTIFIER, "count", 1, 5),
                new Token(TokenType.ASSIGN, "=", 1, 11),
                new Token(TokenType.NUMBER, "10", 1, 13),
                new Token(TokenType.EOF, null, 1, 16)
            };

            var mockLexer = new MockLexer(tokens);
            var parser = new Parser(mockLexer);
            var ast = parser.Parse();

            var expectedAst = new ProgramNode {
                /*Statements = new List<ASTNode> {
                    new FieldDeclarationNode("int", "count", new LiteralNode("10"))
                }*/
            };
            Assert.That(expectedAst, Is.EqualTo(ast));
        }

        // Additional tests for other parsing functionalities...
    }
}