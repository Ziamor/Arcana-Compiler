namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class ScopePrinter {
        private readonly ISymbolTable _symbolTable;

        public ScopePrinter(ISymbolTable symbolTable) {
            _symbolTable = symbolTable;
        }

        public void Print() {
            PrintScope(_symbolTable.GlobalScope, 0);
        }

        private void PrintScope(Scope scope, int level) {
            // Iterate over each list of symbols in the scope
            foreach (var symbolList in scope.Symbols.Values) {
                foreach (var symbol in symbolList) {
                    // Print the symbol information
                    Console.WriteLine($"{new string(' ', level * 2)}{symbol}");

                    // If the symbol owns a scope, print its scope recursively
                    if (symbol.OwnedScope is not null) {
                        PrintScope(symbol.OwnedScope, level + 1);
                    }
                }
            }
        }
    }

}
