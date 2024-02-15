namespace Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol {
    public abstract class Symbol {
        public string Name { get; }
        public Scope? OwnedScope { get; internal set; } 

        protected Symbol(string name) {
            Name = name;
            OwnedScope = null; // Initially, symbols do not own a scope
        }

        public override string ToString() {
            return $"Symbol: {Name}";
        }
    }

    public class NamespaceSymbol : Symbol {
        public NamespaceSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"NamespaceSymbol: {Name}";
        }
    }


    public class ClassSymbol : Symbol {
        public ClassSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"ClassSymbol: {Name}";
        }
    }

    public class InterfaceSymbol : Symbol {
        public InterfaceSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"InterfaceSymbol: {Name}";
        }
    }

    public class FieldSymbol : Symbol {
        public FieldSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"FieldSymbol: {Name}";
        }
    }

    public class MethodSymbol : Symbol {
        public MethodSignature Signature { get; }

        public MethodSymbol(MethodSignature signature) : base(signature.MethodName) {
            Signature = signature;
        }

        public override string ToString() {
            return $"MethodSymbol: {Name}";
        }
    }

    public class VariableSymbol : Symbol {
        public VariableSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"VariableSymbol: {Name}";
        }
    }
}