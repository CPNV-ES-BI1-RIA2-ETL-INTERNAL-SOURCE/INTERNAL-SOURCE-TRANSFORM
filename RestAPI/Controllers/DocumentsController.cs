using BusinessTransformer;
using BusinessTransformer.Mapping;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RestAPI.Controllers;

[ApiController]
[Route("v1/documents")]
public class DocumentsController(
    IDocumentParser parser,
    IMappingTransformer transformer)
    : ControllerBase
{
    [HttpPost("transform")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public IActionResult TransformDocument([FromBody] List<string> request)
    {
        try
        {
            List<dynamic> parsedDocument = parser.Parse(request);
            
            //TODO : Mapping should be taken from request body
            dynamic mapping = JsonConvert.DeserializeObject(System.IO.File.ReadAllText("Mapping.json"))!;
            dynamic transformedDocument = transformer.Transform(parsedDocument, FieldMapping<int>.FromJArray(mapping));

            return Ok(transformedDocument.ToString());
        }
        catch (FormatException ex)
        {
            // Return a 400 Bad Request if there is a format exception
            return BadRequest(new { error = $"Invalid document format: {ex.Message}" });
        }
        catch (Exception e)
        {
            // Catch any other unexpected errors
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }
}