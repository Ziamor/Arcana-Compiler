namespace Arcana_Compiler.ArcanaModule {
    public interface IModuleLoader {
        Module LoadModule(string rootPath);
        void DiscoverSourceFiles(Module module);
        void ResolveDependencies(Module module);
    }
}
