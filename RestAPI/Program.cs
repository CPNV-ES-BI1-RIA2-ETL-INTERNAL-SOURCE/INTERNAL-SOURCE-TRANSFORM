using BusinessTransformer;
using DocumentParser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    var parser = new DocumentParser.DocumentParser();
    var reviver = new DeparturesDocumentReviver();
    var transformer = new DepartureDocumentTransformer();
    
    var parsedDocument = parser.Parse(request.content);
    var departuresDocument = reviver.Revive(parsedDocument);
    var transformedDocument = transformer.Transform(departuresDocument);
    
    return Results.Ok(transformedDocument);
})
.WithName("TransformDocument")
.WithOpenApi();

app.Run();

public class DocumentTransformRequest
{
    public string content { get; set; }  // The document content as a string
}