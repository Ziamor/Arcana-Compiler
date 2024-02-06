using Arcana_Compiler.Common;
using System.Security.AccessControl;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol
{
    public class Scope {
        private readonly Dictionary<SymbolKey, Symbol> _symbols = new Dictionary<SymbolKey, Symbol>();

        public void DeclareSymbol(Symbol symbol) {
            string? signature = null;
            QualifiedName? qualifiedName = null;
            if (symbol is MethodSymbol methodSymbol) {
                signature = GenerateSignature(methodSymbol);
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

        public Symbol? LookupSymbol(string name, Type symbolType, string? signature = null) {
            var key = new SymbolKey(name, symbolType, signature);
            if (_symbols.TryGetValue(key, out var symbol))
                return symbol;

            return null;
        }

        private string GenerateSignature(MethodSymbol methodSymbol) {
            return string.Join(",", methodSymbol.Parameters.Select(p => p.Type.Name));
        }

        public IEnumerable<Symbol> GetSymbols() {
            return _symbols.Values;
        }
    }

    public struct SymbolKey {
        public string Name;
        public Type SymbolType;
        public string? Signature; // For methods
        public QualifiedName? QualifiedName; // For classes

        public SymbolKey(string name, Type symbolType, string? signature = null, QualifiedName? qualifiedName = null) {
            Name = name;
            SymbolType = symbolType;
            Signature = signature;
            QualifiedName = qualifiedName;
        }

        public override bool Equals(object? obj) {
            if (obj is SymbolKey other) {
                return Name == other.Name && SymbolType == other.SymbolType &&
                       Signature == other.Signature && Equals(QualifiedName, other.QualifiedName);
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, SymbolType, Signature, QualifiedName);
        }
    }
}
