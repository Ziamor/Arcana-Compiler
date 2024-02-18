using Arcana_Compiler.ArcanaParser;

namespace Arcana_Compiler.Common {
    public class ErrorReporter {
        public enum ErrorSeverity {
            Warning, // Non-critical issues, code can still compile.
            Error, // Serious issues, code won't compile but parsing continues.
            Fatal // Critical issues, stops compilation process immediately
        }

        public List<IError> Errors { get; } = new List<IError>();

        public void ReportError(IError error) {
            Errors.Add(error);
        }

        public bool HasErrors => Errors.Any();
    }
}
