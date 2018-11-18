using System;
using System.Collections.Generic;
using System.Text;

namespace WinAPIDemo.Models
{
    public class ToDoItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
