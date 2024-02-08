using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SymbolTableBuilder : IVisitor {
        public SymbolTable SymbolTable { get; private set; }
        public ProgramNode AstRoot { get; private set; }

        public SymbolTableBuilder(ProgramNode astRoot, SymbolTable? existingSymbolTable = null) {
            AstRoot = astRoot;
            SymbolTable = existingSymbolTable ?? new SymbolTable();
        }

        public void Analyze() {
            Visit(AstRoot);
        }

        private void Visit(ProgramNode node) {
            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }
        }

        public void Visit(ClassDeclarationNode node) {
            ClassSymbol classSymbol = new ClassSymbol(node.ClassName, node.Namespace);
            if (SymbolTable.LookupSymbol(classSymbol, typeof(ClassSymbol)) != null) {
                throw new SemanticException($"Class {node.ClassName} in namespace {node.Namespace} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(classSymbol);

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
            FieldSymbol fieldSymbol = new FieldSymbol(node.FieldName);
            if (SymbolTable.LookupSymbol(fieldSymbol, typeof(FieldSymbol)) != null) {
                throw new SemanticException($"Field {node.FieldName} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(fieldSymbol);
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
            List<Parameter> parameters = node.Parameters.Select(
                p => new Parameter(p.ParameterName, SymbolTable.ResolveTypeName(p.ParameterType.TypeName))
            ).ToList();
            List<ReturnType> returnTypes = node.ReturnTypes.Select(
                rt => new ReturnType(new UserType(rt.TypeName))
            ).ToList();

            Signature signature = new Signature(parameters, returnTypes);
            MethodSymbol methodSymbol = new MethodSymbol(node.MethodName, signature);

            if (SymbolTable.LookupSymbol(methodSymbol, typeof(MethodSymbol)) != null) {
                throw new SemanticException($"Method {node.MethodName} is already declared in this scope.");
            }

            SymbolTable.DeclareSymbol(methodSymbol);

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

        public void Visit(ObjectInstantiationNode node) {
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
