using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.Common;
using System.Text;

namespace Arcana_Compiler.Utilities
{
    public class PrettyPrinter {
        private StringBuilder _builder;
        private int _indentLevel;

        public PrettyPrinter() {
            _builder = new StringBuilder();
            _indentLevel = 0;
        }

        private void Indent() {
            _indentLevel++;
        }

        private void Unindent() {
            _indentLevel--;
        }

        private void AppendLine(string text = "") {
            _builder.Append(new string(' ', _indentLevel * 4)); // Assuming an indent level of 4 spaces
            _builder.AppendLine(text);
        }

        public string Print(ASTNode node) {
            _builder.Clear();
            Visit(node);
            return _builder.ToString();
        }

        private void Visit(ASTNode node) {
            switch (node) {
                case ProgramNode programNode:
                    foreach (ClassDeclarationNode child in programNode.ClassDeclarations) {
                        Visit(child);
                        AppendLine();
                    }
                    break;
                case ClassDeclarationNode classNode:
                    AppendLine($"class {classNode.ClassName} {{");
                    Indent();
                    foreach (var field in classNode.Fields) {
                        Visit(field);
                    }
                    foreach (var method in classNode.Methods) {
                        Visit(method);
                    }
                    Unindent();
                    AppendLine("}");
                    break;
            }
        }
    }

}
