using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaParser;
using Arcana_Compiler.ArcanaModule;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.ArcanaSemanticAnalyzer;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Factory;
using Arcana_Compiler.ArcanaParser.Parsers;

public class Compiler {
    private IModule _module;

    private IParserOld _parser;
    private ILexer _lexer;

    private ISymbolTableBuilder _symbolTableBuilder;
    private ISymbolTable _symbolTable;

    private ParserFactory _parserFactory;

    public Compiler(IModule module, IParserOld parser, ILexer lexer, ISymbolTable symbolTable, ISymbolTableBuilder symbolTableBuilder) {
        _module = module;

        _parser = parser;
        _lexer = lexer;

        _symbolTable = symbolTable;
        _symbolTableBuilder = symbolTableBuilder;
        AddPrimitiveTypes();

        _parserFactory = new ParserFactory(lexer, new ErrorReporter());
    }

    private void AddPrimitiveTypes() {
        var primitiveTypes = new[] { "int", "bool", "float", "string" };
        foreach (var typeName in primitiveTypes) {
            _symbolTable.AddSymbol(new PrimitiveTypeSymbol(typeName));
        }
    }

    public void Compile() {
        Console.WriteLine("~~~~~~~~~~Syntax Analysis~~~~~~~~~~");
        var astCache = ParseSourceFiles();
        BuildSymbolTables(astCache);
        PerformTypeChecking(astCache);
        Console.WriteLine("Finished");
    }

    private Dictionary<string, ProgramNode> ParseSourceFiles() {
        var astCache = new Dictionary<string, ProgramNode>();

        foreach (var filePath in _module.SourceFiles) {
            Console.WriteLine($"Parsing: {filePath}");
            Console.WriteLine($"Source:");
            string sourceCode = File.ReadAllText(filePath);
            Console.WriteLine(sourceCode);


            ErrorReporter? reporter = null;
            try {
                _lexer.Initialize(sourceCode);

                ProgramParser programParser = (ProgramParser)_parserFactory.CreateParser<ProgramNode>();
                if (programParser != null) {
                    ProgramNode ast = (programParser.Parse() as ProgramNode) ?? throw new Exception("Parsing did not result in a ProgramNode.");
                    astCache[filePath] = ast;

                    PrintAST(ast);
                    ReportParsingErrors(filePath, programParser.errorReporter);
                }

            } catch (ParsingException ex) {
                if (reporter != null) {
                    ReportParsingErrors(filePath, reporter);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{filePath}: {ex.Message}");
                Console.ResetColor();
            }
        }

        return astCache;
    }

    private void PrintAST(ProgramNode ast) {
        Console.WriteLine("\nAST:");
        ASTPrinter printer = new ASTPrinter();
        Console.WriteLine(printer.Print(ast));
    }

    private void ReportParsingErrors(string filePath, ErrorReporter reporter) {
        if (reporter.HasErrors) {
            Console.WriteLine("\nErrors encountered during parsing:");
            Console.ForegroundColor = ConsoleColor.Red;

            foreach (var error in reporter.Errors) {
                Console.WriteLine($"{filePath}: {error.Severity} {error.Message} at line {error.LineNumber}, position {error.Position}");
            }

            Console.ResetColor();
        }
    }


    private void BuildSymbolTables(Dictionary<string, ProgramNode> astCache) {
        Console.WriteLine("\n~~~~~~~~~~Semantic Analysis~~~~~~~~~~");
        foreach (var ast in astCache.Values) {
            BuildSymbolTable(ast);
        }
        PrintSymbolTable();
    }

    private void BuildSymbolTable(ProgramNode ast) {
        _symbolTableBuilder.BuildSymbolTable(ast, _symbolTable);
    }

    private void PrintSymbolTable() {
        Console.WriteLine("\n~~~~~~~~~~Symbol Table~~~~~~~~~~");
        ScopePrinter scopePrinter = new ScopePrinter(_symbolTable);
        scopePrinter.Print();
    }

    private void PerformTypeChecking(Dictionary<string, ProgramNode> astCache) {
        Console.WriteLine("\n~~~~~~~~~~Type Checking~~~~~~~~~~");
        TypeChecker typeChecker = new TypeChecker(_symbolTable);
        foreach (var ast in astCache.Values) {
            ast.Accept(typeChecker);
        }
    }
}