using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAPIDemo.ClientHTTP.Model;

namespace WinAPIDemo.ClientHTTP.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        HttpRequest model = new HttpRequest();

        public string ServerName
        {
            get
            {
                return model.ServerName;
            }
            set
            {
                model.ServerName = value;
                OnPropertyChanged(nameof(ServerName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
