using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Tetherfi;

public class Program
{
    public static void Main(string[] args)
    {
        //Read Configuration from appSettings    
        var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.Development.json").Build();
        //Initialize Logger    
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
        try
        {
            Log.Information("Application Starting...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The Application failed to start.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).UseSerilog();
}
