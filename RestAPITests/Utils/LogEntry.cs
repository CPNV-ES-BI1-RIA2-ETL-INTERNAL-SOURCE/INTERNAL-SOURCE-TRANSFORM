using Microsoft.Extensions.Logging;

namespace RestAPITests.Utils;

/// <summary>
/// A log entry to capture log information.
/// Used in conjunction with InMemoryLogger and InMemoryLoggerProvider.
/// </summary>
public class LogEntry
{
    public LogLevel LogLevel { get; set; }
    public string CategoryName { get; set; }
    public EventId EventId { get; set; }
    public object State { get; set; }
    public Exception Exception { get; set; }
    public string FormattedMessage { get; set; }
}