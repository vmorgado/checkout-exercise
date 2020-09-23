using System;
using Microsoft.Extensions.Logging;

namespace dotnetexample.Logging
{
    public interface ILoggerService
    {
        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception = null, Func<TState, System.Exception, string> formatter = null);
    }
}