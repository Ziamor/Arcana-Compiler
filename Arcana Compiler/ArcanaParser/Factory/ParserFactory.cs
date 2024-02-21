using Arcana_Compiler.ArcanaLexer;
using Arcana_Compiler.Common;
using Arcana_Compiler.ArcanaParser.Parsers;
using Arcana_Compiler.ArcanaParser.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arcana_Compiler.ArcanaParser.Factory {
    public class ParserFactory {
        private readonly ILexer _lexer;
        private readonly ErrorReporter _errorReporter;
        private readonly Dictionary<Type, Func<ILexer, ErrorReporter, object>> _parserCreators = new();

        public ParserFactory(ILexer lexer, ErrorReporter errorReporter) {
            _lexer = lexer;
            _errorReporter = errorReporter;

            AutoRegisterParsers();
        }

        private void AutoRegisterParsers() {
            var parserTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParser<>)))
                .ToList();

            foreach (var type in parserTypes) {
                var interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParser<>));
                var nodeType = interfaceType.GetGenericArguments().First();

                _parserCreators[nodeType] = (lexer, reporter) => {
                    // Find the appropriate constructor
                    var constructor = type.GetConstructor(new[] { typeof(ILexer), typeof(ErrorReporter), typeof(ParserFactory) });
                    if (constructor == null) {
                        throw new InvalidOperationException($"No suitable constructor found for parser type {type.Name}");
                    }
                    // Invoke the constructor with arguments
                    return constructor.Invoke(new object[] { lexer, reporter, this });
                };
            }
        }


        public void RegisterParser<TNode>(Func<ILexer, ErrorReporter, IParser<TNode>> creator)
            where TNode : ASTNode {
            _parserCreators[typeof(TNode)] = (lexer, reporter) => creator(lexer, reporter);
        }

        public IParser<T> CreateParser<T>() where T : ASTNode {
            if (_parserCreators.TryGetValue(typeof(T), out var creator)) {
                var parserInstance = creator(_lexer, _errorReporter);
                if (parserInstance is IParser<T> parser) {
                    return parser;
                }
                throw new InvalidOperationException($"Parser instance created does not match the requested type {typeof(T).Name}.");
            }

            throw new InvalidOperationException($"No parser registered for type {typeof(T).Name}");
        }
    }
}
