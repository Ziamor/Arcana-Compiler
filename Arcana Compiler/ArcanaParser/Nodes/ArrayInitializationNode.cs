using Arcana_Compiler.Common;
using System.Collections.Generic;
using System.Linq;

namespace Arcana_Compiler.ArcanaParser.Nodes {
    public class ArrayInitializationNode : ExpressionNode {
        public IReadOnlyList<ExpressionNode> ArrayElements { get; }
        public ExpressionNode Size { get; private set; }

        public ArrayInitializationNode(List<ExpressionNode> arrayElements) {
            if (arrayElements == null || !arrayElements.Any()) {
                throw new ParsingException("Array initialization must have at least one element.");
            }
            ArrayElements = arrayElements;
            Size = new LiteralNode(arrayElements.Count);
        }

        public ArrayInitializationNode(ExpressionNode size) {
            Size = size ?? throw new ParsingException("Size expression cannot be null.");
            ArrayElements = new List<ExpressionNode>();
        }

        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }

        public override string ToString() {
            var elementsAsString = string.Join(", ", ArrayElements.Select(element => element.ToString()));
            return $"ArrayInitialization(Size={Size}, Elements=[{elementsAsString}])";
        }

        public override bool Equals(object? obj) {
            return obj is ArrayInitializationNode other &&
                   EqualityComparer<ExpressionNode>.Default.Equals(Size, other.Size) &&
                   ArrayElements.SequenceEqual(other.ArrayElements);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + Size.GetHashCode();
                foreach (var element in ArrayElements) {
                    hash = hash * 23 + element.GetHashCode();
                }
                return hash;
            }
        }
    }
}
