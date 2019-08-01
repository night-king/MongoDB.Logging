using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
namespace MongoDB.Logging
{
    [ProviderAlias("MongoDB")]
    public class MongoDBLoggerProvider : ILoggerProvider
    {
        private readonly IOptionsMonitor<MongoDBLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, MongoDBLogger> _loggers;
        private readonly MongoDBLoggerProcessor _messageQueue;

        public MongoDBLoggerProvider(IOptionsMonitor<MongoDBLoggerOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, MongoDBLogger>();
            _messageQueue = new MongoDBLoggerProcessor(options.CurrentValue);
            _messageQueue.MongoDBStorage = new MongoDBStorage();

        }
        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, loggerName => new MongoDBLogger(loggerName, _messageQueue)
            {
                Options = _options.CurrentValue,
            });
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}
