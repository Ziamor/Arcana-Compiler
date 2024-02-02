using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class Symbol {
        public string Name { get; }
        public Symbol(string name) {
            Name = name;
        }
    }

    public class ClassSymbol : Symbol {
        public QualifiedName QualifiedName { get; }
        public ClassSymbol(string name, QualifiedName qualifiedName) : base(name) {
            QualifiedName = qualifiedName;
        }
    }

    public class Scope {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public void DeclareSymbol(Symbol symbol) {
            if (_symbols.ContainsKey(symbol.Name))
                throw new Exception($"Symbol '{symbol.Name}' is already declared in the current scope.");
            _symbols[symbol.Name] = symbol;
        }

        public Symbol? LookupSymbol(string name) {
            if (_symbols.TryGetValue(name, out var symbol))
                return symbol;
            return null;
        }
    }

    public class SymbolTable {
        private readonly Stack<Scope> _scopes = new Stack<Scope>();

        public void EnterScope() {
            _scopes.Push(new Scope());
        }

        public void ExitScope() {
            _scopes.Pop();
        }

        public void DeclareSymbol(Symbol symbol) {
            if (_scopes.Count == 0)
                throw new InvalidOperationException("No active scope to declare symbols in.");
            _scopes.Peek().DeclareSymbol(symbol);
        }

        public Symbol? LookupSymbol(string name) {
            foreach (var scope in _scopes)
                if (scope.LookupSymbol(name) is Symbol symbol)
                    return symbol;
            return null;
        }
    }
}
