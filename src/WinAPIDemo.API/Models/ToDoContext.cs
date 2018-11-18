using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinAPIDemo.Models;

namespace WinAPIDemo.API.Models
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