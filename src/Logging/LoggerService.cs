using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetexample.Logging
{
    public class LoggerService : ILoggerService
    {
        private readonly IMongoCollection<LogEntry> _logCollection;
        public LoggerService(IMongoCollection<LogEntry> collection) {
            _logCollection = collection;
        }

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception = null, Func<TState, System.Exception, string> formatter = null)
        {   

            var logType = nameof(TState);
            
            var logEntry = new LogEntry {
                level = logLevel.ToString(),
                traceId = eventId.Name,
                payload = state,
                logType = logType
            };
            await _logCollection.InsertOneAsync(logEntry);
        }
        
    }
}
