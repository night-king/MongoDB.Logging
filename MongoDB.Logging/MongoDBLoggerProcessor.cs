using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MongoDB.Logging
{
    internal class MongoDBLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;
        private readonly MongoDBLoggerOptions Options;
        private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(_maxQueuedMessages);
        private readonly Thread _outputThread;

        public MongoDBLoggerProcessor(MongoDBLoggerOptions options)
        {
            Options = options;
            // Start Console message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "MongoDB logger queue processing thread"
            };
            _outputThread.Start();
        }
        public IMongoDBStorage MongoDBStorage;

        public virtual void EnqueueMessage(LogMessageEntry message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

        }

        // for testing
        internal virtual void WriteMessage(LogMessageEntry message)
        {
            var connStr = Options == null ? "mongodb://localhost:27017" : Options.Connstr;
            var databaseName = Options == null ? "logs" : Options.Database;
            var collectionName = Options == null ? "log" : Options.Collection;
            MongoDBStorage.Write(connStr, databaseName, collectionName, message);
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(1500); // with timeout in-case Console is locked by user input
            }
            catch (ThreadStateException) { }
        }
    }
}
