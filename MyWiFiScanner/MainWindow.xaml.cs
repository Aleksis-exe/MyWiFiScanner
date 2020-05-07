using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private MyCollectionAccessPointsViewModel pointsViewModel = new MyCollectionAccessPointsViewModel();
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
       
        public MainWindow()
        {
            InitializeComponent();
            Print += AutoScrollArena;
            DataContext = pointsViewModel;
        }

        /// <summary>
        /// Ищем доступные точки доступа wifi 
        /// </summary>
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
                List<MyAccessPoint> bufer = new List<MyAccessPoint>();
                if (accessPoints.Count() > 0)
                {
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
                }
                pointsViewModel.MyAccessPoints = bufer;
            }
            catch (Exception exc)
            {
                pointsViewModel.MyAccessPoints = new List<MyAccessPoint>();
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

        /// <summary>
        /// Сортируем элементы по выбранной колонке 
        /// </summary>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                myTabels.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            myTabels.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
        
        private class SortAdorner : Adorner // Рисуем индикатор сортировки 
        {
            private static Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                    (
                        AdornedElement.RenderSize.Width - 15,
                        (AdornedElement.RenderSize.Height - 5) / 2
                    );

                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }
        }
    }

}

