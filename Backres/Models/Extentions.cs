using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Backres.Models
{
    internal static class Extentions
    {
        public static string NormilizePath(this string value, ActionAbstract action = null)
        {
            var localString = new StringBuilder(value);

            Regex rx = new Regex(@"\{(?<name>[^\}\{]+)\}", RegexOptions.Compiled | RegexOptions.Multiline);
            MatchCollection matches = rx.Matches(value);

            var names = Enum.GetNames(typeof(System.Environment.SpecialFolder));

            for (int ctr = 0; ctr < matches.Count; ctr++)
            {
                var specialFolderName = matches[ctr].Groups["name"].Value;
                if (names.Contains(specialFolderName))
                {
                    var specialFolderEnumValue = (System.Environment.SpecialFolder)Enum.Parse(typeof(System.Environment.SpecialFolder), specialFolderName);
                    var specialFolderValue = Environment.GetFolderPath(specialFolderEnumValue);
                    localString.Remove(matches[ctr].Index, matches[ctr].Length);
                    localString.Insert(matches[ctr].Index, specialFolderValue);
                }

            }

            var localReplacement = new Dictionary<string, string>() {
                { "{StorageFolder}", BackresConfig.Instance.Storage },
                { "{Name}", action.ItemName }
            };

            foreach (var item in localReplacement)
            {
                localString.Replace(item.Key, item.Value);
            }

            return localString.ToString();
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
