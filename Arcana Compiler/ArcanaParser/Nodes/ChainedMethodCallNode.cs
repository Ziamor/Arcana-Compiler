namespace Arcana_Compiler.ArcanaParser.Nodes {
    internal class ChainedMethodCallNode : ASTNode {
        public ASTNode PreviousNode { get; private set; }
        public MethodCallNode CurrentCall { get; private set; }

        public ChainedMethodCallNode(ASTNode previousNode, MethodCallNode currentCall) {
            PreviousNode = previousNode ?? throw new ArgumentNullException(nameof(previousNode));
            CurrentCall = currentCall ?? throw new ArgumentNullException(nameof(currentCall));
        }

        public override string ToString() {
            return $"{PreviousNode}.{CurrentCall}";
        }

        public override bool Equals(object? obj) {
            return obj is ChainedMethodCallNode other &&
                   EqualityComparer<ASTNode>.Default.Equals(PreviousNode, other.PreviousNode) &&
                   EqualityComparer<MethodCallNode>.Default.Equals(CurrentCall, other.CurrentCall);
        }

        public override int GetHashCode() {
            return HashCode.Combine(PreviousNode, CurrentCall);
        }
    }
}
