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
        var jsonDocument = "[\"Gare de Yverdon-les-Bains\"]";
        
        // Then
        Assert.Throws<FormatException>(() => _documentReviver.Revive(jsonDocument));
    }
    
    [Test]
    public void Revive_CompleteDocument()
    {
        // Given 
        var jsonDocument = "[\"Gare de Yverdon-les-Bains\",\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\" ,[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]";

        // When
        var RevivedDocument = _documentReviver.Revive(jsonDocument);

        // Then
        var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "Départ pour le 9 décembre 2024", new List<Departure>
        {
            new Departure("Lausanne", "", "8 00", "IC 5", "2"),
            new Departure("Genève Aéroport", "Morges", "16 45", "IC 5", "2")
        });
        
        Assert.That(JsonSerializer.Serialize(RevivedDocument), Is.EqualTo(JsonSerializer.Serialize(departuresDocument)));
    }
}
