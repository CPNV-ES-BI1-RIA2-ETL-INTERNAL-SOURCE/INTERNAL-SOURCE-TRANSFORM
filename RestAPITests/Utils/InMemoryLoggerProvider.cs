using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RestAPITests.Utils;

public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _output;
    private readonly List<string> _logEntries = new List<string>();

    public InMemoryLoggerProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(categoryName, _logEntries, _output);
    }

    public void Dispose()
    {
        // No-op for now
    }

    public List<string> GetLogEntries() => _logEntries;
}