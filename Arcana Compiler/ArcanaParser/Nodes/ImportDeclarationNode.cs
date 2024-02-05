using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class ImportDeclarationNode : ASTNode
    {
        public QualifiedName ImportedNamespace { get; private set; }

        public ImportDeclarationNode(QualifiedName importedNamespace)
        {
            ImportedNamespace = importedNamespace;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString()
        {
            return $"Import: {ImportedNamespace}";
        }

        public override bool Equals(object? obj)
        {
            return obj is ImportDeclarationNode other &&
                   ImportedNamespace.Equals(other.ImportedNamespace);
        }

        public override int GetHashCode()
        {
            return ImportedNamespace.GetHashCode();
        }
    }
}
