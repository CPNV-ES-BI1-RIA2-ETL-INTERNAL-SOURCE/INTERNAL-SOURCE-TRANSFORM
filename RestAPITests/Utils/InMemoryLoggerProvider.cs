using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RestAPITests.Utils;

/// <summary>
/// A provider for in-memory logging in tests.
/// </summary>
public class InMemoryLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    /// <summary>
    /// The log entries captured by the logger in LogEntry format.
    /// </summary>
    public List<LogEntry> LogEntries { get; private set; } = new List<LogEntry>();

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(categoryName, LogEntries, output);
    }

    public void Dispose()
    {
        // No-op for now
    }
}