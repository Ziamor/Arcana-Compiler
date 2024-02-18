using Arcana_Compiler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Arcana_Compiler.Common.ErrorReporter;

namespace Arcana_Compiler.ArcanaSemanticAnalyzer {
    public class SymbolTableError : IError {
        public string Message { get; }
        public int LineNumber { get; }
        public int Position { get; }
        public ErrorSeverity Severity { get; }

        public SymbolTableError(string message, int lineNumber, int position, ErrorSeverity severity) {
            Message = message;
            LineNumber = lineNumber;
            Position = position;
            Severity = severity;
        }
    }
}
