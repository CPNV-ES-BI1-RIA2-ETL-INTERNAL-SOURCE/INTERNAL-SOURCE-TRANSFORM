using CommonInterfaces.DocumentsRelated;
using DocumentParser;
using System.Text.Json;
namespace DocumentParserTests;

public class DeparturesDocumentReviverTests
{  
    private IDocumentReviver<DeparturesDocument> _documentReviver;

    [SetUp]
    public void Setup()
    {
        _documentReviver = new DeparturesDocumentReviver();
    }
    
    [Test]
    public void Revive_WrongFormat()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains";
        
        // Then
        Assert.Throws<FormatException>(() => _documentReviver.Revive(rawDocument));
    }
    
    [Test]
    public void Revive_CompleteDocument()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains\n" +
                             " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                   IC 5     Lausanne                                                              2\n" +
                             " 16 45                  IC 5     Genève Aéroport     Morges                                            2\n" +
                             "\n" +
                             "\n" +
                             "Départ pour le 9 décembre 2024\n";

        // When
        var RevivedDocument = _documentReviver.Revive(rawDocument);

        // Then
        var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "Départ pour le 9 décembre 2024", new List<Departure>
        {
            new Departure("Lausanne", new List<string>(), "8 00", "IC 5", "2"),
            new Departure("Genève Aéroport", new List<string> { "Morges" }, "16 45", "IC 5", "2")
        });
        
        Assert.That(JsonSerializer.Serialize(RevivedDocument), Is.EqualTo(JsonSerializer.Serialize(departuresDocument)));
    }
}
