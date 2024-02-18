using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ForLoopNode : ASTNode {
        public ASTNode Initialization { get; private set; }
        public ASTNode Condition { get; private set; }
        public ASTNode Increment { get; private set; }
        public List<ASTNode> Body { get; private set; }

        public ForLoopNode(ASTNode initialization, ASTNode condition, ASTNode increment, List<ASTNode> body) {
            Initialization = initialization;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            return $"ForLoop(Initialization: {Initialization}, Condition: {Condition}, Increment: {Increment}, Body: {Body.Count} statements)";
        }
    }
}
