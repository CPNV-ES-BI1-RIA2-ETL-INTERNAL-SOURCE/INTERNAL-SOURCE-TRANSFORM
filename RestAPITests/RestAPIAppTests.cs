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
    private static readonly IEnumerable<string> ValidExampleDocument = new List<string> {
        "Gare de Yverdon-les-Bains", 
        " Heure de départ        Ligne    Destination         Vias                                              Voie", 
        " 8 00                   IC 5     Lausanne                                                              2", 
        " 16 45                  IC 5     Genève Aéroport     Morges                                            2", 
        " 23 00                  IC 5     Rorschar            Neuchâtel, Biel/Bienne, Olten, St. Gallen         1", 
        " 13 18                  S 30     Fribourg/Freiburg   Yverdon-Champ Pittet, Yvonand, Cheyres, Payerne   3D", 
        "Départ pour le 9 décembre 2024"
    };

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
            content = new List<string> {"Invalid document"}
        };

        // Act
        var response = await client.PostAsJsonAsync("/documents/transform", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}