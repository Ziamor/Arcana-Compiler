using Arcana_Compiler.Common;
using System.Text;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class NamespaceDeclarationNode : ASTNode {
        public QualifiedName NamespaceName { get; private set; }
        public List<ASTNode> ClassDeclarations { get; private set; } = new List<ASTNode>();

        public NamespaceDeclarationNode(QualifiedName namespaceName, List<ASTNode> classDeclarations) {
            NamespaceName = namespaceName;
            ClassDeclarations = classDeclarations;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine($"Namespace: {NamespaceName}");
            builder.AppendLine("Classes:");
            foreach (var classDecl in ClassDeclarations) {
                builder.AppendLine(classDecl.ToString());
            }
            return builder.ToString();
        }

        public override bool Equals(object? obj) {
            if (obj is NamespaceDeclarationNode other) {
                return NamespaceName.Equals(other.NamespaceName) && ClassDeclarations.SequenceEqual(other.ClassDeclarations);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = NamespaceName.GetHashCode();
                foreach (var classDecl in ClassDeclarations) {
                    hash = hash * 31 + classDecl.GetHashCode();
                }
                return hash;
            }
        }
    }
}
