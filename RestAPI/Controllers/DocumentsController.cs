using BusinessTransformer;
using BusinessTransformer.Records;
using CommonInterfaces.Records.DocumentsRelated;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("v1/documents")]
public class DocumentsController(
    IDocumentParser parser,
    IDocumentReviver<DeparturesDocument> reviver,
    IDocumentTransformer<DeparturesDocument, TrainStation> transformer)
    : ControllerBase
{
    [HttpPost("transform")]
    [ProducesResponseType(typeof(TrainStation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public IActionResult TransformDocument([FromBody] List<string> request)
    {
        try
        {
            string parsedDocument = parser.Parse(request);
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