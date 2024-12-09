using DocParser = DocumentParser.DocumentParser;

namespace DocumentParserTests;

public class DocumentParserTests
{  
    [Test]
    public void Revive_SingleString()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains";

        // When
        var parsedDocument = DocParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\"]"));
    }
    
    [Test]
    public void Revive_HeadersWithoutValues()
    {
        // Given 
        string rawDocument = "Heure de départ        Ligne    Destination         Vias                                              Voie\n";

        // When
        var parsedDocument = DocParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[]"));
    }
    
    [Test]
    public void Revive_HeadersWithValues()
    {
        // Given 
        string rawDocument = " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                   IC 5     Lausanne                                                              2\n" +
                             " 16 45                  IC 5     Genève Aéroport     Morges                                            2";

        // When
        var parsedDocument = DocParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[[{\"Heure de départ\": \"8 00\", \"Ligne\": \"IC 5\", \"Destination\": \"Lausanne\", \"Vias\": \"\", \"Voie\": \"2\"}, {\"Heure de départ\": \"16 45\", \"Ligne\": \"IC 5\", \"Destination\": \"Genève Aéroport\", \"Vias\": \"Morges\", \"Voie\": \"2\"}]]"));
    }
    
    [Test]
    public void Revive_CompleteDocument()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains" +
                             " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                   IC 5     Lausanne                                                              2\n" +
                             " 16 45                  IC 5     Genève Aéroport     Morges                                            2\n" +
                             " \n" +
                             " \n" +
                             "Départ pour le 9 décembre 2024\n";

        // When
        var parsedDocument = DocParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\",[{\"Heure de départ\": \"8 00\", \"Ligne\": \"IC 5\", \"Destination\": \"Lausanne\", \"Vias\": \"\", \"Voie\": \"2\"}, {\"Heure de départ\": \"16 45\", \"Ligne\": \"IC 5\", \"Destination\": \"Genève Aéroport\", \"Vias\": \"Morges\", \"Voie\": \"2\"}], \"Départ pour le 9 décembre 2024\"]"));
    }
}
