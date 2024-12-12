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
        string rawDocument = "Gare de Yverdon-les-Bains";

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[\"Gare de Yverdon-les-Bains\"]"));
    }
    
    [Test]
    public void Parse_HeadersWithoutValues()
    {
        // Given 
        string rawDocument = "Heure de départ        Ligne    Destination         Vias                                              Voie\n";

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[]"));
    }
    
    [Test]
    public void Parse_HeadersWithValues()
    {
        // Given 
        string rawDocument = " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                   IC 5     Lausanne                                                              2\n" +
                             " 16 45                  IC 5     Genève Aéroport     Morges                                            2";

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}]]"));
    }
    
    [Test]
    public void Parse_CompleteDocument()
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
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[\"Gare de Yverdon-les-Bains\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}],\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\"]"));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithManyMissingValues()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains\n" +
                             " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                            Lausanne                                                               \n" +
                             "                        IC 5     Genève Aéroport     Morges                                            2\n" +
                             "\n" +
                             "\n" +
                             "Départ pour le 9 décembre 2024\n";

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[\"Gare de Yverdon-les-Bains\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"\"},{\"Heure de d\\u00E9part\":\"\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}],\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\"]"));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithHeadersReminder()
    {
        // Given 
        string rawDocument = "Gare de Yverdon-les-Bains\n" +
                             " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 8 00                   IC 5     Lausanne                                                              2\n" +
                             " Heure de départ        Ligne    Destination         Vias                                              Voie\n" +
                             " 16 45                  IC 5     Genève Aéroport     Morges                                            2\n" +
                             "\n" +
                             "\n" +
                             "Départ pour le 9 décembre 2024\n";

        // When
        var parsedDocument = _documentParser.Parse(rawDocument);

        // Then
        Assert.That(parsedDocument,Is.EqualTo("[\"Gare de Yverdon-les-Bains\",[{\"Heure de d\\u00E9part\":\"8 00\",\"Ligne\":\"IC 5\",\"Destination\":\"Lausanne\",\"Vias\":\"\",\"Voie\":\"2\"},{\"Heure de d\\u00E9part\":\"16 45\",\"Ligne\":\"IC 5\",\"Destination\":\"Gen\\u00E8ve A\\u00E9roport\",\"Vias\":\"Morges\",\"Voie\":\"2\"}],\"D\\u00E9part pour le 9 d\\u00E9cembre 2024\"]"));
    }
}
