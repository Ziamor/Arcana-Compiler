using Arcana_Compiler.ArcanaParser.Nodes;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SemanticAnalyzer {
        public SymbolTable SymbolTable { get; private set; }
        public ASTNode AstRoot { get; private set; }

        public SemanticAnalyzer(ASTNode astRoot) {
            AstRoot = astRoot;
            SymbolTable = new SymbolTable();
        }

        public void Analyze() {
            Visit(AstRoot);
        }

        private void Visit(ASTNode node) {
            switch (node) {
                case ProgramNode programNode:
                    VisitProgram(programNode);
                    break;
                case ClassDeclarationNode classNode:
                    VisitClassDeclaration(classNode);
                    break;
            }
        }

        private void VisitProgram(ProgramNode node) {
            // Process imports, namespaces, etc.
            foreach (var classDeclaration in node.ClassDeclarations) {
                Visit(classDeclaration);
            }
        }

        private void VisitClassDeclaration(ClassDeclarationNode node) {
            // Add class to symbol table, check for redeclarations, etc.
            SymbolTable.EnterScope();

            foreach (var field in node.Fields) {
                VisitFieldDeclaration(field);
            }

            foreach (var method in node.Methods) {
                VisitMethodDeclaration(method);
            }

            SymbolTable.ExitScope();
        }

        private void VisitFieldDeclaration(FieldDeclarationNode node) {
            // Check for type correctness, redeclarations in the same scope, etc.
        }

        private void VisitMethodDeclaration(MethodDeclarationNode node) {
            // Check for return type, parameter types, etc.
        }
    }
}
