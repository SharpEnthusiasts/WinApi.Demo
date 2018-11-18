using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinAPIDemo.ClientHTTP.Model
{
    public class HttpRequest
    {
        public string Output { get; set; }
        public string ServerName { get; set; }
        public string ServerPort { get; set; }
        public string ServerEndpoint { get; set; }   
        public string Data { get; set; }
        public string Headers { get; set; }
        public string RequestMethod { get; set; }
        public IReadOnlyList<string> RequestMethods { get; } = new List<string>
        {
            "GET",
            "POST",
            "PUT",
            "DELETE"
        };
    }
}
