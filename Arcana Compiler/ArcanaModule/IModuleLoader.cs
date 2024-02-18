namespace Arcana_Compiler.ArcanaModule {
    public interface IModuleLoader {
        IModule LoadModule(string rootPath);
        void DiscoverSourceFiles(IModule module);
        void ResolveDependencies(IModule module);
    }
}
