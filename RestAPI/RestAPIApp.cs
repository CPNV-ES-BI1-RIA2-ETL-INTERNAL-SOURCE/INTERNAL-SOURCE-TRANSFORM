using System.Text.Json.Nodes;
using BusinessTransformer;
using DocumentParser;

public class RestAPIApp
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add MVC and Swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register your dependencies (if needed)
        builder.Services.AddScoped<IStringManipulator, StandardLibStringManipulator>();
        builder.Services.AddScoped<IDocumentParser, DocumentParser.DocumentParser>();
        builder.Services.AddScoped<IMappingTransformer, JsonMappingTransformer>();
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