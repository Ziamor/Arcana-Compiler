using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer
{
    public class SemanticAnalyzer : IVisitor {
        public SymbolTable SymbolTable { get; private set; }
        public ProgramNode AstRoot { get; private set; }

        public SemanticAnalyzer(ProgramNode astRoot) {
            AstRoot = astRoot;
            SymbolTable = new SymbolTable();
        }

        public void Analyze() {
            Visit(AstRoot);
        }

        private void Visit(ProgramNode node) {
            SymbolTable.EnterScope();

            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }

            SymbolTable.ExitScope();
        }

        public void Visit(ClassDeclarationNode node) {
            if (SymbolTable.LookupSymbol(node.ClassName, typeof(ClassSymbol)) != null) {
                throw new SemanticException($"Class {node.ClassName} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(new ClassSymbol(node.ClassName, node.Namespace));

            SymbolTable.EnterScope();

            foreach (var field in node.Fields) {
                field.Accept(this);
            }
            foreach (var method in node.Methods) {
                method.Accept(this);
            }

            SymbolTable.ExitScope();
        }
                
        void IVisitor.Visit(ProgramNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(ImportDeclarationNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(FieldDeclarationNode node) {
            if (SymbolTable.LookupSymbol(node.FieldName, typeof(FieldSymbol)) != null) {
                throw new SemanticException($"Field {node.FieldName} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(new FieldSymbol(node.FieldName));
        }

        void IVisitor.Visit(TypeNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(LiteralNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(NullLiteralNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(VariableAccessNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(VariableDeclarationNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(VariableAssignmentNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(UnaryOperationNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(BinaryOperationNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(MethodDeclarationNode node) {
            if (SymbolTable.LookupSymbol(node.MethodName, typeof(MethodSymbol)) != null) {
                throw new SemanticException($"Method {node.MethodName} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(new MethodSymbol(node.MethodName));

            SymbolTable.EnterScope();
            // TODO BODY
            SymbolTable.ExitScope();
        }

        void IVisitor.Visit(MethodCallNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(ParameterNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(ChainedMethodCallNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(ChainedPropertyAccessNode node) {
            throw new NotImplementedException();
        }

        void IVisitor.Visit(IfStatementNode node) {
            throw new NotImplementedException();
        }
    }

    public class SemanticException : Exception {
        public SemanticException(string message) : base(message) { }

        public SemanticException(string message, Exception innerException) : base(message, innerException) { }

        public SemanticException(string message, int lineNumber, int position)
            : base($"Error at line {lineNumber}, position {position}: {message}") {
        }
    }
}
