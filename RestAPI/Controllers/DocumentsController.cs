using System.Text.Json.Nodes;
using BusinessTransformer;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers;

[ApiController]
[Route("v1/documents")]
public class DocumentsController(
    IDocumentParser parser,
    IMappingTransformer transformer)
    : ControllerBase
{
    [HttpPost("transform")]
    public IActionResult TransformDocument([FromBody] List<string> request)
    {
        try
        {
            string parsedDocument = parser.Parse(request);
            //TODO : Parser should return a JsonArray instead of a string
            JsonArray rivided = JsonNode.Parse(parsedDocument).AsArray();
            
            //TODO : Mapping should be taken from request body
            JsonArray mapping = JsonNode.Parse(System.IO.File.ReadAllText("config.json")).AsArray();
            JsonArray transformedDocument = transformer.Transform(rivided, mapping);

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