using System.Text;

namespace Arcana_Compiler.ArcanaParser.Nodes
{

    internal class ProgramNode : ASTNode
    {
        public List<ImportDeclarationNode> Imports { get; set; } = new List<ImportDeclarationNode>();
        public List<ClassDeclarationNode> ClassDeclarations { get; set; } = new List<ClassDeclarationNode>();

        public override bool Equals(object? obj)
        {
            if (obj is ProgramNode other)
            {
                return Imports.SequenceEqual(other.Imports) && ClassDeclarations.SequenceEqual(other.ClassDeclarations);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var import in Imports)
                {
                    hash = hash * 31 + (import != null ? import.GetHashCode() : 0);
                }
                foreach (var declaration in ClassDeclarations)
                {
                    hash = hash * 31 + (declaration != null ? declaration.GetHashCode() : 0);
                }
                return hash;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Program:");
            builder.AppendLine("Imports:");
            foreach (var import in Imports)
            {
                builder.AppendLine(import.ToString());
            }
            builder.AppendLine("Declarations:");
            foreach (var declaration in ClassDeclarations)
            {
                builder.AppendLine(declaration.ToString());
            }
            return builder.ToString();
        }
    }
}
