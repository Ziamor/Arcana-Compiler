using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.Common;

public class Compiler {
    private IModule _module;

    private IParser _parser;
    private ILexer _lexer;

    private ISymbolTableBuilder _symbolTableBuilder;
    private ISymbolTable _symbolTable;

    public Compiler(IModule module, IParser parser, ILexer lexer, ISymbolTable symbolTable, ISymbolTableBuilder symbolTableBuilder) {
        _module = module;

        _parser = parser;
        _lexer = lexer;

        _symbolTable = symbolTable;
        _symbolTableBuilder = symbolTableBuilder;

        AddPrimitiveTypes();
    }

    private void AddPrimitiveTypes() {
        var primitiveTypes = new[] { "int", "bool", "float", "string" };
        foreach (var typeName in primitiveTypes) {
            _symbolTable.AddSymbol(new PrimitiveTypeSymbol(typeName));
        }
    }

    public void Compile() {
        Dictionary<string, ProgramNode> astCache = new Dictionary<string, ProgramNode>();

        Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
        foreach (var filePath in _module.SourceFiles) {
            Console.WriteLine($"Parsing: {filePath}");
            Console.WriteLine($"Source:");
            string sourceCode = File.ReadAllText(filePath);
            Console.WriteLine(sourceCode);
            try {
                _lexer.Initialize(sourceCode);
                ErrorReporter reporter;
                ProgramNode ast = _parser.Parse(_lexer, out reporter);

                astCache[filePath] = ast;

                Console.WriteLine("\nAST:");
                ASTPrinter printer = new ASTPrinter();
                Console.WriteLine(printer.Print(ast));

                if (reporter.HasErrors) {
                    Console.WriteLine("\nErrors encountered during parsing:");
                    foreach (var error in reporter.Errors) {
                        Console.WriteLine($"{filePath}: {error.Severity} {error.Message} at line {error.LineNumber}, position {error.Position}");
                    }
                }

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

        Console.WriteLine("\n~~~~~~~~~~Type Checking~~~~~~~~~~");
        TypeChecker typeChecker = new TypeChecker(_symbolTable);
        foreach (var ast in astCache.Values) {
            ast.Accept(typeChecker);
        }

        Console.WriteLine("Finished");
    }

    private void BuildSymbolTable(ProgramNode ast) {
        _symbolTableBuilder.BuildSymbolTable(ast, _symbolTable);
    }
}