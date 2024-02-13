using Arcana_Compiler.ArcanaParser.Nodes;
using Arcana_Compiler.ArcanaSemanticAnalyzer.ArcanaSymbol;
using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class TypeChecker : IVisitor {
        private readonly SymbolTable _symbolTable;

        public TypeChecker(SymbolTable symbolTable) {
            _symbolTable = symbolTable;
        }

        public void Check(ProgramNode astRoot) {
            Visit(astRoot);
        }

        public void Visit(ProgramNode node) {
            foreach (var namespaceDeclarations in node.NamespaceDeclarations) {
                foreach (var classDeclaration in namespaceDeclarations.ClassDeclarations) {
                    classDeclaration.Accept(this);
                }
            }
        }

        public void Visit(ClassDeclarationNode node) {
            foreach (var field in node.Fields) {
                field.Accept(this);
            }
            foreach (var method in node.Methods) {
                method.Accept(this);
            }
        }

        public void Visit(FieldDeclarationNode node) {
            if (node.InitialValue != null) {
                var initialValueType = EvaluateType(node.InitialValue);

                if (!IsTypeCompatible(node.FieldType, initialValueType)) {
                    throw new SemanticException($"Type of the initial value for field {node.FieldName} is not compatible with its declared type {node.FieldType}.");
                }
            }
        }

        public void Visit(MethodDeclarationNode node) {
            foreach (var parameter in node.Parameters) {
                parameter.Accept(this);
            }
            foreach (var statement in node.Body) {
                statement.Accept(this);
            }
            // TODO return types
        }

        public void Visit(BinaryOperationNode node) {
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        public void Visit(ImportDeclarationNode node) {
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

        private TypeNode EvaluateType(ASTNode node) {
            switch (node) {
                case LiteralNode literalNode:
                    return EvaluateLiteralNodeType(literalNode);
                case VariableAccessNode variableAccessNode:
                    return EvaluateVariableAccessType(variableAccessNode);
                case BinaryOperationNode binaryOperationNode:
                    return EvaluateBinaryOperationType(binaryOperationNode);
                case ObjectInstantiationNode objectInstantiationNode:
                    return EvaluateObjectInstantiationType(objectInstantiationNode);
                default:
                    throw new NotImplementedException($"Type evaluation not implemented for node type {node.GetType().Name}");
            }
        }

        private TypeNode EvaluateLiteralNodeType(LiteralNode node) {
            // Determine type based on the runtime type of the value
            var valueType = node.Value.GetType();
            if (valueType == typeof(int)) {
                return new TypeNode("int", false); // Example for non-nullable int
            } else if (valueType == typeof(string)) {
                return new TypeNode("string", true); // Assuming strings can be nullable
            } else if (valueType == typeof(bool)) {
                return new TypeNode("bool", false); // Example for non-nullable bool
            }

            throw new SemanticException($"Unsupported literal type: {valueType.Name}");
        }

        private TypeNode EvaluateVariableAccessType(VariableAccessNode node) {
            throw new NotImplementedException();
            /*var symbol = _symbolTable.LookupSymbol(node.VariableName, typeof(VariableSymbol)) as VariableSymbol;
            if (symbol == null) {
                throw new SemanticException($"Variable {node.VariableName} not found.");
            }
            // Assuming symbol.Type is a string representing the type name
            return new TypeNode(node.type, symbol.IsNullable);*/
        }

        private TypeNode EvaluateBinaryOperationType(BinaryOperationNode node) {
            var leftType = EvaluateType(node.Left);
            var rightType = EvaluateType(node.Right);

            if (leftType.TypeName == "int" && rightType.TypeName == "int") {
                return new TypeNode("int", false);
            }

            // String concatenation
            else if (leftType.TypeName == "string" && rightType.TypeName == "string" && node.Operator.Value == "+") {
                return new TypeNode("string", false); // Strings are not nullable by default
            }

            throw new SemanticException($"Unsupported binary operation {node.Operator.Value} between types {leftType.TypeName} and {rightType.TypeName}.");
        }
        private TypeNode EvaluateObjectInstantiationType(ObjectInstantiationNode node) {
            ClassSymbol? classSymbol;
            if (node.ClassName.IsFullyQualified) {
                IdentifierName namespacePart = node.ClassName.Qualifiers;
                classSymbol = _symbolTable.LookupSymbol(node.ClassName.Identifier, typeof(ClassSymbol), null, namespacePart) as ClassSymbol;
            } else {
                classSymbol = _symbolTable.LookupSymbol(node.ClassName.Identifier, typeof(ClassSymbol), null, IdentifierName.DefaultNameSpace) as ClassSymbol;
            }

            if (classSymbol == null) {
                // TODO search if another namespace has a class with that name
                if (classSymbol == null) {
                    throw new SemanticException($"Class {node.ClassName} not found.");
                }
            }
            return new TypeNode(node.ClassName.ToString(), false);
        }

        public bool IsTypeCompatible(TypeNode expected, TypeNode actual) {
            if (expected == actual) return true;

            if (CanImplicitlyConvert(actual, expected)) return true;

            return false;
        }

        private bool CanImplicitlyConvert(TypeNode actual, TypeNode expected) {
            // If the types are the same, no conversion is needed
            if (actual.TypeName == expected.TypeName) return true;

            switch (actual.TypeName) {
                case "int":
                    if (expected.TypeName == "float" || expected.TypeName == "double") return true;
                    break;
                case "float":
                    if (expected.TypeName == "double") return true;
                    break;
                    // TODO more cases
            }

            return false;
        }

        public void Visit(ObjectInstantiationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(NamespaceDeclarationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ThisExpressionNode node) {
            throw new NotImplementedException();
        }

        public void Visit(InterfaceDeclarationNode node) {
            throw new NotImplementedException();
        }

        public void Visit(MethodSignatureNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ThisAssignmentNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatementNode node) {
            throw new NotImplementedException();
        }

        public void Visit(DestructuringAssignmentNode node) {
            throw new NotImplementedException();
        }

        public void Visit(ClassModifierNode node) {
            throw new NotImplementedException();
        }

        public void Visit(FieldModifierNode node) {
            throw new NotImplementedException();
        }
    }
}
