namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class UserType : IType {
        public string Name { get; private set; }

        public UserType(string name) {
            Name = name;
        }
    }

}
