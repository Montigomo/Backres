using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backres.Models
{
	public static class Extentions
	{
		public static string NormilizePath(this string value)
		{
			var localString = value;
			foreach (KeyValuePair<string, string> kvp in BrConfig.PathKeys)
			{
				localString = localString.Replace("{" + kvp.Key + "}", kvp.Value);
			}
			return localString;
		}
	}
}
