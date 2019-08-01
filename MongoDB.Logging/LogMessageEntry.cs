using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Logging
{
    public  struct LogMessageEntry
    {
        public readonly string Id;
        /// <summary>
        /// 时间
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// 消息等级
        /// </summary>
        public readonly LogLevel Level;

        /// <summary>
        /// 消息内容
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// 操作系统
        /// </summary>
        public readonly string OS;

        /// <summary>
        /// 发生IP
        /// </summary>
        public readonly string IP;

        /// <summary>
        /// 进程名称
        /// </summary>
        public readonly string ProcessName;

        public LogMessageEntry(LogLevel level, string message, DateTime timestamp, string os, string ip, string processorName)
        {
            Id = Guid.NewGuid().ToString("N");
            Level = level;
            Message = message;
            Timestamp = timestamp;
            IP = ip;
            OS = os;
            ProcessName = processorName;
        }
    }
}
