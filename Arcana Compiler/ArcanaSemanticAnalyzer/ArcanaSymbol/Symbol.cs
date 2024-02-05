namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol
{
    public abstract class Symbol
    {
        public string Name { get; }

        // An boolean flag to track whether a symbol has been fully defined or just declared.
        // This is to allow forward declarations or to ensure that all declared
        // functions and variables are eventually defined
        public bool IsDefined { get; set; } = false;

        // Additional possible properties:
        // - Visibility (public/private/protected)
        // - Modifiers (static, final, etc.)
        // - Initial value (for variables)
        // - Parameter list (for functions or methods)
        // - Return type (for functions or methods, if not already covered by Type)

        public Symbol(string name)
        {
            Name = name;
        }
    }
}
