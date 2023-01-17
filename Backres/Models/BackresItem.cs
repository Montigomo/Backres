using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backres.Models
{
	internal class BackresItem
	{

		//[JsonConstructor]
		public BackresItem(string name)
		{
			Name = name;
		}

		//[OnDeserialized]
		//internal void OnDeserializedMethod(StreamingContext context)
		//{
		//	Name = "bbb";
		//}

		//[OnDeserializing]
		//internal void OnDeserializingMethod(StreamingContext context)
		//{
		//	Name = "bbb";
		//}

		public string Name { get; }

		public bool IsMachinable { get; set; } = false;

		public List<BackresAction> BackupActions { get; set; }

		public List<BackresAction> RestoreActions { get; set; }

	}
}
