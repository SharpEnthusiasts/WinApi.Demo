using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinAPIDemo.Models;

namespace WinAPIDemo.API.Models
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