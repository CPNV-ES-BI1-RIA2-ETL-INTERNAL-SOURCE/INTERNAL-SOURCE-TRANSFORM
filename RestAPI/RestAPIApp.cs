using BusinessTransformer;
using BusinessTransformer.Records;
using CommonInterfaces.Records.DocumentsRelated;
using DocumentParser;
using Microsoft.OpenApi.Models;

namespace RestAPI;

public class RestAPIApp
{
    private static readonly OpenApiInfo ApiInfo = new()
    {
        Title = "Document Transformation API",
        Version = "v1",
        Description = "An API to parse from text and transform documents into business objects based on dynamic schema mapping.",
        Contact = new OpenApiContact
        {
            Name = "Eliott Jaquier, Cyprien Jaquier",
            Email = "eliott.jaquier@eduvaud.ch",
            Url = new Uri("https://github.com/CPNV-ES-BI1-RIA2-ETL-INTERNAL-SOURCE/INTERNAL-SOURCE-TRANSFORM/issues")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    };
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add MVC and Swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", ApiInfo);
        });

        // Register your dependencies (if needed)
        builder.Services.AddScoped<IStringManipulator, StandardLibStringManipulator>();
        builder.Services.AddScoped<IDocumentParser, DocumentParser.DocumentParser>();
        builder.Services.AddScoped<IDocumentReviver<DeparturesDocument>, DeparturesDocumentReviver>();
        builder.Services.AddScoped<IDocumentTransformer<DeparturesDocument, TrainStation>, DepartureDocumentTransformer>();
        builder.Services.AddScoped<StandardLibStringManipulator>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers(); // Use controllers for routing

        app.Run();
    }
}