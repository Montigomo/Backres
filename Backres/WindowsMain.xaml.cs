﻿using Backres.Infrastructure;
using Backres.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            Trace.Listeners.Add(new TextBoxTraceListener(txtDisplayTrace));
            try
            {
                dataGridMain.ItemsSource = BackresConfig.Instance.Items;

            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error - :{ex.Message}");
            }
            Trace.WriteLine($"Application initialized.");
            Trace.WriteLine($"Machine ID - {PcFingerPrint.Value()}");
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
                //string[] vars = control.Tag.ToString().Split('|');
                //if (vars.Length >= 3)
                //{
                //    //if (control is MenuItem)
                //    //	((MenuItem)control).Header = ((MenuItem)control).Header.ToString() == vars[1] ? vars[2] : vars[1];
                //    switch (control)
                //    {
                //        case MenuItem mi:
                //            ((MenuItem)control).Header = ((MenuItem)control).Header.ToString() == vars[1] ? vars[2] : vars[1];
                //            break;
                //        default:
                //            break;
                //    }

                //}
                control.IsEnabled = !control.IsEnabled;
            }

            // CancelButton.Enabled = true;

        }


        internal async Task RunItem(ActionDirection bDirection)
        {
            ToggleControls();
            try
            {
                var name = ((ObservableKeyValuePair<string, BackresItem>)dataGridMain.SelectedItem).Key;
                if (!String.IsNullOrWhiteSpace(name))
                    await BackresConfig.Instance.RunAction(name, bDirection);
            }
            catch (Exception e)
            {
                //await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (System.Action)(() => Trace.WriteLine(e.Message)));
                Trace.WriteLine(e.Message);
            }
            ToggleControls();

        }


        #region Context Menu Events

        private async void menuItemCmBackup_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Backup);
        }

        private async void menuItemCmRestore_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Restore);
        }

        private void menuItemAddItem_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new WndSelectAndAddItem();
            wnd.Owner = this;
            var dr = wnd.ShowDialog();
            BackresConfig.Instance.AddItems(wnd.SelectedItems);
        }

        #endregion

        private async void BntBackup_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Backup);
        }

        private async void BntRestore_Click(object sender, RoutedEventArgs e)
        {
            await RunItem(ActionDirection.Restore);
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

        private void Test() {
            var Overwrite = true;
            var SrcPath = "D:\\work\\csharp\\Backres\\Backres\\bin\\Debug\\net7.0-windows\\AGIDESKTOP (35C5-F948-6B27-059C-A533-5494-6749-7941)\\rar\\rarreg.key";
            var DstPath = "C:\\Program Files\\WinRAR\rarreg.key";
            File.Copy(SrcPath, DstPath, Overwrite);
        }

        private void dataGridMain_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                for(int i =0; i < grid.SelectedItems.Count; i++)
                {
                    var row = (ObservableKeyValuePair<string, BackresItem>)grid.SelectedItems[i];
                    BackresConfig.Instance.DeleteItem(row);
                }
            }
        }

        #region Top app menu

        private async void RunMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileMenuItemLoasStorage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileMenuItemSaveStorage_Click(object sender, RoutedEventArgs e)
        {
            ToggleControls();
            e.Handled = true;
            BackresConfig.Instance.SaveItems();
            ToggleControls();
        }

        private void TestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }

        #endregion
    }
}
