using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.Utilities
{
    public class ASTPrinter
    {
        public string Print(ASTNode node)
        {
            return Print(node, 0);
        }

        private string Print(ASTNode node, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4); // 4 spaces per indent level
            var result = indent + node.ToString();

            var children = GetChildren(node);
            foreach (var child in children)
            {
                result += "\n" + Print(child, indentLevel + 1);
            }

            return result;
        }

        private IEnumerable<ASTNode> GetChildren(ASTNode node)
        {
            return node switch
            {
                ProgramNode programNode => programNode.Imports.Cast<ASTNode>().Concat(programNode.ClassDeclarations),
                ClassDeclarationNode classNode => classNode.Fields.Cast<ASTNode>().Concat(classNode.Methods),
                MethodDeclarationNode methodNode => methodNode.Parameters.Cast<ASTNode>(),
                _ => new List<ASTNode>() // Return an empty list for node types without children
            };
        }
    }

}
