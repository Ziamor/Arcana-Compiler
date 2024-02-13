namespace Common {
    public static class MathUtilities {
        // Method without any modifiers
        func CalculateCircleArea(float radius): float {
            return PI * radius * radius
        }

        // Public method
        public func CalculateCircleCircumference(float radius): float {
            return 2 * PI * radius
        }

        // Static method
        static func Square(int number): int {
            return number * number
        }

        // Public and static method
        public static func Cube(int number): int {
            return number * number * number
        }

        // Example showing a potential protected static method
        // Note: This is just illustrative. The actual use of `protected` in static classes may depend on language specifics.
        protected static func GenerateRandomNumber(): int {
            // Implementation to generate a random number
        }

        // Example with multiple modifiers (if applicable)
        // This is purely illustrative; the exact syntax and combination of modifiers depend on the programming language.
        public static func CalculateHypotenuse(float a, float b): float {
            return Math.Sqrt(a * a + b * b)
        }
    }
}