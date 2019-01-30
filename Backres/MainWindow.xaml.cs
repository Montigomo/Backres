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


		public async Task Backup()
		{
			var name = ((BrItem)dataGridMain.SelectedItem).Name;
			if (!String.IsNullOrWhiteSpace(name))
				await BrConfig.Instance.Backup(name);
		}

		public async Task Restore()
		{
			var name = ((BrItem)dataGridMain.SelectedItem).Name;
			if (!String.IsNullOrWhiteSpace(name))
				await BrConfig.Instance.Restore(name);
		}


		private async void menuItemCmBackup_Click(object sender, RoutedEventArgs e)
		{
			await Backup();
		}

		private async void menuItemCmRestore_Click(object sender, RoutedEventArgs e)
		{
			await Restore();
		}

		private async void BntBackup_Click(object sender, RoutedEventArgs e)
		{
			await Backup();
		}

		private async void BntRestore_Click(object sender, RoutedEventArgs e)
		{
			await Restore();
		}

		private void ButtonTest_Click(object sender, RoutedEventArgs e)
		{
			var t = BrConfig.Instance;
		}


	}
}
