﻿using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class IfStatementNode : StatementNode {
        public List<(ExpressionNode Condition, List<StatementNode> Statements)> ConditionsAndStatements { get; }
        public List<StatementNode>? ElseStatements { get; }

        public IfStatementNode() {
            ConditionsAndStatements = new List<(ExpressionNode Condition, List<StatementNode> Statements)>();
        }

        public IfStatementNode(List<(ExpressionNode Condition, List<StatementNode> Statements)> conditionsAndStatements, List<StatementNode>? elseStatements = null) {
            ConditionsAndStatements = conditionsAndStatements;
            ElseStatements = elseStatements;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString() {
            var result = "IfStatementNode:\n";
            foreach (var (Condition, Statements) in ConditionsAndStatements) {
                result += $"Condition: {Condition}\n";
                foreach (var statement in Statements) {
                    result += $"\tStatement: {statement}\n";
                }
            }

            if (ElseStatements != null) {
                result += "Else:\n";
                foreach (var elseStatement in ElseStatements) {
                    result += $"\tStatement: {elseStatement}\n";
                }
            }

            return result;
        }

        public override bool Equals(object? obj) {
            if (obj == null || obj.GetType() != GetType()) return false;
            var other = (IfStatementNode)obj;

            // Check if ElseStatements are both null or equal
            bool elseStatementsEqual = (ElseStatements == null && other.ElseStatements == null) ||
                                       (ElseStatements != null && other.ElseStatements != null && ElseStatements.SequenceEqual(other.ElseStatements));

            // Check if ConditionsAndStatements are equal
            bool conditionsAndStatementsEqual = ConditionsAndStatements.Count == other.ConditionsAndStatements.Count &&
                                                ConditionsAndStatements.Zip(other.ConditionsAndStatements, (a, b) =>
                                                    a.Condition.Equals(b.Condition) && a.Statements.SequenceEqual(b.Statements)).All(x => x);

            return conditionsAndStatementsEqual && elseStatementsEqual;
        }


        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                // Compute hash code for ConditionsAndStatements
                foreach (var (Condition, Statements) in ConditionsAndStatements) {
                    hash = hash * 31 + Condition.GetHashCode();
                    foreach (var statement in Statements) {
                        hash = hash * 31 + statement.GetHashCode();
                    }
                }

                // Compute hash code for ElseStatements if it's not null
                if (ElseStatements != null) {
                    foreach (var statement in ElseStatements) {
                        hash = hash * 31 + statement.GetHashCode();
                    }
                }

                return hash;
            }
        }

    }
}
