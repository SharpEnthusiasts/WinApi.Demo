using System.Collections.Generic;

namespace WinApi.Demo.Client.HTTP.Model
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
