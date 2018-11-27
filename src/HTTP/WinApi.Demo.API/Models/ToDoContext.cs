using Microsoft.EntityFrameworkCore;
using WinApi.Demo.Models;

namespace WinApi.Demo.API.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) 
            : base(options)
        {

        }

        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}