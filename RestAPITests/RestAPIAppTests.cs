using System.Net;
using System.Net.Http.Json;
using BusinessTransformer.Records;
using Microsoft.AspNetCore.Mvc.Testing;
using RestAPI.Controllers;

namespace RestAPITests;

/// <summary>
/// End-to-end tests for the REST API.
/// </summary>
public class RestAPIAppTests(WebApplicationFactory<RestAPIApp> factory)
    : IClassFixture<WebApplicationFactory<RestAPIApp>>
{
    private const string ValidExampleDocument = "Gare de Yverdon-les-Bains\n Heure de départ        Ligne    Destination         Vias                                              Voie\n 8 00                   IC 5     Lausanne                                                              2\n 16 45                  IC 5     Genève Aéroport     Morges                                            2\n 23 00                  IC 5     Rorschar            Neuchâtel, Biel/Bienne, Olten, St. Gallen         1\n 13 18                  S 30     Fribourg/Freiburg   Yverdon-Champ Pittet, Yvonand, Cheyres, Payerne   3D\n\n\n\n\nDépart pour le 9 décembre 2024";

    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsValid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new DocumentTransformRequest
        {
            content = ValidExampleDocument
        };

        // Act
        var response = await client.PostAsJsonAsync("/documents/transform", request);

        // Assert (also that it's in JSON format)
        response.EnsureSuccessStatusCode();
        var transformedDocument = await response.Content.ReadFromJsonAsync<TrainStation>(); // Replace object with your expected type
        Assert.NotNull(transformedDocument);
    }
    
    
    [Fact]
    public async Task Post_DocumentTransform_ShouldReturnTransformedDocument_WhenInputIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new DocumentTransformRequest
        {
            content = "Invalid document"
        };

        // Act
        var response = await client.PostAsJsonAsync("/documents/transform", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}