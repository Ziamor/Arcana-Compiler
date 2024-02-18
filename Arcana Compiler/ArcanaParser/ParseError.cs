using Arcana_Compiler.Common;
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.ArcanaParser {
    public class ParseError : IError {
        public string Message { get; }
        public int LineNumber { get; }
        public int Position { get; }

        public ErrorSeverity Severity { get; }

        public ParseError(string message, int lineNumber, int position, ErrorSeverity severity) {
            Message = message;
            LineNumber = lineNumber;
            Position = position;
            Severity = severity;
        }
    }
}
