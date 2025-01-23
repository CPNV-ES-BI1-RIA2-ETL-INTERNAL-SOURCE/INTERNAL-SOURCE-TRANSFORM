using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RestAPI;

namespace RestAPITests;

/// <summary>
/// End-to-end tests for the REST API.
/// </summary>
public class RestAPIAppTests(WebApplicationFactory<RestAPIApp> factory)
    : IClassFixture<WebApplicationFactory<RestAPIApp>>
{
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
        var client = factory.CreateClient();
        var input = GetTestRawData("SimpleInput.txt").Split("\n").ToList();
        var expectedOutput = GetTestData("SimpleOutput.json");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/documents/transform", input);

        // Assert (also that it's in JSON format)
        response.EnsureSuccessStatusCode();
        var transformedDocument = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedOutput.ToString(), transformedDocument);
    }
    
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new List<string> {"Invalid document"};

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/documents/transform", request);

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
        var expectedOutput = GetTestRawData("OpenApiEndpointOutput.json");

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