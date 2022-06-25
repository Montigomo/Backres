using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;

namespace Backres.Models
{
	internal class BackresConfig
	{

		#region Instance
		
		private static readonly string _appPath = Environment.ProcessPath;

		private static readonly string _appDirectory = AppContext.BaseDirectory;

		private static string _filePath  = Path.Combine(_appDirectory, @"backres.json");

		private static string _brSettingFolder = _appDirectory;

		private static string _localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		private static string _appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		private static string _machineName = Environment.MachineName;

		public static Dictionary<string, string> PathKeys = new Dictionary<string, string>
		{
			{ "AppData" , _appDataFolder },
			{ "LocalAppData" , _localAppDataFolder },
			{ "BrSettingFolder", _brSettingFolder },
			{ "MachineName",  _machineName}
		};

		private static readonly Lazy<BackresConfig> _instance = new Lazy<BackresConfig>(Load);

		internal static BackresConfig Instance
		{
			get
			{
				return _instance.Value;
			}
		}


		private static BackresConfig Load()
		{

			var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };

			var config = new BackresConfig()
			{
				Items = JsonSerializer.Deserialize<List<BackresItem>>(File.ReadAllText(_filePath), options)?.ToList()
			};

			return config;
		}


		#endregion

		#region Properties 

		public IList<BackresItem> Items;
		
		#endregion

		#region Backup Restore

		public Task<bool> RunActions(string name, ActionDirection aDirection)
		{
			var tcs = new TaskCompletionSource<bool>();
			
			Task.Factory.StartNew(/*async*/ () =>
			{
				try
				{
					BackresItem Item = Items.FirstOrDefault(item => item.Name == name);
					var ActionItems = aDirection == ActionDirection.Backup ? Item.BackupActions.OrderBy(i => i.Order) : Item.RestoreActions.OrderBy(i => i.Order);
					foreach (var action in ActionItems)
					{
						var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Action" + action.ActionName);

						if (type == null)
							continue;
						action.ItemName = name;
						IAction iaction = (IAction)Activator.CreateInstance(type, action, aDirection);

						iaction.Run();
					}
				}
				catch(Exception e)
				{
					tcs.SetException(e);
				}
				tcs.SetResult(true);
			}, TaskCreationOptions.None);
			return tcs.Task;
		}

		#endregion

	}

}

