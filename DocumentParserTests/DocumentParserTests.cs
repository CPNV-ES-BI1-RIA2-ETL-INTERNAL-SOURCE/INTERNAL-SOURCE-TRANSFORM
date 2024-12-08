using DocParser = DocumentParser.DocumentParser;


namespace DocumentParserTests;

public class DocumentParserTests
{   
    [SetUp]
    public void Setup()
    {
    }
    
    [Test]
    public void Revive_SingleInt()
    {
        // Given 
        string rawDocument = "{\"content\": {\"1\": 45}}";

        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.That(parsedDocument["1"], Is.EqualTo(45));
    }
    
    [Test]
    public void Revive_SingleFloat()
    {
        // Given 
        string rawDocument = "{\"content\": {\"1\": 45.5}}";

        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.That(parsedDocument["1"], Is.EqualTo(45.5));
    }


    [Test]
    public void Revive_SingleString()
    {
        // Given 
        string rawDocument = "{\"content\": {\"1\": \"Gare de Yverdon-les-Bains\"}}";

        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.That(parsedDocument["1"], Is.EqualTo("Gare de Yverdon-les-Bains"));
    }
    
    [Test]
    public void Revive_ListOfStrings()
    {
        // Given 
        var rawDocument = "{\"content\": {\"1\": [\"Gare de Yverdon-les-Bains\", \"Gare de Lausanne\"]}}";
        
        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.That(parsedDocument["1"], Is.EqualTo(new List<string> {"Gare de Yverdon-les-Bains", "Gare de Lausanne"}));
    }
    
    [Test]
    public void Revive_ComplexDocument()
    {
        // Given 
        var rawDocument = "{\"content\": {\"1\": [\"Gare de Yverdon-les-Bains\", \"Gare de Lausanne\"], \"2\": 83, \"3\": 4.5}}";
        
        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.That(parsedDocument["1"], Is.EqualTo(new List<string> {"Gare de Yverdon-les-Bains", "Gare de Lausanne"}));
        Assert.That(parsedDocument["2"], Is.EqualTo(83));
        Assert.That(parsedDocument["3"], Is.EqualTo(4.5));
    }
}