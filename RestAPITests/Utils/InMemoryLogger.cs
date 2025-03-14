using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RestAPITests.Utils;

/// <summary>
/// A test utility to capture log entries in memory.
/// </summary>
/// <param name="categoryName">The category name of the logger provided by the application.</param>
/// <param name="logEntries">The reference list of log entries to store the log entries.</param>
/// <param name="output">The test output helper to output the log entries for debugging.</param>
public class InMemoryLogger(string categoryName, List<LogEntry> logEntries, ITestOutputHelper output)
    : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var logEntry = new LogEntry
        {
            LogLevel = logLevel,
            CategoryName = categoryName,
            EventId = eventId,
            State = state,
            Exception = exception,
            FormattedMessage = formatter(state, exception)
        };

        logEntries.Add(logEntry);

        // Optionally output the log to the test output for debugging
        output.WriteLine($"{logLevel}: {categoryName} - {logEntry.FormattedMessage}");
    }
}