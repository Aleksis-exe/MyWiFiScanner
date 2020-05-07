using MyWiFiScanner.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyWiFiScanner.ViewModels
{
    public  class MyCollectionAccessPointsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Моя коллекция точек доступа
        /// </summary>
        private List<MyAccessPoint> _MyAccessPoints { set; get; }

        public List<MyAccessPoint> MyAccessPoints
        {
            get { return _MyAccessPoints; }
            set
            {
                _MyAccessPoints = value;
                OnPropertyChanged("MyAccessPoints");
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
