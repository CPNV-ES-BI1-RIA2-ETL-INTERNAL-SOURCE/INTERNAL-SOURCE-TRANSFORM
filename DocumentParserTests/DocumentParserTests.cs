using _documentParser = DocumentParser.DocumentParser;
using IDocumentParser = DocumentParser.IDocumentParser;

namespace DocumentParserTests;

public class DocumentParserTests
{  
    private IDocumentParser _documentParser;

    [SetUp]
    public void Setup()
    {
        _documentParser = new _documentParser();
    }
    
    [Test]
    public void Parse_SingleString()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Gare de Yverdon-les-Bains"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\"]"));
    }
    
    [Test]
    public void Parse_HeadersWithoutValues()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Heure de départ        Ligne    Destination         Vias                                              Voie"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[]"));
    }
    
    [Test]
    public void Parse_HeadersWithValues()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Heure de départ        Ligne    Destination         Vias                                              Voie", 
            "8 00                   IC 5     Lausanne                                                              2", 
            "16 45                  IC 5     Genève Aéroport     Morges                                            2"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]"));
    }
    
    [Test]
    public void Parse_CompleteDocument()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Gare de Yverdon-les-Bains", 
            "Départ pour le 9 décembre 2024",
            "Heure de départ        Ligne    Destination         Vias                                              Voie", 
            "8 00                   IC 5     Lausanne                                                              2", 
            "16 45                  IC 5     Genève Aéroport     Morges                                            2"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\",\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]"));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithManyMissingValues()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Gare de Yverdon-les-Bains", 
            "Départ pour le 9 décembre 2024",
            "Heure de départ        Ligne    Destination         Vias                                              Voie", 
            "8 00                            Lausanne                                                               ", 
            "                       IC 5     Genève Aéroport     Morges                                            2"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\",\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"\"},{\"Heure de d\\u00E9part\":\"\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]"));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithHeadersReminder()
    {
        // Given 
        IEnumerable<string> rawDocument = new List<string> {
            "Gare de Yverdon-les-Bains", 
            "Départ pour le 9 décembre 2024",
            "Heure de départ        Ligne    Destination         Vias                                              Voie", 
            "8 00                   IC 5     Lausanne                                                              2", 
            "Heure de départ        Ligne    Destination         Vias                                              Voie", 
            "16 45                  IC 5     Genève Aéroport     Morges                                            2"
        };

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument, Is.EqualTo("[\"Gare de Yverdon-les-Bains\",\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]"));
    }
}
