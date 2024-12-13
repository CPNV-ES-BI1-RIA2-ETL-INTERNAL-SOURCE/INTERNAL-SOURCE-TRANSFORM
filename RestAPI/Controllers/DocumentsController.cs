using BusinessTransformer;
using BusinessTransformer.Records;
using CommonInterfaces.DocumentsRelated;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("documents")]
public class DocumentsController(
    IDocumentParser parser,
    IDocumentReviver<DeparturesDocument> reviver,
    IDocumentTransformer<DeparturesDocument, TrainStation> transformer)
    : ControllerBase
{
    [HttpPost("transform")]
    public IActionResult TransformDocument([FromBody] DocumentTransformRequest request)
    {
        try
        {
            string parsedDocument = parser.Parse(request.content);
            DeparturesDocument departuresDocument = reviver.Revive(parsedDocument);
            TrainStation transformedDocument = transformer.Transform(departuresDocument);

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