using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {

    public class SymbolTable
    {
        private readonly Stack<Scope> _scopes = new Stack<Scope>();

        public SymbolTable() {
            _scopes.Push(new Scope());
        }

        public void EnterScope()
        {
            _scopes.Push(new Scope());
        }

        public void ExitScope() {
            // Prevent removing the global scope. Ensure there's more than one scope before popping.
            if (_scopes.Count > 1)
                _scopes.Pop();
            // TODO perhaps throw an exception/warning
        }

        public void DeclareSymbol(Symbol symbol)
        {
            if (_scopes.Count == 0)
                throw new InvalidOperationException("No active scope to declare symbols in.");
            _scopes.Peek().DeclareSymbol(symbol);
        }

        public Symbol? LookupSymbol(Symbol symbol, Type symbolType) {
            return LookupSymbol(symbol.Name, symbolType);
        }

        public Symbol? LookupSymbol(string name, Type symbolType, Signature? signature = null, IdentifierName? qualifiedName = null) {
            foreach (var scope in _scopes) {
                var existingSymbol = scope.LookupSymbol(name, symbolType, signature, qualifiedName);
                if (existingSymbol != null)
                    return existingSymbol;
            }
            return null;
        }

        public IType ResolveTypeName(string typeName) {
            switch (typeName) {
                case "int":
                    return BuiltInType.Int;
                case "float":
                    return BuiltInType.Float;
                default:
                    return new UserType(typeName);
            }
        }
    }
}
