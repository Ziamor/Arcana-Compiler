using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class ScopePrinter {
        private readonly SymbolTable _symbolTable;

        public ScopePrinter(SymbolTable symbolTable) {
            _symbolTable = symbolTable;
        }

        public void Print() {
            PrintScope(_symbolTable._globalScope, 0);
        }

        private void PrintScope(Scope scope, int level) {
            foreach (var symbol in scope.Symbols.Values) {
                Console.WriteLine($"{new string(' ', level * 2)}{symbol}");

                // If the symbol owns a scope, print its scope recursively
                if (symbol.OwnedScope != null) {
                    PrintScope(symbol.OwnedScope, level + 1);
                }
            }
        }
    }

}
