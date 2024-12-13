using BusinessTransformer;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("documents")]
public class DocumentsController(
    DocumentParser.DocumentParser parser,
    DeparturesDocumentReviver reviver,
    DepartureDocumentTransformer transformer)
    : ControllerBase
{
    [HttpPost("transform")]
    public IActionResult TransformDocument([FromBody] DocumentTransformRequest request)
    {
        try
        {
            var parsedDocument = parser.Parse(request.content);
            var departuresDocument = reviver.Revive(parsedDocument);
            var transformedDocument = transformer.Transform(departuresDocument);

            return Ok(transformedDocument);
        }
        catch (FormatException ex)
        {
            // Return a 400 Bad Request if there is a format exception
            return BadRequest(new { error = $"Invalid document format: {ex.Message}" });
        }
        catch (Exception)
        {
            // Catch any other unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }
}

public class DocumentTransformRequest
{
    public string content { get; set; } // The document content as a string
}