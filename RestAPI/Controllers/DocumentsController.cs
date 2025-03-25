using System.Diagnostics;
using BusinessTransformer;
using BusinessTransformer.Mapping;
using DocumentParser;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.DTOs;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/v2/documents")]
public class DocumentsController(
    IDocumentParser parser,
    IMappingTransformer transformer,
    ILogger<DocumentsController> logger)
    : ControllerBase
{
    [HttpPost("transform")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public IActionResult TransformDocument([FromBody] TransformRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            List<dynamic> parsedDocument = parser.Parse(request.Document);
            
            dynamic mapping = request.Mapping;
            dynamic transformedDocument = transformer.Transform(parsedDocument, FieldMapping<int>.FromJArray(mapping));

            stopwatch.Stop();

            // Log the processed document and the time taken
            logger.LogInformation(
                "Document transformation completed successfully. Time taken: {ElapsedTime}",
                stopwatch.Elapsed
            );

            return Ok(transformedDocument.ToString());
        }
        catch (FormatException ex)
        {
            // Return a 400 Bad Request if there is a format exception
            stopwatch.Stop();

            logger.LogWarning(
                ex,
                "Document transformation failed due to invalid format. Request: {Request}. Time taken: {ElapsedTime}",
                request,
                stopwatch.Elapsed
            );
            return BadRequest(new { error = $"Invalid document format: {ex.Message}" });
        }
        catch (Exception ex)
        {
            // Catch any other unexpected errors
            stopwatch.Stop();

            logger.LogError(
                ex,
                "An unexpected error occurred during document transformation. Request: {Request}. Time taken: {ElapsedTime}",
                request,
                stopwatch.Elapsed
            );
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }
}