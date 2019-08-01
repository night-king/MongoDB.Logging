using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Logging
{
    public class MongoDBLoggerOptions
    {
        /// <summary>
        /// MongoDB数据库链接字符串
        /// </summary>
        public string Connstr { set; get; }

        /// <summary>
        /// MongoDB Database Name
        /// </summary>
        public string Database { set; get; }

        /// <summary>
        /// Collection Name
        /// </summary>
        public string Collection { set; get; }

    }
}
