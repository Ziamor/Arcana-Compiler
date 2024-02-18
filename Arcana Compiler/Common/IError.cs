
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.Common {
    public interface IError {
        string Message { get; }
        int LineNumber { get; }
        int Position { get; }
        ErrorSeverity Severity { get; }
    }
}
