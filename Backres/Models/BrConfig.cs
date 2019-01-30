using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

		#region Backup Restore

		public Task<bool> RunActions(string name, ActionDirection aDirection)
		{
			var tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(/*async*/ () =>
			{
				BrItem Item = Items.FirstOrDefault(item => item.Name == name);
				var ActionItems = aDirection == ActionDirection.Backup ? Item.BackupActions.OrderBy(i => i.Order) : Item.RestoreActions.OrderBy(i => i.Order);
				foreach (var action in ActionItems)
				{
					var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Action" + action.Name);

					if (type == null)
						continue;

					IAction iaction = (IAction)Activator.CreateInstance(type, action, aDirection);

					iaction.Run();
				}
				Thread.Sleep(1000);
				tcs.SetResult(true);
			}, TaskCreationOptions.LongRunning);
			return tcs.Task;
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

		public bool Overwrite { get; set; } = true;

		public string SrcPath { get; set; }

		public string DstPath { get; set; }

		public string RegistryKey { get; set; }

	}

	public interface IAction
	{
		bool Run();

		Task<bool> RunAsync();
	}


	public enum ActionDirection
	{
		Backup,
		Restore
	}

}

