using BusinessTransformer;
using DocumentParser;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;

namespace RestAPI;

public class RestAPIApp
{
    private static readonly OpenApiInfo ApiInfo = new()
    {
        Title = "Document Transformation API",
        Version = "v2",
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
        // Read configuration from appsettings.json and environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure Serilog from configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            // Add MVC and Swagger
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.Formatting = Formatting.Indented;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", ApiInfo);
            });

            // Register your dependencies (if needed)
            builder.Services.AddScoped<IStringManipulator, StandardLibStringManipulator>();
            builder.Services.AddScoped<IDocumentParser, DocumentParser.DocumentParser>();
            builder.Services.AddScoped<IMappingTransformer, JsonMappingTransformer>();
            builder.Services.AddScoped<StandardLibStringManipulator>();

            // Add Serilog logging
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseSwagger();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Document Transformation API V2");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers(); // Use controllers for routing

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}