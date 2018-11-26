using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WinApi.Demo.API.Models;

namespace WinApi.Demo.API
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
