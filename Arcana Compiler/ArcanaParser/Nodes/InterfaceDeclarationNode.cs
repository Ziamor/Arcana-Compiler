using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class InterfaceDeclarationNode : ASTNode {
        public IdentifierName Name { get; }
        public List<MethodSignatureNode> Methods { get; }

        public InterfaceDeclarationNode(IdentifierName name, string? accessModifier, List<ClassModifierNode> classModifierNodes, List<MethodSignatureNode> methods) {
            Name = name;
            Methods = methods;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var methodSignatures = string.Join(", ", Methods.Select(m => m.ToString()));
            return $"Interface {Name} {{ {methodSignatures} }}";
        }

        public override bool Equals(object? obj) {
            if (obj is InterfaceDeclarationNode otherNode)
                return Name.Equals(otherNode.Name) && Methods.SequenceEqual(otherNode.Methods);
            return false;
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Name.GetHashCode();
                foreach (var method in Methods) {
                    hash = hash * 23 + method.GetHashCode();
                }
                return hash;
            }
        }

    }
}
