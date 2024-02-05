namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol
{

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

        public Symbol? LookupSymbol(string name, Type symbolType) {
            foreach (var scope in _scopes) {
                var symbol = scope.LookupSymbol(name);
                if (symbol != null && symbol.GetType() == symbolType)
                    return symbol;
            }
            return null;
        }
    }
}
