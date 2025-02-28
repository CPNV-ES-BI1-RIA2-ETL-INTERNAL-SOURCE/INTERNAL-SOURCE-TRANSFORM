using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RestAPITests.Utils;

/// <summary>
/// Add extensions to the WebApplicationFactory class.
/// Based on https://gist.github.com/danielcrenna/68285f4187d0f7e5eff325f5014ee0e1#file-webapplicationfactoryextensions-cs
/// </summary>
internal static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<TStartup> WithTestLogging<TStartup>(this WebApplicationFactory<TStartup> factory, ITestOutputHelper output) where TStartup : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.Services.AddSingleton<ILoggerProvider>(new InMemoryLoggerProvider(output));
            });
        });
    }
}