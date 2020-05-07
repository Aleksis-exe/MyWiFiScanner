using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyWiFiScanner.Models
{
    /// <summary>
    ///точека доступа
    /// </summary>
    public class MyAccessPoint : INotifyPropertyChanged
    {
        private string _Ssid { set; get; }
        private uint _SignalStrength { set; get; }

        public string Ssid
        {
            get
            {
                return _Ssid;
            }
            set
            {
                _Ssid = value;
                OnPropertyChanged("Ssid");
            }
        }

        /// <summary>
        /// сила сигнала
        /// </summary>
        public uint SignalStrength {
            get
            {
                return _SignalStrength;
            }
            set
            {
                _SignalStrength = value;
                OnPropertyChanged("SignalStrength");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
