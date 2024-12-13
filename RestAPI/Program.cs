using BusinessTransformer;
using DocumentParser;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // TODO : Should we use POST /documents/{id}/transform with id is path to bucket instead ?
        app.MapPost("/documents/transform", async (DocumentTransformRequest request) =>
        {
            try
            {
                var parser = new DocumentParser.DocumentParser();
                var reviver = new DeparturesDocumentReviver();
                var transformer = new DepartureDocumentTransformer(new StandardLibStringManipulator());

                var parsedDocument = parser.Parse(request.content);
                var departuresDocument = reviver.Revive(parsedDocument);
                var transformedDocument = transformer.Transform(departuresDocument);

                return Results.Json(transformedDocument);
            }
            catch (FormatException ex)
            {
                // Return a 400 Bad Request if there is a format exception
                return Results.BadRequest($"Invalid document format: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                return Results.StatusCode(500);
            }
        })
        .WithName("TransformDocument")
        .WithOpenApi();

        app.Run();
    }

    public class DocumentTransformRequest
    {
        public string content { get; set; } // The document content as a string
    }
}