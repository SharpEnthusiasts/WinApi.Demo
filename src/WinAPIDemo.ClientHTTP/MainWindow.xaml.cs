using Newtonsoft.Json;
using System.Windows;
using WinAPIDemo.Models;
using static WinAPIDemo.ClientHTTP.Helpers.HttpRequestHelpers;

namespace WinAPIDemo.ClientHTTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Get_All_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = "";
            TextBlock.Text = HttpSendRequestHelper("https://localhost:5001/api/todo");
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {          
            var item = new ToDoItem
            {
                Title = "Clean up house",
                IsCompleted = false
            };
            var json = JsonConvert.SerializeObject(item);


            TextBlock.Text = "";
            var serverName = ServerName.Text;
            if (short.TryParse(ServerPort.Text, out var serverPort) == false)
            {
                TextBlock.Text = "INVALID PORT!";
                return;
            }
            var serverEndpoint = ServerEndpoint.Text;
            var requestMethod = RequestMethod.Text;
            var headers = Headers.Text;
            var data = Data.Text;
            if(data.Equals("sample"))
            {
                data = json;
            }
            TextBlock.Text = HttpSendRequestHelper(serverName, serverPort, requestMethod, serverEndpoint, headers, data);
        }     
    }
}
