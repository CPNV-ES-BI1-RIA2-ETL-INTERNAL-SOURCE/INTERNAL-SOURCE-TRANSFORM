using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessTransformer.Records;
using Microsoft.AspNetCore.Mvc.Testing;
using RestAPI;

namespace RestAPITests;

/// <summary>
/// End-to-end tests for the REST API.
/// </summary>
public class RestAPIAppTests(WebApplicationFactory<RestAPIApp> factory)
    : IClassFixture<WebApplicationFactory<RestAPIApp>>
{
    private static readonly IEnumerable<string> ValidExampleDocument = new List<string> {
        "Gare de Yverdon-les-Bains", 
        "Départ pour le 9 décembre 2024",
        " Heure de départ        Ligne    Destination         Vias                                              Voie", 
        " 8 00                   IC 5     Lausanne                                                              2", 
        " 16 45                  IC 5     Genève Aéroport     Morges                                            2", 
        " 23 00                  IC 5     Rorschar            Neuchâtel, Biel/Bienne, Olten, St. Gallen         1", 
        " 13 18                  S 30     Fribourg/Freiburg   Yverdon-Champ Pittet, Yvonand, Cheyres, Payerne   3D"
    };

    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsValid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = ValidExampleDocument;

        // Act
        var response = await client.PostAsJsonAsync("/v1/documents/transform", request);

        // Assert (also that it's in JSON format)
        response.EnsureSuccessStatusCode();
        var transformedDocument = await response.Content.ReadFromJsonAsync<TrainStation>();
        Assert.NotNull(transformedDocument);
    }
    
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request =  new List<string> {"Invalid document"};

        // Act
        var response = await client.PostAsJsonAsync("/v1/documents/transform", request);

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

        // Assert
        response.EnsureSuccessStatusCode(); // Ensure the endpoint returns 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var openApiDocument = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(openApiDocument), "OpenAPI document should not be empty");

        // Parse the JSON to ensure it is well-formed
        var document = JsonDocument.Parse(openApiDocument);
        Assert.NotNull(document);
        Assert.True(document.RootElement.TryGetProperty("info", out var info));
        Assert.Equal("Document Transformation API", info.GetProperty("title").GetString());
    }
}