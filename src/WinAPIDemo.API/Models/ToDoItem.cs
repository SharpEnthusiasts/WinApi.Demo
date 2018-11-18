using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPRDemo.API.Models
{
    public class ToDoItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
