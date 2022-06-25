using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Backres.Models
{
    internal class BackresAction
    {
		public string ItemName { get; set; }

		[JsonPropertyName("Name")]
		public string ActionName { get; set; }

		public int Order { get; set; }

		public bool Overwrite { get; set; } = true;

		public string SrcPath { get; set; }

		public string DstPath { get; set; }

		public string RegistryKey { get; set; }
	}
}
