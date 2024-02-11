namespace Arcana_Compiler.ArcanaModule {
    public class FolderModuleLoader : IModuleLoader {
        public Module LoadModule(string rootPath) {
            var module = new Module(rootPath);
            DiscoverSourceFiles(module);
            ResolveDependencies(module);
            return module;
        }

        public void DiscoverSourceFiles(Module module) {
            if (Directory.Exists(module.Path)) {
                var arcFiles = Directory.GetFiles(module.Path, "*.arc", SearchOption.AllDirectories);
                foreach (var file in arcFiles) {
                    module.AddSourceFile(file);
                }
            } else {
                throw new InvalidOperationException($"The provided path is not valid or does not exist: {module.Path}");
            }
        }

        public void ResolveDependencies(Module module) {
            var directories = Directory.GetDirectories(module.Path, "*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories) {
                var dependencyModule = new Module(directory);
                module.AddDependency(dependencyModule);
            }
        }
    }
}
