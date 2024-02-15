using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class SymbolTable {
        internal Scope _globalScope = new Scope();
        private Stack<Scope> _scopeStack = new Stack<Scope>();

        public SymbolTable() {
            // Initialize with the global scope at the base of the stack
            _scopeStack.Push(_globalScope);
        }

        public void EnterScope(Symbol symbol) {
            // If the symbol can own a scope, we either use its existing scope or create a new one.
            if (symbol.OwnedScope == null) {
                symbol.OwnedScope = new Scope();
            }
            _scopeStack.Push(symbol.OwnedScope);
        }

        public void ExitScope() {
            if (_scopeStack.Count > 1) { // Ensure we don't pop the global scope
                _scopeStack.Pop();
            } else {
                throw new InvalidOperationException("Attempting to exit the global scope.");
            }
        }

        public void AddSymbol(Symbol symbol) {
            if (_scopeStack.TryPeek(out Scope? currentScope)) {
                currentScope.AddSymbol(symbol);
            } else {
                throw new InvalidOperationException("There's no active scope to add a symbol to.");
            }
        }

        public Symbol? FindSymbol(string name, bool searchAllScopes = true) {
            foreach (var scope in _scopeStack) {
                if (scope.Symbols.TryGetValue(name, out var symbol)) {
                    return symbol;
                }
                if (!searchAllScopes) {
                    break;
                }
            }

            if (searchAllScopes) {
                // If not found in the current or enclosing scopes, search recursively in owned scopes
                foreach (var scope in _scopeStack) {
                    foreach (var symbol in scope.Symbols.Values) {
                        var foundSymbol = FindSymbolInOwnedScope(symbol, name);
                        if (foundSymbol != null) {
                            return foundSymbol;
                        }
                    }
                }
            }

            // Symbol not found in any scope or owned scopes
            return null;
        }

        private Symbol? FindSymbolInOwnedScope(Symbol symbol, string name) {
            if (symbol.OwnedScope != null) {
                if (symbol.OwnedScope.Symbols.TryGetValue(name, out var foundSymbol)) {
                    return foundSymbol;
                }
                // Recursively search in nested scopes
                foreach (var nestedSymbol in symbol.OwnedScope.Symbols.Values) {
                    var recursiveFoundSymbol = FindSymbolInOwnedScope(nestedSymbol, name);
                    if (recursiveFoundSymbol != null) {
                        return recursiveFoundSymbol;
                    }
                }
            }
            return null;
        }
    }

    public class Scope {
        public Dictionary<string, Symbol> Symbols { get; } = new Dictionary<string, Symbol>();

        // Adds a symbol to this scope
        public void AddSymbol(Symbol symbol) {
            if (Symbols.ContainsKey(symbol.Name)) {
                throw new ArgumentException($"Symbol '{symbol.Name}' already exists in this scope.");
            }
            Symbols.Add(symbol.Name, symbol);
        }
    }

}
