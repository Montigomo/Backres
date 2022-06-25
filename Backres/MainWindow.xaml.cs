using Backres.Infrastructure;
using Backres.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Backres
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Inititalize();
            Title = App.AppTitle;
        }

        public void Inititalize()
        {
            //_textBoxListener = new TextBoxTraceListener(txtDisplayTrace);
            var pcfingerprint = PcFingerPrint.Value();

            Trace.Listeners.Add(new TextBoxTraceListener(txtDisplayTrace));
            try
            {
                dataGridMain.ItemsSource = BackresConfig.Instance.Items;

                //var _appDirectory = AppContext.BaseDirectory;

                //var _filePath = System.IO.Path.Combine(_appDirectory, @"backres.json");

                //Trace.WriteLine($"Application initialized. {_filePath} {Environment.NewLine} {Environment.ProcessPath}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error - :{ex.Message}");
            }
            Trace.WriteLine($"Application initialized.");
        }

        #region Commands

        public string GetUninstallCommandFor(string productDisplayName)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            string productsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products";
            RegistryKey products = localMachine.OpenSubKey(productsRoot);
            string[] productFolders = products.GetSubKeyNames();

            foreach (string p in productFolders)
            {
                RegistryKey installProperties = products.OpenSubKey(p + @"\InstallProperties");
                if (installProperties != null)
                {
                    string displayName = (string)installProperties.GetValue("DisplayName");
                    if ((displayName != null) && (displayName.Contains(productDisplayName)))
                    {
                        string uninstallCommand = (string)installProperties.GetValue("UninstallString");
                        return uninstallCommand;
                    }
                }
            }

            return "";

        }

        #endregion

        private void ToggleControls(params Control[] excludeControls)
        {
            var tc = this.FindVisualChildren<Control>().Where(ctrl => ctrl.Tag != null && ctrl.Tag.ToString().StartsWith("toggle") && !excludeControls.Contains(ctrl)).ToList();

            foreach (var control in tc)
            {
                string[] vars = control.Tag.ToString().Split('|');
                if (vars.Length >= 3)
                {
                    //if (control is MenuItem)
                    //	((MenuItem)control).Header = ((MenuItem)control).Header.ToString() == vars[1] ? vars[2] : vars[1];
                    switch (control)
                    {
                        case MenuItem mi:
                            ((MenuItem)control).Header = ((MenuItem)control).Header.ToString() == vars[1] ? vars[2] : vars[1];
                            break;
                        default:
                            break;
                    }

                }
                control.IsEnabled = !control.IsEnabled;
            }

            // CancelButton.Enabled = true;
        }


        internal async Task RunItem(ActionDirection bDirection)
        {
            ToggleControls();
            try
            {
                var name = ((BackresItem)dataGridMain.SelectedItem).Name;
                if (!String.IsNullOrWhiteSpace(name))
                    await BackresConfig.Instance.RunActions(name, bDirection);
            }
            catch (Exception e)
            {
                //await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (System.Action)(() => Trace.WriteLine(e.Message)));
                Trace.WriteLine(e.Message);
            }
            ToggleControls();

        }


        private async void menuItemCmBackup_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Backup);
        }

        private async void menuItemCmRestore_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Restore);
        }

        private async void BntBackup_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Backup);
        }

        private async void BntRestore_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Restore);
        }

        private async void RunMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleControls();
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
            });
            ToggleControls();
        }

        private void WindowMain1_Loaded(object sender, RoutedEventArgs e)
        {
            // Find first child of type ScrollViewer in object Foo
            var scrollViewer = VisualTreeHelpers.FindChild<ScrollViewer>(dataGridMain);

            // If object is found, set other control's height to ViewportHeight
            if (scrollViewer != null)
            {
                //MyControl.Height = scrollViewer.ViewportHeight;
            }
        }
    }
}
