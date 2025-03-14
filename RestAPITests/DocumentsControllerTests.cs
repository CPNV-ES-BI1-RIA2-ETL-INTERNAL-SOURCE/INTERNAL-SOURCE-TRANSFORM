using BusinessTransformer;
using BusinessTransformer.Mapping;
using DocumentParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RestAPI.Controllers;

namespace RestAPITests;

/// <summary>
/// Unit tests for the DocumentsController class.
/// </summary>
public class DocumentsControllerTests
{
    private readonly Mock<IDocumentParser> _mockParser = new();
    private readonly Mock<IMappingTransformer> _mockTransformer = new();
    private readonly Mock<ILogger<DocumentsController>> _mockLogger = new();
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        _controller = new DocumentsController(_mockParser.Object, _mockTransformer.Object, _mockLogger.Object);
        Assert.NotNull(_controller);
    }

    [Fact]
    public void TransformDocument_ShouldLogUnexpectedException()
    {
        // Given
        // Simulate an unexpected exception in transformation
        _mockParser.Setup(p => p.Parse(It.IsAny<List<string>>()))
            .Returns([new { Data = "ValidData" }]);

        _mockTransformer.Setup(t => t.Transform(It.IsAny<List<dynamic>>(), It.IsAny<IEnumerable<FieldMapping<int>>>()))
            .Throws(new Exception("Unexpected error"));

        var request = new List<string> { "ValidData" };

        // When
        var result = _controller.TransformDocument(request);

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
}