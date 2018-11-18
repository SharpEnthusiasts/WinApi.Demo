using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WinAPIDemo.API.Models;

namespace WinAPIDemo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            ToDoInit.SeedAsync(host.Services).Wait();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
