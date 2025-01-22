using DocumentParser;
using Newtonsoft.Json;

namespace DocumentParserTests;

public class DocumentParserTests
{  
    private IDocumentParser _documentParser;
    
    private static string GetTestRawData(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return File.ReadAllText(path);
    }
    
    private static dynamic GetTestData(string fileName)
    {
        return JsonConvert.DeserializeObject(GetTestRawData(fileName))!;
    }

    [SetUp]
    public void Setup()
    {
        _documentParser = new DocumentParser.DocumentParser();
    }
    
    [Test]
    public void Parse_SingleString()
    {
        // Given 
        var input = GetTestRawData("SingleStringInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("SingleStringOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);
        
        
        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput.ToString()));
    }
    
    [Test]
    public void Parse_HeadersWithoutValues()
    {
        // Given 
        var input = GetTestRawData("HeadersWithoutValuesInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("HeadersWithoutValuesOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);
        
        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
    
    [Test]
    public void Parse_HeadersWithValues()
    {
        // Given 
        var input = GetTestRawData("HeadersWithValuesInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("HeadersWithValuesOutput.json"), Formatting.None);

        // When
        var parsedDocument = _documentParser.Parse(input);

        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
    
    [Test]
    public void Parse_CompleteDocument()
    {
        // Given
        var input = GetTestRawData("CompleteDocumentInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("CompleteDocumentOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);

        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithManyMissingValues()
    {
        // Given 
        var input = GetTestRawData("CompleteDocumentWithManyMissingValuesInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("CompleteDocumentWithManyMissingValuesOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);

        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithHeadersReminder()
    {
        // Given 
        var input = GetTestRawData("CompleteDocumentWithHeadersReminderInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("CompleteDocumentWithHeadersReminderOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);

        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
    
    [Test]
    public void Parse_CompleteDocumentWithTableFirst()
    {
        // Given 
        var input = GetTestRawData("CompleteDocumentWithTableFirstInput.txt").Split("\r\n").ToList();
        var expectedOutput = JsonConvert.SerializeObject(GetTestData("CompleteDocumentWithTableFirstOutput.json"), Formatting.None);
        
        // When
        var parsedDocument = _documentParser.Parse(input);

        // Then
        Assert.That(System.Text.Json.JsonSerializer.Serialize(parsedDocument), Is.EqualTo(expectedOutput));
    }
}
