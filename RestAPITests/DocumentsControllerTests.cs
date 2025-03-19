using BusinessTransformer;
using BusinessTransformer.Mapping;
using DocumentParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RestAPI.Controllers;
using RestAPI.DTOs;
using RestAPITests.Utils;

namespace RestAPITests;

/// <summary>
/// Unit tests for the DocumentsController class.
/// </summary>
public class DocumentsControllerTests
{
    private readonly Mock<IDocumentParser> _mockParser = new();
    private readonly Mock<IMappingTransformer> _mockTransformer = new();
    private readonly Mock<ILogger<DocumentsController>> _mockLogger = new();
    
    private static readonly List<string> invalidDocument = new() { "Invalid document" };
    private static readonly TransformRequest invalidDocumentRequest = new() { Document = invalidDocument, Mapping = TestUtils.GetTestData("Mapping.json") };


    [Fact]
    public void TransformDocument_ShouldLogUnexpectedException()
    {
        // Given
        var controller = new DocumentsController(_mockParser.Object, _mockTransformer.Object, _mockLogger.Object);
        Assert.NotNull(controller);
        
        // Mocks to Simulate an unexpected exception in transformation
        _mockParser.Setup(p => p.Parse(It.IsAny<List<string>>()))
            .Returns([new { Data = "ValidData" }]);

        _mockTransformer.Setup(t => t.Transform(It.IsAny<List<dynamic>>(), It.IsAny<IEnumerable<FieldMapping<int>>>()))
            .Throws(new Exception("Unexpected error"));

        var request = invalidDocumentRequest;

        // When
        var result = controller.TransformDocument(request);

        // Then
        
        // Ensure a 500 Internal Server Error response
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        // Verify logger captured the Exception
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unexpected error occurred")),
                It.Is<Exception>(ex => ex.Message == "Unexpected error"),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
    
    [Fact]
    public void TransformDocument_ShouldLogTotalTimeTaken()
    {
        // Given
        var parser = new DocumentParser.DocumentParser();
        var transformer = new JsonMappingTransformer(new StandardLibStringManipulator());
        var controller = new DocumentsController(parser, transformer, _mockLogger.Object);
        
        var request = TestUtils.CreateRequestFromFiles("SimpleInput.txt", "Mapping.json");
        var expectedOutput = TestUtils.GetTestData("SimpleOutput.json");
        
        // When
        var result = controller.TransformDocument(request);

        // Then
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedOutput.ToString(), okResult.Value);

        // Verify that logging captured the time taken
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Document transformation completed successfully.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
    
    [Fact]
    public void TransformDocument_ShouldReturnBadRequest_AndLogWarning_WhenInputIsInvalid()
    {
        // Given
        var parser = new DocumentParser.DocumentParser();
        var transformer = new JsonMappingTransformer(new StandardLibStringManipulator());
        var controller = new DocumentsController(parser, transformer, _mockLogger.Object);
        
        var request = invalidDocumentRequest;

        // When
        var result = controller.TransformDocument(request);

        // Then
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        //  Verify that logging captured warning about invalid format
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Document transformation failed due to invalid format.")),
                It.IsAny<FormatException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}