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
                if (scope.Symbols.TryGetValue(name, out var symbolList)) {
                    if (symbolList.Count == 1 || !(symbolList[0] is MethodSymbol)) {
                        return symbolList[0];
                    }
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

        private Symbol? FindSymbolInOwnedScope(List<Symbol> symbols, string name) {
            foreach (var symbol in symbols) {
                if (symbol.OwnedScope is not null) {
                    foreach (var entry in symbol.OwnedScope.Symbols) {
                        var symbolName = entry.Key;
                        var symbolList = entry.Value;

                        // If the name matches, we need to decide how to handle multiple symbols with the same name
                        if (symbolName == name) {
                            // For now, return the first symbol if multiple are found, need to change later
                            return symbolList.FirstOrDefault();
                        }

                        // Recursively search in nested symbols' scopes
                        foreach (var nestedSymbol in symbolList) {
                            var recursiveFoundSymbol = FindSymbolInOwnedScope(new List<Symbol> { nestedSymbol }, name);
                            if (recursiveFoundSymbol != null) {
                                return recursiveFoundSymbol;
                            }
                        }
                    }
                }
            }
            // If we reach this point, the symbol was not found in the provided list or any owned scopes
            return null;
        }
    }
    public class Scope {
        public Dictionary<string, List<Symbol>> Symbols { get; } = new Dictionary<string, List<Symbol>>();

        // Adds a symbol to this scope, supporting overloads
        public void AddSymbol(Symbol symbol) {
            if (!Symbols.TryGetValue(symbol.Name, out var symbolList)) {
                symbolList = new List<Symbol>();
                Symbols[symbol.Name] = symbolList;
            }

            // For methods, check for signature uniqueness
            if (symbol is MethodSymbol methodSymbol) {
                if (symbolList.Any(s => s is MethodSymbol ms && ms.Signature.Equals(methodSymbol.Signature))) {
                    throw new ArgumentException($"A method with the same signature '{methodSymbol.Signature}' already exists in this scope.");
                }
            } else {
                // Non-method symbols should not have duplicates
                if (symbolList.Any()) {
                    throw new ArgumentException($"Symbol '{symbol.Name}' already exists in this scope.");
                }
            }

            symbolList.Add(symbol);
        }
    }
}