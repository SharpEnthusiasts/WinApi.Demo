using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using WinApi.Demo.Client.HTTP.Model;
using WinApi.Demo.Models;
using static WinApi.Demo.Client.HTTP.Helpers.HttpRequestHelpers;

namespace WinApi.Demo.Client.HTTP.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        HttpRequest model = new HttpRequest();

        #region ModelBinding

        public string Output
        {
            get { return model.Output; }
            set
            {
                model.Output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        public string ServerName
        {
            get { return model.ServerName; }
            set
            {
                model.ServerName = value;
                OnPropertyChanged(nameof(ServerName));
            }
        }

        public string ServerPort
        {
            get { return model.ServerPort; }
            set
            {
                model.ServerPort = value;
                OnPropertyChanged(nameof(ServerPort));
            }
        }

        public string ServerEndpoint
        {
            get { return model.ServerEndpoint; }
            set
            {
                model.ServerEndpoint = value;
                OnPropertyChanged(nameof(ServerEndpoint));
            }
        }

        public string Data
        {
            get { return model.Data; }
            set
            {
                model.Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public string Headers
        {
            get { return model.Headers; }
            set
            {
                model.Headers = value;
                OnPropertyChanged(nameof(Headers));
            }
        }

        public string RequestMethod
        {
            get { return model.RequestMethod; }
            set
            {
                model.RequestMethod = value;
                OnPropertyChanged(nameof(RequestMethod));
            }
        }

        public IReadOnlyList<string> RequestMethods
        {
            get { return model.RequestMethods; }
        }

        #endregion

        private ICommand executeRequest;
        private ICommand getAllRequest;

        public ICommand ExecuteRequest
        {
            get
            {
                if (executeRequest == null)
                    executeRequest = new RelayCommand(
                        (object parameter) =>
                        {
                            var item = new ToDoItem
                            {
                                Title = "Clean up house",
                                IsCompleted = false
                            };
                            var json = JsonConvert.SerializeObject(item);

                            if (Data.Equals("Sample"))
                            {
                                Data = json;
                            }

                            var port = short.Parse(ServerPort);
                            Output = HttpSendRequestHelper(ServerName, port,
                            RequestMethod, ServerEndpoint, Headers, Data);
                            OnPropertyChanged(nameof(Output));

                        },
                        (object parameter) =>
                        {
                            if (short.TryParse(ServerPort, out var port) == false)
                            {
                                Output = "INVALID PORT";
                                OnPropertyChanged(nameof(Output));
                                return false;
                            }
                            return true;
                        });
                return executeRequest;
            }
        }

        public ICommand GetAllRequest
        {
            get
            {
                if (getAllRequest == null)
                    getAllRequest = new RelayCommand(
                        (object parameter) =>
                        {
                            Output = HttpSendRequestHelper("https://localhost:5001/api/todo");
                            OnPropertyChanged(nameof(Output));
                        },
                        (object parameter) =>
                        {
                            return true;
                        });
                return getAllRequest;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
