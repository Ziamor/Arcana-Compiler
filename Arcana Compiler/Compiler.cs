using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.ArcanaSemanticAnalyzer;

public class Compiler {
    private Module _module;
    private SymbolTable _symbolTable;

    public Compiler(Module module) {
        _module = module;
        _symbolTable = new SymbolTable();
    }

    public void Compile() {
        Dictionary<string, ProgramNode> astCache = new Dictionary<string, ProgramNode>();

        Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
        foreach (var filePath in _module.SourceFiles) {
            Console.WriteLine($"Parsing: {filePath}");
            Console.WriteLine($"Source:");
            string sourceCode = File.ReadAllText(filePath);
            Console.WriteLine(sourceCode);
            Lexer lexer = new Lexer(sourceCode);
            Parser parser = new Parser(lexer);
            try {
                ProgramNode ast = parser.Parse();

                astCache[filePath] = ast;

                Console.WriteLine("\nAST:");
                ASTPrinter printer = new ASTPrinter();
                Console.WriteLine(printer.Print(ast));

                // Perform Semantic Analysis
                Console.WriteLine("\n~~~~~~~~~~Semantic Analysis~~~~~~~~~~");
                BuildSymbolTable(ast);

            } catch (ParsingException ex) {
                string errorMessage = $"{filePath}: {ex.Message}";
                Console.WriteLine(errorMessage);
                throw;
            }
        }

        Console.WriteLine("\n~~~~~~~~~~Symbol Table~~~~~~~~~~");
        ScopePrinter scopePrinter = new ScopePrinter(_symbolTable);
        scopePrinter.Print();

        Console.WriteLine("Finished");
    }

    private void BuildSymbolTable(ProgramNode ast) {
        SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(_symbolTable);
        ast.Accept(symbolTableBuilder);
    }
}