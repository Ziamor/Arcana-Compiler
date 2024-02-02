using Arcana_Compiler.ArcanaParser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class ScopeVisualizer {
        private StringBuilder _builder;
        private int _indentationLevel;

        public ScopeVisualizer() {
            _builder = new StringBuilder();
            _indentationLevel = 0;
        }

        public string Visualize(ASTNode astRoot, SymbolTable symbolTable) {
            Visit(astRoot, symbolTable);
            return _builder.ToString();
        }

        private void Visit(ASTNode node, SymbolTable symbolTable) {
            switch (node) {
                case ProgramNode programNode:
                    VisitProgram(programNode, symbolTable);
                    break;
                case ClassDeclarationNode classNode:
                    VisitClassDeclaration(classNode, symbolTable);
                    break;
            }
        }

        private void VisitProgram(ProgramNode node, SymbolTable symbolTable) {
            _builder.AppendLine("Global Scope");
            IncreaseIndentation();
            foreach (var classDeclaration in node.ClassDeclarations) {
                Visit(classDeclaration, symbolTable);
            }
            DecreaseIndentation();
        }

        private void VisitClassDeclaration(ClassDeclarationNode node, SymbolTable symbolTable) {
            AppendIndentedLine($"Class: {node.ClassName}");
            IncreaseIndentation();

            foreach (var field in node.Fields) {
                AppendIndentedLine($"Field: {field.FieldName} ({field.FieldType})");
            }

            foreach (var method in node.Methods) {
                AppendIndentedLine($"Method: {method.MethodName}");
            }

            DecreaseIndentation();
        }

        private void IncreaseIndentation() {
            _indentationLevel++;
        }

        private void DecreaseIndentation() {
            if (_indentationLevel > 0)
                _indentationLevel--;
        }

        private void AppendIndentedLine(string line) {
            _builder.AppendLine(new string(' ', _indentationLevel * 4) + line);
        }
    }
}
