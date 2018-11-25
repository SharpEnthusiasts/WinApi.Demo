using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WinAPIDemo.ChatClient.Common;
using WinAPIDemo.ChatClient.Model;

namespace WinAPIDemo.ChatClient.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Chat model;
        private Server server;
        private Client client;
        private MessageHandler messageHandler;
        private ConnectionHandler connectionHandler;

        public ViewModel()
        {
            model = new Chat();
            server = new Server();
        }

        private void MessageRecieved(object sender, string e)
        {
            ChatLog += $"{e}\n";
        }

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

        public string IPAddress
        {
            get { return server.IPAddress; }
            set
            {
                server.IPAddress = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        public string Port
        {
            get { return server.Port; }
            set
            {
                server.Port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private ICommand sendMessage;
        private ICommand connect;
        private ICommand disconnect;

        public ICommand SendMessage
        {
            get
            {
                if (sendMessage == null)
                    sendMessage = new RelayCommand(
                        (object parameter) =>
                        {
                            string time = DateTime.Now.ToLocalTime().ToShortTimeString();
                            string message = $"{time} {Nickname}: {Message}";
                            messageHandler.Send(message).Wait();
                            Message = string.Empty;
                            OnPropertyChanged(nameof(ChatLog));
                            OnPropertyChanged(nameof(Message));
                        },
                        (object parameter) =>
                        {
                            if (client == null)
                                return false;
                            if (client != null)
                                if (client.Socket == IntPtr.Zero)
                                    return false;
                            if (string.IsNullOrEmpty(Nickname))
                                return false;
                            return true;
                        });
                return sendMessage;
            }
        }

        public ICommand Connect
        {
            get
            {
                if (connect == null)
                    connect = new RelayCommand(
                        (object parameter) =>
                        {
                            connectionHandler = new ConnectionHandler();
                            client = new Client(connectionHandler.Connect(IPAddress, Port));
                            messageHandler = new MessageHandler(client);
                            messageHandler.MessageRecieved += MessageRecieved;
                            Task.Run(() => messageHandler.Handle());
                        },
                        (object parameter) =>
                        {
                            if (client != null)
                                if(client.Socket != IntPtr.Zero)
                                    return false;
                            if (System.Net.IPAddress.TryParse(IPAddress, out var address) == false)
                                return false;
                            if (ushort.TryParse(Port, out var port) == false)
                                return false;                                
                            return true;
                        });
                return connect;
            }
        }
        public ICommand Disconnect
        {
            get
            {
                if (disconnect == null)
                    disconnect = new RelayCommand(
                        (object parameter) =>
                        {
                            connectionHandler.Disconnect(client);
                            client = null;
                        },
                        (object parameter) =>
                        {
                            if (client != null)
                                if (client.Socket != IntPtr.Zero)
                                    return true;
                            return false;
                        });
                return disconnect;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
