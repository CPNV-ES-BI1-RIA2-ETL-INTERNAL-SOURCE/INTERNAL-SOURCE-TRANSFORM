using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RestAPI;
using RestAPI.DTOs;
using RestAPITests.Utils;

namespace RestAPITests;

/// <summary>
/// End-to-end tests for the REST API.
/// </summary>
public class RestAPIAppTests(WebApplicationFactory<RestAPIApp> factory)
    : IClassFixture<WebApplicationFactory<RestAPIApp>>
{
    private const string LogDirectory = "logs";

    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsValid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = TestUtils.SerializeRequestFromFiles("SimpleInput.txt", "Mapping.json");
        var expectedOutput = TestUtils.GetTestData("SimpleOutput.json");

        // Act
        var response = await client.PostAsync("/api/v1/documents/transform", request);

        // Assert (also that it's in JSON format)
        response.EnsureSuccessStatusCode();
        var transformedDocument = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedOutput.ToString(), transformedDocument);
    }
    
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnErrorDocument_WhenInputIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = TestUtils.SerializeInvalidDocumentRequest("Mapping.json");

        // Act
        var response = await client.PostAsync("/api/v1/documents/transform", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    
    [Fact]
    public async Task Get_OpenApiEndpoint_ShouldReturnValidOpenApiDocument()
    {
        // Arrange
        var client = factory.CreateClient();

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
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldCreateLogFile()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = TestUtils.SerializeInvalidDocumentRequest("Mapping.json");

        // Act
        var response = await client.PostAsync("/api/v1/documents/transform", request);

        // Assert (if the request is invalid, a log file should be created with a warning)
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Ensure a log file was created
        Assert.True(Directory.Exists(LogDirectory), "No log folder found");
        var logFiles = Directory.GetFiles(LogDirectory, "*.log");
        Assert.True(logFiles.Any(), "No log file found");
    }
}