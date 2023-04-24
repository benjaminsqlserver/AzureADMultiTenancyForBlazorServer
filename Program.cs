using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzen;
using Microsoft.EntityFrameworkCore;
using SimplifiedNorthwind;

public partial class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseKestrel();
                   webBuilder.UseUrls("https://localhost:5001", "https://localhost:5002");//urls for 2 tenants
                   webBuilder.UseStartup<Startup>();
               });
}