using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyWiFiScanner.Models;
using MyWiFiScanner.ViewModels;
using SimpleWifi;

namespace MyWiFiScanner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyCollectionAccessPointsViewModel pointsViewModel;
        public MainWindow()
        {
            InitializeComponent();
            pointsViewModel = new MyCollectionAccessPointsViewModel();
            Print += AutoScrollArena;
            DataContext = pointsViewModel;
        }

        private void ScanerWiFi_ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Wifi wifi = new Wifi();
                // get list of access points
                IEnumerable<AccessPoint> accessPoints = wifi.GetAccessPoints();
                ArenaPrint(null);
                ArenaPrint($"найдено точек {accessPoints.Count()}");
                ArenaPrint("-------------------------------------");
                if (accessPoints.Count() > 0)
                {
                    List<MyAccessPoint> bufer = new List<MyAccessPoint>();
                    foreach (var item in accessPoints)
                    {
                        if (item.Name == "")// если сеть скрытая
                        {
                            ArenaPrint($"Скрытая сеть \t- {item.SignalStrength} %");
                            bufer.Add(new Models.MyAccessPoint()
                            {
                                Ssid = "Скрытая сеть",
                                SignalStrength = item.SignalStrength
                            });
                        }
                        else
                        {
                            ArenaPrint($"{item.Name} \t- {item.SignalStrength} %");
                            bufer.Add(new Models.MyAccessPoint()
                            {
                                Ssid = item.Name,
                                SignalStrength = item.SignalStrength
                            });
                        }
                    }
                    pointsViewModel.MyAccessPoints = bufer;
                }
            }
            catch (Exception exc)
            {
                ArenaPrint(exc.Message, true);
            }
        }

        #region helpers
        public void ArenaPrint(string message, bool red = false)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = message;
            textBlock.TextWrapping = TextWrapping.Wrap;
            if (red)
            {
                textBlock.Foreground = Brushes.Red;
                textBlock.FontWeight = FontWeights.Bold;
            }
            arena.Children.Add(textBlock);
            Print.Invoke();
        }

        public delegate void ArenaPrintHandler();

        /// <summary>
        /// Происходит при выводе информации на arena 
        /// </summary>
        public event ArenaPrintHandler Print;

        /// <summary>
        /// Прокручиваем arenaScrollViewe в конец окна 
        /// </summary>
        private void AutoScrollArena()
        {
            arenaScrollViewe.ScrollToEnd();
        }

        #endregion
    }

}

