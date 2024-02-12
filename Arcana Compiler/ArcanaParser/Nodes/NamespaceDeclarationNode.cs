using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class NamespaceDeclarationNode : ASTNode {
        public IdentifierName Name { get; private set; }
        public List<ClassDeclarationNode> ClassDeclarations { get; private set; }
        public List<InterfaceDeclarationNode> InerfaceDeclarations { get; private set; }

        public NamespaceDeclarationNode(IdentifierName name, List<ClassDeclarationNode> classDeclarations, List<InterfaceDeclarationNode> inerfaceDeclarations) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClassDeclarations = classDeclarations ?? new List<ClassDeclarationNode>();
            InerfaceDeclarations = inerfaceDeclarations;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"namespace {Name} {{...}}";
        }

        public override bool Equals(object? obj) {
            if (obj is NamespaceDeclarationNode other) {
                return Name.Equals(other.Name) &&
                       ClassDeclarations.SequenceEqual(other.ClassDeclarations);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked { // Overflow is fine, just wrap
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Name.GetHashCode();
                foreach (var classDeclaration in ClassDeclarations) {
                    hash = hash * 23 + classDeclaration.GetHashCode();
                }
                return hash;
            }
        }
    }
}
