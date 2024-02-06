using Arcana_Compiler.ArcanaSemanticAnalyzer;

namespace ArcanaCompilerTests {
    [TestFixture]
    public class SignatureTests {
        [Test]
        public void SignaturesWithSameParametersAndReturnTypes_AreEqual() {
            // Arrange
            var parameters = new List<Parameter> {
                new Parameter("param1", new MockType("TypeA")),
                new Parameter("param2", new MockType("TypeB"))
            };
            var returnTypes = new List<ReturnType> {
                new ReturnType(new MockType("ReturnType"))
            };

            var signature1 = new Signature(parameters, returnTypes);
            var signature2 = new Signature(parameters, returnTypes);

            // Act & Assert
            Assert.AreEqual(signature1, signature2, "Signatures with the same parameters and return types should be equal.");
        }

        [Test]
        public void SignaturesWithDifferentParametersOrReturnTypes_AreNotEqual() {
            // Arrange
            var parameters1 = new List<Parameter> {
                new Parameter("param1", new MockType("TypeA")),
                new Parameter("param2", new MockType("TypeB"))
            };
            var returnTypes1 = new List<ReturnType> {
                new ReturnType(new MockType("ReturnType"))
            };

            var parameters2 = new List<Parameter> { // Different parameter
                new Parameter("param1", new MockType("TypeC")),
                new Parameter("param2", new MockType("TypeB"))
            };
            var returnTypes2 = new List<ReturnType> { // Different return type
                new ReturnType(new MockType("ReturnTypeDifferent"))
            };

            var signature1 = new Signature(parameters1, returnTypes1);
            var signature2 = new Signature(parameters2, returnTypes2);

            // Act & Assert
            Assert.AreNotEqual(signature1, signature2, "Signatures with different parameters or return types should not be equal.");
        }

        // Mock IType implementation for testing purposes
        class MockType : IType {
            public string Name { get; }

            public MockType(string name) {
                Name = name;
            }

            // Implement equality based on type name for simplicity
            public override bool Equals(object? obj) {
                return obj is MockType other && Name == other.Name;
            }

            public override int GetHashCode() {
                return Name.GetHashCode();
            }
        }
    }
}
