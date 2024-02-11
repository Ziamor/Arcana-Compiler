using System.Text;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ProgramNode : ASTNode {
        public List<ImportDeclarationNode> Imports { get; set; } = new List<ImportDeclarationNode>();
        public List<NamespaceDeclarationNode> NamespaceDeclarations { get; set; } = new List<NamespaceDeclarationNode>();
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override bool Equals(object? obj) {
            if (obj is ProgramNode other) {
                return Imports.SequenceEqual(other.Imports) && NamespaceDeclarations.SequenceEqual(other.NamespaceDeclarations);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 19;
                foreach (var import in Imports) {
                    hash = hash * 31 + (import != null ? import.GetHashCode() : 0);
                }
                foreach (var declaration in NamespaceDeclarations) {
                    hash = hash * 31 + (declaration != null ? declaration.GetHashCode() : 0);
                }
                return hash;
            }
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("Program:");
            builder.AppendLine("Imports:");
            foreach (var import in Imports) {
                builder.AppendLine(import.ToString());
            }
            builder.AppendLine("Namespaces:");
            foreach (var declaration in NamespaceDeclarations) {
                builder.AppendLine(declaration.ToString());
            }
            return builder.ToString();
        }
    }
}
