namespace Arcana_Compiler.ArcanaParser.Nodes {
    /// <summary>
    /// During the parsing stage, we can't be sure if a class is extending another base class
    /// or if it's implmenting an interface. So ParentTypeNode is a generic way to store both
    /// </summary>
    internal class ParentTypeNode {
        public string TypeName { get; private set; }

        public ParentTypeNode(string typeName) {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public override string ToString() {
            return TypeName;
        }

        public override bool Equals(object? obj) {
            return obj is ParentTypeNode other && TypeName == other.TypeName;
        }

        public override int GetHashCode() {
            return TypeName.GetHashCode();
        }
    }
}
