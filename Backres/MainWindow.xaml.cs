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

		}


		private void menuItemCmBackup_Click(object sender, RoutedEventArgs e)
		{
		}

		private void menuItemCmRestore_Click(object sender, RoutedEventArgs e)
		{

		}

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

		private void ButtonTest_Click(object sender, RoutedEventArgs e)
		{
			var t = BrConfig.Instance;
		}
	}
}
