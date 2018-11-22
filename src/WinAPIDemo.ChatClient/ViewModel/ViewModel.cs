using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinAPIDemo.ChatClient.Model;

namespace WinAPIDemo.ChatClient.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        Chat model = new Chat();

        public string ChatLog
        {
            get { return model.ChatLog; }
            set
            {
                model.ChatLog = value;
                OnPropertyChanged(nameof(ChatLog));
            }
        }

        public string Nickname
        {
            get { return model.Nickname; }
            set
            {
                model.Nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        public string Message
        {
            get { return model.Message; }
            set
            {
                model.Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private ICommand sendMessage;

        public ICommand SendMessage
        {
            get
            {
                if (sendMessage == null)
                    sendMessage = new RelayCommand(
                        (object parameter) =>
                        {
                            //var port = short.Parse(ServerPort);
                            //Output = HttpSendRequestHelper(ServerName, port,
                            //RequestMethod, ServerEndpoint, Headers, Data);
                            //OnPropertyChanged(nameof(Output));
                        },
                        (object parameter) =>
                        {
                            if (string.IsNullOrEmpty(Message))
                                return false;
                            if (string.IsNullOrEmpty(Nickname))
                                return false;
                            return true;
                        });
                return sendMessage;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
