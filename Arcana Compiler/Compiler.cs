using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.ArcanaModule;

public class Compiler {
    private Module _module;
    // Map each source file to its own symbol table
    private Dictionary<string, SymbolTable> _fileSymbolTables = new Dictionary<string, SymbolTable>();

    public Compiler(Module module) {
        _module = module;
    }

    public void Compile() {
        Dictionary<string, ProgramNode> astCache = new Dictionary<string, ProgramNode>();

        Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
        foreach (var filePath in _module.SourceFiles) {
            Console.WriteLine($"Parsing: {filePath}");
            string sourceCode = File.ReadAllText(filePath);

            Lexer lexer = new Lexer(sourceCode);
            Parser parser = new Parser(lexer);
            ProgramNode ast = parser.Parse();

            astCache[filePath] = ast;

            ASTPrinter printer = new ASTPrinter();
            Console.WriteLine(printer.Print(ast));

            // Create a symbol table for each file
            SymbolTable fileSymbolTable = new SymbolTable();
            _fileSymbolTables[filePath] = fileSymbolTable;

            // Perform semantic analysis per file
            SymbolTableBuilder symbolTableBuilder = new SymbolTableBuilder(ast, fileSymbolTable);
            symbolTableBuilder.Analyze();
        }

        Console.WriteLine("~~~~~~~~~~Semantic Analysis~~~~~~~~~~");
        // Now, each file has its own symbol table, and type checking can proceed with this context.
        foreach (var entry in astCache) {
            string filePath = entry.Key;
            ProgramNode ast = entry.Value;
            SymbolTable fileSymbolTable = _fileSymbolTables[filePath];

            // Pass the specific symbol table for the file to the type checker
            TypeChecker typeChecker = new TypeChecker(fileSymbolTable);
            typeChecker.Check(ast);
        }
    }
}