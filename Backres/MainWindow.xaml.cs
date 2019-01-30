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
using System.Reflection;
using Backres.Models;
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
			dataGridMain.ItemsSource = BrConfig.Instance.Items;
		}

		#region 
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
			var tc = this.FindVisualChildren<Control>().Where(ctrl => (ctrl.Tag?.ToString() == "toggle") && !excludeControls.Contains(ctrl)).ToList();
			foreach (var control in tc)
			{
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

		private void ButtonTest_Click(object sender, RoutedEventArgs e)
		{
			var t = BrConfig.Instance;
		}


	}
}
