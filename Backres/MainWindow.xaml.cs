using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Reflection;
using System.Diagnostics;
using Backres.Models;
using Backres.Infrastructure;
using Microsoft.Win32;

namespace Backres
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class WindowMain : Window
	{
		public WindowMain()
		{
			InitializeComponent();
			Inititalize();
			Title = App.AppTitle;
		}

		public void Inititalize()
		{
			//_textBoxListener = new TextBoxTraceListener(txtDisplayTrace);
			Trace.Listeners.Add(new TextBoxTraceListener(txtDisplayTrace));

			dataGridMain.ItemsSource = BrConfig.Instance.Items;
			Trace.WriteLine("Application initialized.");
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


		public async Task RunItem(ActionDirection bDirection)
		{
			ToggleControls();
			var name = ((BrItem)dataGridMain.SelectedItem).Name;
			if (!String.IsNullOrWhiteSpace(name))
				await BrConfig.Instance.RunActions(name, bDirection);
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
	}
}
