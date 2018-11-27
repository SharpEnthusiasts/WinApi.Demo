using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WinApi.Demo.Models;

namespace WinApi.Demo.API.Models
{
    public class ToDoInit
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ToDoContext context = scope.ServiceProvider.GetService<ToDoContext>();

                context.ToDoItems.Add(new ToDoItem
                {
                    Title = "Walk Dog",
                    IsCompleted = false
                });

                context.ToDoItems.Add(new ToDoItem
                {
                    Title = "Feed Cat",
                    IsCompleted = false
                });

                context.ToDoItems.Add(new ToDoItem
                {
                    Title = "Wash Cars",
                    IsCompleted = true
                });

                context.SaveChanges();

                await Task.CompletedTask;
            }
        }
    }
}