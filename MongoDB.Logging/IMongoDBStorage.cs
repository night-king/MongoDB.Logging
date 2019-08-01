using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.Logging
{
    internal interface IMongoDBStorage
    {
        void Write(string connStr, string database, string collection, LogMessageEntry message);
    }
}
