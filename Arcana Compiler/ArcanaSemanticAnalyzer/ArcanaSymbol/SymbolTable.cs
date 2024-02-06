namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {

    public class SymbolTable
    {
        private readonly Stack<Scope> _scopes = new Stack<Scope>();

        public void EnterScope()
        {
            _scopes.Push(new Scope());
        }

        public void ExitScope()
        {
            _scopes.Pop();
        }

        public void DeclareSymbol(Symbol symbol)
        {
            if (_scopes.Count == 0)
                throw new InvalidOperationException("No active scope to declare symbols in.");
            _scopes.Peek().DeclareSymbol(symbol);
        }

        public Symbol? LookupSymbol(Symbol symbol, Type symbolType) {
            string name = symbol.Name;
            foreach (var scope in _scopes) {
                var existingSymbol = scope.LookupSymbol(name, symbolType);
                if (existingSymbol != null && existingSymbol.GetType() == symbolType)
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
