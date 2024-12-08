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
        Assert.AreEqual(45, parsedDocument["1"]);
    }
    
    [Test]
    public void Revive_SingleFloat()
    {
        // Given 
        string rawDocument = "{\"content\": {\"1\": 45.5}}";

        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.AreEqual(45.5, parsedDocument["1"]);
    }


    [Test]
    public void Revive_SingleString()
    {
        // Given 
        string rawDocument = "{\"content\": {\"1\": \"Gare de Yverdon-les-Bains\"}}";

        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.AreEqual("Gare de Yverdon-les-Bains", parsedDocument["1"]);
    }
    
    [Test]
    public void Revive_ListOfStrings()
    {
        // Given 
        var rawDocument = "{\"content\": {\"1\": [\"Gare de Yverdon-les-Bains\", \"Gare de Lausanne\"]}}";
        
        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.AreEqual(new List<string> {"Gare de Yverdon-les-Bains", "Gare de Lausanne"}, parsedDocument["1"]);
    }
    
    [Test]
    public void Revive_ComplexDocument()
    {
        // Given 
        var rawDocument = "{\"content\": {\"1\": [\"Gare de Yverdon-les-Bains\", \"Gare de Lausanne\"], \"2\": 83, \"3\": 4.5}}";
        
        // When
        var parsedDocument = DocParser.Revive(rawDocument);
        
        // Then
        Assert.AreEqual(new List<string> {"Gare de Yverdon-les-Bains", "Gare de Lausanne"}, parsedDocument["1"]);
        Assert.AreEqual(83, parsedDocument["2"]);
        Assert.AreEqual(4.5, parsedDocument["3"]);
    }
}