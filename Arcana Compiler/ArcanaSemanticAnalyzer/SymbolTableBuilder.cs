using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SymbolTableBuilder : IVisitor {
        private readonly SymbolTable _symbolTable;

        public SymbolTableBuilder(SymbolTable symbolTable) {
            _symbolTable = symbolTable;
        }

        public void Visit(ProgramNode node) {
            foreach (var import in node.Imports) {
                import.Accept(this);
            }

            foreach (var namespaceDeclaration in node.NamespaceDeclarations) {
                namespaceDeclaration.Accept(this);
            }
        }

        public void Visit(ImportDeclarationNode node) {
            Console.WriteLine("TODO, implement visit import node");
        }

        public void Visit(NamespaceDeclarationNode node) {
            var namespaceSymbol = new NamespaceSymbol(node.Name.ToString());
            _symbolTable.AddSymbol(namespaceSymbol); // Add the namespace symbol to the current scope
            _symbolTable.EnterScope(namespaceSymbol); // Enter the new namespace scope

            foreach (var classDeclaration in node.ClassDeclarations) {
                classDeclaration.Accept(this);
            }

            foreach (var interfaceDeclaration in node.InerfaceDeclarations) {
                interfaceDeclaration.Accept(this);
            }

            _symbolTable.ExitScope();
        }

        public void Visit(ClassDeclarationNode node) {
            var classSymbol = new ClassSymbol(node.ClassName);
            _symbolTable.AddSymbol(classSymbol);
            _symbolTable.EnterScope(classSymbol);

            foreach (var field in node.Fields) {
                field.Accept(this);
            }

            foreach (var method in node.Methods) {
                method.Accept(this);
            }

            // TODO NESTED CLASSES

            _symbolTable.ExitScope();
        }

        public void Visit(FieldDeclarationNode node) {
            var fieldSymbol = new FieldSymbol(node.FieldName, node.FieldType.TypeName);
            _symbolTable.AddSymbol(fieldSymbol);
        }

        public void Visit(InterfaceDeclarationNode node) { }
        public void Visit(MethodSignatureNode node) { }

        public void Visit(ClassModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(FieldModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(TypeNode node) {
            throw new NotImplementedException();
        }

        public void Visit(LiteralNode node) {
            throw new NotImplementedException();
        }

        public void Visit(NullLiteralNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ObjectInstantiationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ThisExpressionNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ThisAssignmentNode node) {
            throw new NotImplementedException();
        }

        public void Visit(VariableAccessNode node) {
            throw new NotImplementedException();
        }

        public void Visit(VariableDeclarationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(VariableAssignmentNode node) {
            throw new NotImplementedException();
        }

        public void Visit(UnaryOperationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(BinaryOperationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(MethodDeclarationNode node) {
            var parameterTypes = node.Parameters.Select(p => p.ParameterType.TypeName).ToList();
            var returnTypes = node.ReturnTypes.Select(rt => rt.TypeName).ToList();
            var signature = new MethodSignature(node.MethodName, parameterTypes, returnTypes);

            var methodSymbol = new MethodSymbol(signature);

            _symbolTable.AddSymbol(methodSymbol);
        }


        public void Visit(MethodModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(MethodCallNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ParameterNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ChainedMethodCallNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ChainedPropertyAccessNode node) {
            throw new NotImplementedException();
        }

        public void Visit(IfStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(DestructuringAssignmentNode node) {
            throw new NotImplementedException();
        }
    }
}
