﻿using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class BinaryOperationNode : ExpressionNode {
        public ExpressionNode Left { get; private set; }
        public Token Operator { get; private set; }
        public ExpressionNode Right { get; private set; }

        public BinaryOperationNode(ExpressionNode left, Token operatorToken, ExpressionNode right) {
            Left = left;
            Operator = operatorToken;
            Right = right;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            return $"({Left} {Operator.Value} {Right})";
        }

        public override bool Equals(object? obj) {
            if (obj is BinaryOperationNode other) {
                return Left.Equals(other.Left) && Operator.Equals(other.Operator) && Right.Equals(other.Right);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked { // Overflow is fine, just wrap
                int hash = 17;
                hash = hash * 23 + Left.GetHashCode();
                hash = hash * 23 + Operator.GetHashCode();
                hash = hash * 23 + Right.GetHashCode();
                return hash;
            }
        }
    }
}
