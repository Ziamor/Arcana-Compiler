namespace Arcana_Compiler.ArcanaModule {
    public interface IModule {
        string Name { get; }
        string Path { get; }
        IEnumerable<string> SourceFiles { get; }
        void AddSourceFile(string filePath);
        void AddDependency(IModule module);
    }
}
