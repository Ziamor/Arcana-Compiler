using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ForLoopNode : StatementNode {
        public StatementNode Initialization { get; private set; }
        public ExpressionNode Condition { get; private set; }
        public ExpressionNode Increment { get; private set; }
        public List<StatementNode> Body { get; private set; }

        public ForLoopNode(StatementNode initialization, ExpressionNode condition, ExpressionNode increment, List<StatementNode> body) {
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
