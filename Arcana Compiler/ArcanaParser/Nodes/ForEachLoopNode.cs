using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ForEachLoopNode : StatementNode {
        public VariableDeclarationNode Variable { get; private set; }
        public ASTNode Collection { get; private set; }
        public List<ASTNode> Body { get; private set; }

        public ForEachLoopNode(VariableDeclarationNode variable, ASTNode collection, List<ASTNode> body) {
            Variable = variable;
            Collection = collection;
            Body = body;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"ForEachLoop(Variable: {Variable}, Collection: {Collection}, Body: {Body.Count} statements)";
        }
    }
}
