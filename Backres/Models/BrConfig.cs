using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Backres.Models
{
	public class BrConfig
	{

		#region Instance
		
		private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

		private static readonly string _exeDirectory = Path.GetDirectoryName(_exePath);

		private static string FilePath { get; } = Path.Combine(_exeDirectory, @"appsettings.json");

		private static string _brSettingFolder = _exeDirectory;

		private static string _localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		private static string _appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		public static Dictionary<string, string> PathKeys = new Dictionary<string, string>
		{
			{ "AppData" , _appDataFolder },
			{ "LocalAppData" , _localAppDataFolder },
			{ "BrSettingFolder", _brSettingFolder }
		};

		private static readonly Lazy<BrConfig> _instance = new Lazy<BrConfig>(Load);

		internal static BrConfig Instance
		{
			get
			{
				return _instance.Value;
			}
		}


		private static BrConfig Load()
		{
			var config = new BrConfig()
			{
				Items = JsonConvert.DeserializeObject<List<BrItem>>(File.ReadAllText(FilePath))
			};

			return config;
		}


		#endregion

		#region Properties 

		public IList<BrItem> Items;
		
		#endregion

		#region Backup

		public async Task Backup()
		{
			foreach (var item in Items)
				await Backup(item.Name);
		}

		public async Task Backup(string name)
		{
			BrItem Item = Items.FirstOrDefault(item => item.Name == name);
			if (Item == null)
				return;
			foreach (var backupAction in Item.BackupActions.OrderBy(i => i.Order))
			{
				var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Action" + backupAction.Name);

				if (type == null)
					continue;

				IAction action = (IAction)Activator.CreateInstance(type, backupAction, ActionDirection.Backup);

				await action.Run();
			}
		}

		#endregion

		#region Restore

		public async Task Restore()
		{
			foreach (var item in Items)
				await Restore(item.Name);
		}

		public async Task Restore(string name)
		{
			BrItem Item = Items.FirstOrDefault(item => item.Name == name);
			if (Item == null)
				return;
			foreach (var restoreAction in Item.RestoreActions.OrderBy(i => i.Order))
			{
				var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Action" + restoreAction.Name);

				if (type == null)
					continue;

				IAction action = (IAction)Activator.CreateInstance(type, restoreAction, ActionDirection.Restore);

				await action.Run();
			}
		}

		#endregion

	}
	 
	public class BrItem
	{
		public string Name { get; set; }

		public List<BrAction> BackupActions { get; set; }

		public List<BrAction> RestoreActions { get; set; }

	}

	public class BrAction
	{
		public string Name { get; set; }

		public int Order { get; set; }

		public string SrcFile { get; set; }

		public string DstFile { get; set; }

		public string RegistryKey { get; set; }

	}

	public interface IAction
	{
		Task<bool> Run();
	}


	public enum ActionDirection
	{
		Backup,
		Restore
	}

}

