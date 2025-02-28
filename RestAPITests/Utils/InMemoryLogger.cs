using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RestAPITests.Utils;

public class InMemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly List<string> _logEntries;
    private readonly ITestOutputHelper _output;

    public InMemoryLogger(string categoryName, List<string> logEntries, ITestOutputHelper output)
    {
        _categoryName = categoryName;
        _logEntries = logEntries;
        _output = output;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var logEntry = $"{logLevel}: {_categoryName} - {formatter(state, exception)}";
        _logEntries.Add(logEntry);
        _output.WriteLine(logEntry);
    }
}