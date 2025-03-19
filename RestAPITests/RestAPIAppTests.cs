using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessTransformer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestAPI;
using RestAPITests.Utils;
using Xunit.Abstractions;

namespace RestAPITests;

/// <summary>
/// End-to-end tests for the REST API.
/// </summary>
public class RestAPIAppTests : IClassFixture<WebApplicationFactory<RestAPIApp>>
{
    private readonly WebApplicationFactory<RestAPIApp> _factory;
    private readonly InMemoryLoggerProvider? _loggerProvider;
    private readonly List<LogEntry> _logEntries;

    public RestAPIAppTests(WebApplicationFactory<RestAPIApp> factory, ITestOutputHelper output)
    {
        _factory = factory.WithTestLogging(output);
        _loggerProvider = _factory.Services.GetRequiredService<ILoggerProvider>() as InMemoryLoggerProvider;
        Assert.NotNull(_loggerProvider);
        _logEntries = _loggerProvider.LogEntries;
        Assert.NotNull(_logEntries);
    }

    private static string GetTestRawData(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return File.ReadAllText(path);
    }
    
    private static dynamic GetTestData(string fileName)
    {
        return JsonConvert.DeserializeObject(GetTestRawData(fileName))!;
    }
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var input = TestUtils.GetTestRawData("SimpleInput.txt").Split("\n").ToList();
        var expectedOutput = TestUtils.GetTestData("SimpleOutput.json");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/documents/transform", input);

        // Assert (also that it's in JSON format)
        response.EnsureSuccessStatusCode();
        var transformedDocument = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedOutput.ToString(), transformedDocument);
        Assert.Contains(_logEntries, log => log is { LogLevel: LogLevel.Information, CategoryName: "RestAPI.Controllers.DocumentsController", Exception: null });
        Assert.DoesNotContain(_logEntries, log => log.LogLevel is LogLevel.Error or LogLevel.Critical);
    }
    
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsInvalid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new List<string> {"Invalid document"};

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/documents/transform", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(_logEntries, log => log is { LogLevel: LogLevel.Warning, Exception: InvalidInputFormatException });
        Assert.DoesNotContain(_logEntries, log => log.LogLevel is LogLevel.Error or LogLevel.Critical);
    }
    
    
    [Fact]
    public async Task Get_OpenApiEndpoint_ShouldReturnValidOpenApiDocument()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var expectedOutput = TestUtils.GetTestRawData("OpenApiEndpointOutput.json");

        // Assert
        response.EnsureSuccessStatusCode(); // Ensure the endpoint returns 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var openApiDocument = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(openApiDocument), "OpenAPI document should not be empty");

        // Parse the JSON to ensure it is well-formed
        var document = JsonDocument.Parse(openApiDocument);
        Assert.NotNull(document);
        Assert.True(document.RootElement.TryGetProperty("info", out var info));
        Assert.Equal(expectedOutput, info.GetProperty("title").GetString());
    }
}