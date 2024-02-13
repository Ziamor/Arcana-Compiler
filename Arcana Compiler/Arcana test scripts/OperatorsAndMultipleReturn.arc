public class OperatorsTest {
    func calculate(int a, int b): int, int {
        int sum = a + b
        int product = a * b
        return sum, product
    }

    func exampleUsage() {
        int resultSum, int resultProduct = calculate(5, 10)
        // Usage of resultSum and resultProduct
    }
}
