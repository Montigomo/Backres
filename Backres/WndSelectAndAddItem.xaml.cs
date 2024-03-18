using Backres.Infrastructure;
using Backres.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Backres
{
    /// <summary>
    /// Interaction logic for WindowsSelectAndAddItem.xaml
    /// </summary>
    public partial class WndSelectAndAddItem : Window
    {
        internal ObservableDictionary<string, BackresItem> SelectedItems { get; set; } = new ObservableDictionary<string, BackresItem>();

        public WndSelectAndAddItem()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {

            try
            {
                dataGridMain.ItemsSource = BackresConfig.Instance.CommonItems;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error - :{ex.Message}");
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in dataGridMain.SelectedItems)
            {
                SelectedItems.Add((ObservableKeyValuePair<string, BackresItem>)item);
            }
            DialogResult = true;
            Close();
        }
    }
}
