﻿using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public class Scope {
        private readonly Dictionary<SymbolKey, Symbol> _symbols = new Dictionary<SymbolKey, Symbol>();

        public void DeclareSymbol(Symbol symbol) {
            Signature? signature = null;
            QualifiedName? qualifiedName = null;
            if (symbol is MethodSymbol methodSymbol) {
                signature = new Signature(methodSymbol.Parameters, methodSymbol.ReturnTypes);
            }
            if (symbol is ClassSymbol classSymbol) {
                qualifiedName = classSymbol.QualifiedName;
            }
            SymbolKey key = new SymbolKey(symbol.Name, symbol.GetType(), signature, qualifiedName);

            if (_symbols.ContainsKey(key)) {                
                throw new Exception($"Symbol '{symbol.Name}' is already declared in the current scope.");
            }

            _symbols[key] = symbol;
        }

        public Symbol? LookupSymbol(string name, Type symbolType, Signature? signature = null) {
            var key = new SymbolKey(name, symbolType, signature);
            if (_symbols.TryGetValue(key, out var symbol))
                return symbol;

            return null;
        }

        public IEnumerable<Symbol> GetSymbols() {
            return _symbols.Values;
        }
    }

    public struct SymbolKey {
        public string Name;
        public Type SymbolType;
        public Signature? Signature; // For methods
        public QualifiedName? QualifiedName; // For classes

        public SymbolKey(string name, Type symbolType, Signature? signature = null, QualifiedName? qualifiedName = null) {
            Name = name;
            SymbolType = symbolType;
            Signature = signature;
            QualifiedName = qualifiedName;
        }

        public override bool Equals(object? obj) {
            return obj is SymbolKey other &&
                   Name == other.Name &&
                   SymbolType == other.SymbolType &&
                   Equals(Signature, other.Signature) &&
                   Equals(QualifiedName, other.QualifiedName);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, SymbolType, Signature, QualifiedName);
        }
    }
}
