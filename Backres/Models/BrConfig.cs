using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Backres.Models
{
	public class BrConfig
	{

		#region Instance

		public static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

		private static string _jsonFilePath = System.IO.Path.Combine((System.IO.Path.GetDirectoryName(_exePath)), @"appsettings.json");

		private static string FilePath { get { return _jsonFilePath; } }


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

		public IList<BrItem> Items;

	}

	public class BrItem
	{
		public string Name { get; set; }

		public Dictionary<string, object> BackupAction { get; set; }

	}

	//public class BackupAction
	//{
	//	public List<CopyFiles> CopyFiles { get; set; } 
	//}

	//public class CopyFiles
	//{
	//	public string SrcFile { get; set; }
	//	public string DstFile { get; set; }
	//}

}

