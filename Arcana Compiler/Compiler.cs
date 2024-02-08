using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler {
    public class Compiler {
        private Module _module;

        public Compiler(Module module) {
            _module = module;
        }

        public void Compile() {
            // A dictionary to cache ASTs for each source file
            Dictionary<string, ProgramNode> astCache = new Dictionary<string, ProgramNode>();
            SymbolTable globalSymbolTable = new SymbolTable();

            Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
            foreach (var filePath in _module.SourceFiles) {
                Console.WriteLine($"Parsing: {filePath}");
                string sourceCode = File.ReadAllText(filePath);

                Lexer lexer = new Lexer(sourceCode);
                Parser parser = new Parser(lexer);
                ProgramNode ast = parser.Parse();

                astCache[filePath] = ast;

                ASTPrinter printer = new ASTPrinter();
                string astString = printer.Print(ast);
                Console.WriteLine(astString);
            }

            Console.WriteLine("~~~~~~~~~~Semantic Analysis~~~~~~~~~~");
            Console.WriteLine("Building global symbol table...");
            foreach (var ast in astCache.Values) {
                SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(ast, globalSymbolTable);
                symbolTableBuilder.Analyze();
            }

            Console.WriteLine("Type checking...");
            foreach (var ast in astCache.Values) {
                TypeChecker typeChecker = new TypeChecker(globalSymbolTable);
                typeChecker.Check(ast);
            }

            // Intermediate Code Generation (e.g., to LLVM IR)
            /*CodeGenerator codeGenerator = new CodeGenerator();
            string intermediateCode = codeGenerator.Generate(ast);

            // Output the result
            Console.WriteLine(intermediateCode);*/
        }
    }
}
