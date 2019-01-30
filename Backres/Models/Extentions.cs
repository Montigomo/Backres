using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Backres.Models
{
	public static class Extentions
	{
		public static string NormilizePath(this string value, string itemName = null)
		{
			var localString = value;
			IDictionary<string, string> substitudeDictionary;
			if (String.IsNullOrWhiteSpace(itemName))
			{
				substitudeDictionary = BrConfig.PathKeys;
			}
			else
			{
				substitudeDictionary = new Dictionary<string, string>(BrConfig.PathKeys);
				substitudeDictionary.Add("Name", itemName);
			}

			foreach (KeyValuePair<string, string> kvp in substitudeDictionary)
			{
				localString = localString.Replace("{" + kvp.Key + "}", kvp.Value);
			}
			return localString;
		}

		public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

	}
}
