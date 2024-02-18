namespace Arcana_Compiler.ArcanaModule {
    namespace Arcana_Compiler.ArcanaModule {
        public class SingleFileModuleLoader : IModuleLoader {

            public IModule LoadModule(string filePath) {
                string path = Path.GetDirectoryName(filePath) ?? throw new ArgumentException("The file path is invalid or not in a recognized format.", nameof(filePath));
                Module module = new Module(path);
                AddSourceFileToModule(module, filePath);
                ResolveDependencies(module);
                return module;
            }

            private void AddSourceFileToModule(IModule module, string filePath) {
                if (File.Exists(filePath)) {
                    module.AddSourceFile(filePath);
                } else {
                    throw new InvalidOperationException($"The provided file path is not valid or does not exist: {filePath}");
                }
            }

            public void DiscoverSourceFiles(IModule module) {
                throw new NotSupportedException("DiscoverSourceFiles is not supported in SingleFileModuleLoader.");
            }

            public void ResolveDependencies(IModule module) {
            }
        }
    }

}
