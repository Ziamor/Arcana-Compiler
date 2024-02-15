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

    public abstract class TypeSymbol : Symbol {
        protected TypeSymbol(string name) : base(name) { }

        public override string ToString() => Name;
    }

    public class NamespaceSymbol : Symbol {
        public NamespaceSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"NamespaceSymbol: {Name}";
        }
    }


    public class ClassSymbol : TypeSymbol {
        public ClassSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"ClassSymbol: {Name}";
        }
    }

    public class InterfaceSymbol : TypeSymbol {
        public InterfaceSymbol(string name) : base(name) { }

        public override string ToString() {
            return $"InterfaceSymbol: {Name}";
        }
    }

    public class FieldSymbol : Symbol {
        public TypeSymbol? Type { get; private set; }
        public string TypeName { get; } // Temporary storage for type name, will be resolved later

        public FieldSymbol(string name, string typeName) : base(name) {
            TypeName = typeName;
            Type = null;
        }

        public void ResolveType(TypeSymbol typeSymbol) {
            Type = typeSymbol;
        }

        public override string ToString() {
            return $"FieldSymbol: {Name}, Type: {Type?.Name ?? TypeName}";
        }
    }


    public class MethodSymbol : Symbol {
        public MethodSignature Signature { get; }

        public MethodSymbol(MethodSignature signature) : base(signature.MethodName) {
            Signature = signature;
        }

        public void ResolveSignatureTypes(SymbolTable symbolTable) {
            Signature.ResolveTypes(symbolTable);
        }

        public override string ToString() {
            return $"MethodSymbol: {Name}, Parameters: [{string.Join(", ", Signature.ParameterTypesNames)}], Returns: [{string.Join(", ", Signature.ReturnTypesNames)}]";
        }
    }

    public class VariableSymbol : Symbol {
        public TypeSymbol Type { get; }
        public VariableSymbol(string name, TypeSymbol typeSymbol) : base(name) {
            Type = typeSymbol;
        }

        public override string ToString() {
            return $"VariableSymbol: {Name}";
        }
    }
}