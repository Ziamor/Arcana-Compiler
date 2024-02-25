using Arcana_Compiler.Common;

namespace Arcana_Compiler.ArcanaParser.Nodes
{
    public class LiteralNode : PrimaryExpressionNode {
        public object Value { get; private set; }

        public LiteralNode(object value)
        {
            Value = value;
        }
        public override void Accept(IVisitor visitor) {
            visitor.Visit(this);
        }
        public override string ToString()
        {
            // Check if the value is a string, as special formatting might be required
            if (Value is string stringValue)
            {
                return $"\"{stringValue}\""; // Enclose strings in quotes
            }

            // For other types, just use their default string representation
            string? defaultStr = Value.ToString();
            return defaultStr ?? "null";
        }

        public override bool Equals(object? obj)
        {
            if (obj is LiteralNode otherNode)
                return Equals(Value, otherNode.Value);
            return false;
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }
}
