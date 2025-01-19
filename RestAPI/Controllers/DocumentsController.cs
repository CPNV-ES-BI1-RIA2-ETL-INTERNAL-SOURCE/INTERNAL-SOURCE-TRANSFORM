using System.Text.Json.Nodes;
using BusinessTransformer;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            List<dynamic> parsedDocument = parser.Parse(request);
            
            //TODO : Mapping should be taken from request body
            dynamic mapping = JsonConvert.DeserializeObject(System.IO.File.ReadAllText("config.json"));
            dynamic transformedDocument = transformer.Transform(parsedDocument, mapping);

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