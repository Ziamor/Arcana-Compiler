namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol
{
    public class Scope
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public void DeclareSymbol(Symbol symbol)
        {
            if (_symbols.ContainsKey(symbol.Name))
                throw new Exception($"Symbol '{symbol.Name}' is already declared in the current scope.");
            _symbols[symbol.Name] = symbol;
        }

        public Symbol? LookupSymbol(string name)
        {
            if (_symbols.TryGetValue(name, out var symbol))
                return symbol;
            return null;
        }
    }
}
