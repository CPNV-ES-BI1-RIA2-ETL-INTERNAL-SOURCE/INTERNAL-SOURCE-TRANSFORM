using CommonInterfaces.DocumentsRelated;
using DocumentParser;

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

        // When
        var RevivedDocument = _documentReviver.Revive(rawDocument);

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
        Assert.That(RevivedDocument, Is.EqualTo(
            new DeparturesDocument("Gare de Yverdon-les-Bains", "9 décembre 2024", new List<Departure> {
                new Departure("Lausanne", new List<string>(), 8, "IC 5", "2", null, new List<string>()),
                new Departure("Genève Aéroport", new List<string> { "Morges" }, 16, "IC 5", "2", null, new List<string>())
            })
        ));
    }
}
