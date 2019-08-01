using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace MongoDB.Logging
{
    public static class MongoDBLoggerExtensions
    {
        /// <summary>
        /// Adds a console logger named 'Console' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddMongoDB(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MongoDBLoggerProvider>());
            return builder;
        }

        /// <summary>
        /// Adds a  logger named 'MongoDB' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">A delegate to configure the <see cref="MongoDBLoggerOptions"/>.</param>
        public static ILoggingBuilder AddMongoDB(this ILoggingBuilder builder, Action<MongoDBLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            builder.AddMongoDB();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
