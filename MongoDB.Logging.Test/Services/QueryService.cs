using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Logging.Test.Services
{
    public class QueryService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _tokenKeep = new CancellationTokenSource();

        public QueryService(
            ILogger<LogService> logger,
            IApplicationLifetime appLifetime,
            IConfiguration configuration)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _configuration = configuration;
        }
        private void OnStarted()
        {
            _logger.LogWarning("OnStarted");
        }

        private void OnStopping()
        {
            _logger.LogWarning("OnStopping");
            _tokenKeep.Cancel();
        }

        private void OnStopped()
        {
            _logger.LogWarning("OnStopped");

        }
        private void log()
        {
            var interval = 60000;
            while (_tokenKeep.IsCancellationRequested == false)
            {
                var loggingSettings = _configuration.GetSection("Logging");
                var mongoDBSettings = loggingSettings == null ? null : loggingSettings.GetSection("MongoDB");

                var conn = mongoDBSettings == null ? "mongodb://localhost:27017" : mongoDBSettings["Conn"];
                var databaseName = mongoDBSettings == null ? "logs" : mongoDBSettings["Database"];
                var collectionName = mongoDBSettings == null ? "log" : mongoDBSettings["Collection"];

                var mongodbClient = new MongoClient(conn);
                var database = mongodbClient.GetDatabase(databaseName);
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var query = collection.Find(new BsonDocument());
                var total = (int)query.CountDocuments();//Total 
                Console.WriteLine("======>Searched " + total + " logs");
                if (total > 0)
                {
                    var sort = Builders<BsonDocument>.Sort.Descending("Timestamp");
                    var newestLog = query.Sort(sort).Limit(1).As<LogMessage>().FirstOrDefault();
                    Console.WriteLine("Newest Log is ====>" + newestLog.Message);
                }

                Thread.Sleep(interval);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            Task.Factory.StartNew(log, _tokenKeep.Token);
            return Task.CompletedTask;
        }
    }

    public class LogMessage
    {

        public ObjectId _id { set; get; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Timestamp { set; get; }

        /// <summary>
        /// 消息等级
        /// </summary>
        public LogLevel Level { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { set; get; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string OS { set; get; }

        /// <summary>
        /// 发生IP
        /// </summary>
        public string IP { set; get; }

        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { set; get; }
    }
}
