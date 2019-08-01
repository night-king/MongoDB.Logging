using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace MongoDB.Logging
{
    internal class MongoDBLogger : ILogger
    {
        private readonly string _name;

        private readonly MongoDBLoggerProcessor _processor;
        internal MongoDBLoggerOptions Options { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="processor">processor.</param>
        /// 
        public MongoDBLogger(string name, MongoDBLoggerProcessor processor)
        {
            _name = name;
            _processor = processor;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            return logLevel != LogLevel.None;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }
            var ip = GetAddressIP();
            var os = RuntimeInformation.OSDescription;
            var process = Process.GetCurrentProcess();
            var processName = process.Id.ToString() + ":" + process.ProcessName;
            var entry = new LogMessageEntry(logLevel, message, DateTime.Now, os, ip, processName);
            _processor.EnqueueMessage(entry);
        }

        /// <summary>
        /// 获取本地IP地址信息
        /// </summary>
        private string GetAddressIP()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }
    }
}
