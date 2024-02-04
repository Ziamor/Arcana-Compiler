using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Utilities;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using System;

namespace Arcana_Compiler
{
    public class Compiler {
        private string _filePath;

        public Compiler(string filePath) {
            _filePath = filePath;
        }

        public void Compile() {
            // Load source code from file
            string sourceCode = File.ReadAllText(_filePath);

            // Lexical Analysis
            Lexer lexer = new Lexer(sourceCode);

            // For testing only
            /*List<Token> tokens = lexer.Tokenize();
            foreach(Token token in tokens) {
                Console.WriteLine(token);
            }*/

            // Syntax Analysis
            Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
            Parser parser = new Parser(lexer);
            ASTNode ast = parser.Parse();

            ASTPrinter printer = new ASTPrinter();
            string astString = printer.Print(ast);
            Console.WriteLine(astString);

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~");
            PrettyPrinter prettyPrinter = new PrettyPrinter();
            string prettyString = prettyPrinter.Print(ast);
            Console.WriteLine(prettyString);

            // Semantic Analysis
            /*Console.WriteLine("~~~~~~~~~~Semantic Analysis~~~~~~~~~~");
            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(ast);
            semanticAnalyzer.Analyze();

            ScopeVisualizer visualizer = new ScopeVisualizer();
            string scopeString = visualizer.Visualize(ast, semanticAnalyzer.SymbolTable);
            Console.WriteLine(scopeString);
            // Intermediate Code Generation (e.g., to LLVM IR)
            CodeGenerator codeGenerator = new CodeGenerator();
            string intermediateCode = codeGenerator.Generate(ast);

            // Output the result
            Console.WriteLine(intermediateCode);*/
        }
    }
}
